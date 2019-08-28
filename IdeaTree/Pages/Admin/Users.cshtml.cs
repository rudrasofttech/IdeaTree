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
    public class UsersModel : PageModel
    {
        private readonly IdeaTree.Models.IdeaTreeContext _context;

        public UsersModel(IdeaTree.Models.IdeaTreeContext context)
        {
            _context = context;
        }

        public IList<Member> Member { get;set; }

        public async Task OnGetAsync()
        {
            Member = await _context.Member.OrderByDescending(o => o.CreateDate).ToListAsync();
        }
    }
}
