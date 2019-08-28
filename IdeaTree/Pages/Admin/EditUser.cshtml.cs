using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdeaTree.Models;
using System.ComponentModel.DataAnnotations;

namespace IdeaTree.Pages.Admin
{
    
    public class EditUserModel : PageModel
    {
        private readonly IdeaTree.Models.IdeaTreeContext _context;

        public EditUserModel(IdeaTree.Models.IdeaTreeContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        public int MemberID { get; set; }
        [BindProperty]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        [BindProperty]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [BindProperty]
        public string Country { get; set; }
        [BindProperty]
        public bool Newsletter { get; set; }
        [BindProperty]
        [Required]
        public StatusType Status { get; set; }

        public List<Country> Countries { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member m= await _context.Member.FirstOrDefaultAsync(f => f.ID == id);

            if (m == null)
            {
                return NotFound();
            }
            else
            {
                MemberID = m.ID;
                Name = m.Name;
                Email = m.Email;
                Country = m.Country;
                Phone = m.Phone;
                Newsletter = m.Newsletter;
                Status = m.Status;
            }

            Countries = new List<Country> {
        new Country { Abbr = "", Full="" },
        new Country { Abbr = "ind", Full="India" }
    };
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Member m = await _context.Member.FirstOrDefaultAsync(f => f.ID == MemberID);

            if (m == null)
            {
                return NotFound();
            }
            else
            {
                m.Name = Name;
                m.Email = Email;
                m.Country = Country;
                m.Phone = Phone;
                m.Newsletter = Newsletter;
                m.Status = Status;
                m.ModifiedDate = DateTime.UtcNow;
                m.ModifiedBy = null;
            }

                _context.Attach(m).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(m.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Users");
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.ID == id);
        }
    }

}
