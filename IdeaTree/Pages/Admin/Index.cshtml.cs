using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaTree.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdeaTree.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IdeaTree.Models.IdeaTreeContext _context;

        public int VoteCount { get; set; }
        public int IdeaCount { get; set; }
        public int MemberCount { get; set; }
        public int CommentCount { get; set; }
        public Idea LastIdea { get; set; }
        public Comment LastComment { get; set; }
        public Member LastMember { get; set; }
        public Vote LastVote { get; set; }

        public IndexModel(IdeaTree.Models.IdeaTreeContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            VoteCount = _context.Vote.Count();
            IdeaCount = _context.Idea.Count(t => t.Status != Models.StatusType.Deleted);
            MemberCount = _context.Member.Count(t => t.Status != Models.StatusType.Deleted);
            CommentCount = _context.Comment.Count(t => t.Status != Models.StatusType.Deleted);
            LastIdea = _context.Idea.OrderByDescending(t => t.PostDate).FirstOrDefault();
            LastComment = _context.Comment.OrderByDescending(t => t.CreateDate).FirstOrDefault();
            LastMember = _context.Member.OrderByDescending(t => t.CreateDate).FirstOrDefault();
            LastVote = _context.Vote.OrderByDescending(t => t.CreateDate).FirstOrDefault();
        }
    }
}