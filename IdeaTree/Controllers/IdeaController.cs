using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdeaTree;
using IdeaTree.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdeaTree.Controllers
{

    public class IdeaController : Controller
    {
        private readonly IdeaTreeContext _context;

        public IdeaController(IdeaTreeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string id)
        {
            IdeaPublicModel model = new IdeaPublicModel();
            var idea = _context.Idea
                .Include(i => i.PostedBy)
                .Include(i => i.ModifiedBy)
                .Include(i=>i.IdeaImage)
                .FirstOrDefault(f => Utility.TextToURL(f.Title) == id.Trim());
            if (idea != null)
            {
                model.VoteCount = _context.Vote.Count(t => t.VoteTo.ID == idea.ID);
                if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
                {
                    Member m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);
                    var vote = _context.Vote.FirstOrDefault(t => t.VoteTo.ID == idea.ID && t.VotedBy.ID == m.ID);
                    if (vote == null) { model.HasVoted = false; }
                    else { model.HasVoted = true; }

                    if (idea.PostedBy.ID == m.ID || m.MType== MemberType.Admin) { model.IsAdminOrOwner = true; }
                }
                model.Comments.AddRange(_context.Comment.Include(i => i.PostedBy).Where(t => t.PostedTo.ID == idea.ID && t.Status != StatusType.Deleted).OrderByDescending(t => t.CreateDate).ToList());
                model.OtherIdeasFromOwner.AddRange(_context.Idea.OrderByDescending(t => t.PostDate).Where(t => t.PostedBy.ID == idea.PostedBy.ID && t.ID != idea.ID).ToList());
            }
            model.idea = idea;
            return View(model);
        }

        //EditIdea
        [HttpPost]
        [Authorize]
        public IActionResult EditIdea(int Id, string IdeaTitle, string Description,string video)
        {
            var postedIdea = _context.Idea.SingleOrDefault(x => x.ID == Id);

            if (postedIdea != null)
            {
                if (IdeaTitle != null)
                    postedIdea.Title = IdeaTitle;
                if (Description != null)
                    postedIdea.Description = Description;
                if(video!=null)
                    postedIdea.Video = video;

                _context.Update(postedIdea);
                _context.SaveChanges();
                return Json("success");
            }
            else
            {
                return Json("Idea not found!");
            }

        }
        [HttpGet]
        [Authorize]
        public JsonResult Vote(string id)
        {
            if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
            {
                var idea = _context.Idea
                    .FirstOrDefault(f => Utility.TextToURL(f.Title) == id.Trim());

                Member m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);


                if (idea != null && m != null)
                {
                    var vote = _context.Vote.FirstOrDefault(t => t.VoteTo.ID == idea.ID && t.VotedBy.ID == m.ID);
                    if (vote == null)
                    {
                        vote = new Vote();
                        vote.VotedBy = m;
                        vote.VoteTo = idea;
                        vote.CreateDate = DateTime.UtcNow;
                        _context.Add(vote);
                        _context.SaveChanges();
                        int count = _context.Vote.Count(t => t.VoteTo.ID == idea.ID);
                        return Json(new { result = true, voted = true, count = count });
                    }
                    else
                    {
                        _context.Remove(vote);
                        _context.SaveChanges();
                        int count = _context.Vote.Count(t => t.VoteTo.ID == idea.ID);
                        return Json(new { result = true, voted = false, count = count });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Something went wrong." });
                }
            }
            else
            {
                return Json(new { result = false, message = "You are not logged in." });
            }
        }

        private bool IdeaExists(int id)
        {
            return _context.Idea.Any(e => e.ID == id);
        }

        [Authorize]
        [HttpPost]
        public JsonResult PostComment(string id, string comment)
        {
            if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
            {
                var idea = _context.Idea
                    .FirstOrDefault(f => Utility.TextToURL(f.Title) == id.Trim());
                Member m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);


                if (idea != null && m != null)
                {
                    var c = new Comment();
                    c.Content = comment;
                    c.CreateDate = DateTime.UtcNow;
                    c.PostedBy = m;
                    c.PostedTo = idea;
                    c.Status = StatusType.Unapproved;
                    _context.Add<Comment>(c);
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                else
                {
                    return Json(new { result = false, message = "Something went wrong." });
                }
            }
            else
            {
                return Json(new { result = false, message = "You are not logged in." });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteComment(int id)
        {
            if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
            {

                var ct = _context.Comment.FirstOrDefault(f => f.ID == id && f.PostedBy.Phone == HttpContext.User.Identity.Name);

                if (ct != null)
                {
                    _context.Remove<Comment>(ct);
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                else
                {
                    return Json(new { result = false, message = "Something went wrong." });
                }
            }
            else
            {
                return Json(new { result = false, message = "You are not logged in." });
            }
        }
    }
}
