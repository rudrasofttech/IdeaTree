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
    public class EditIdeaModel : PageModel
    {
        private readonly IdeaTree.Models.IdeaTreeContext _context;

        public EditIdeaModel(IdeaTree.Models.IdeaTreeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int IdeaID { get; set; }
        [BindProperty]
        [Required]
        [MaxLength(250)]
        
        public string IdeaTitle { get; set; }

        [BindProperty]
        [Required]
        
        public string IdeaDescription { get; set; }

        [BindProperty]
        [Required]
        public string IdeaTags { get; set; }
        [BindProperty]
        [Required]
        public StatusType IdeaStatus { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Idea item = await _context.Idea.FirstOrDefaultAsync(m => m.ID == id);

            if (item == null)
            {
                return NotFound();
            }
            else
            {
                IdeaID = item.ID;
                IdeaTitle = item.Title;
                IdeaDescription = item.Description;
                IdeaTags = item.Tags;
                IdeaStatus = item.Status;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Idea item = await _context.Idea.FirstOrDefaultAsync(m => m.ID == IdeaID);
            Member mem = await _context.Member.FirstOrDefaultAsync(m => m.Phone == HttpContext.User.Identity.Name);
            item.Title = IdeaTitle;
            item.Description = IdeaDescription;
            item.Status = IdeaStatus;
            item.Tags = IdeaTags;
            item.ModifiedBy = mem;
            item.ModifyDate = DateTime.UtcNow;


            _context.Attach(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IdeaExists(item.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Ideas");
        }

        private bool IdeaExists(int id)
        {
            return _context.Idea.Any(e => e.ID == id);
        }
    }
}
