using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdeaTree.Models;

namespace IdeaTree.Pages.Admin
{
    public class CommentsModel : PageModel
    {
        private readonly IdeaTree.Models.IdeaTreeContext _context;

        public CommentsModel(IdeaTree.Models.IdeaTreeContext context)
        {
            _context = context;
        }

        public IList<Comment> Comment { get;set; }

        public async Task OnGetAsync()
        {
            Comment = await _context.Comment.Include(i => i.PostedTo).Include(i => i.PostedBy).OrderByDescending(o => o.CreateDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            int commentid;
            if(int.TryParse(Request.Form["commentid"], out commentid))
            {
                var c = _context.Comment.FirstOrDefault(i => i.ID == commentid);
                if(c != null)
                {
                    _context.Comment.Remove(c);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("./Comments");
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            int commentid;
            if (int.TryParse(Request.Form["commentid"], out commentid))
            {
                var c = _context.Comment.FirstOrDefault(i => i.ID == commentid);
                if (c != null)
                {
                    c.Status = StatusType.Active;
                    _context.Attach(c).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("./Comments");
        }

    }
}
