﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdeaTree.Models;

namespace IdeaTree.Pages.Admin
{
    public class IdeasModel : PageModel
    {
        private readonly IdeaTree.Models.IdeaTreeContext _context;

        public IdeasModel(IdeaTree.Models.IdeaTreeContext context)
        {
            _context = context;
        }

        public IList<Idea> Idea { get;set; }

        public async Task OnGetAsync()
        {
            Idea = await _context.Idea.OrderByDescending(o => o.PostDate).ToListAsync();
        }
    }
}