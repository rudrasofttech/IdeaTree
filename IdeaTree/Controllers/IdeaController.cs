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

using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Web;

namespace IdeaTree.Controllers
{

    public class IdeaController : Controller
    {
        private readonly IdeaTreeContext _context;
        private IHostingEnvironment _env;

        public IdeaController(IdeaTreeContext context, IHostingEnvironment env)
        {
            _context = context;

            _env = env;

        }

        [HttpGet]
        public IActionResult Index(string id)
        {
            IdeaPublicModel model = new IdeaPublicModel();
            var idea = _context.Idea
                .Include(i => i.PostedBy)
                .Include(i => i.ModifiedBy)
                .Include(i => i.IdeaImage)
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

                    if (idea.PostedBy.ID == m.ID || m.MType == MemberType.Admin) { model.IsAdminOrOwner = true; }
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
        public IActionResult EditIdea(int Id, string IdeaTitle, string Description, string video, int Id1, string AllImagepath)
        {
            string[] imgToRemove = null;
            if (!string.IsNullOrEmpty(AllImagepath))
            {
                imgToRemove = AllImagepath.Split(',');
            }
            var postedIdea = _context.Idea.Where(x => x.ID == Id || x.ID == Id1).SingleOrDefault();
            var Imageidea = _context.IdeaImages.Where(x => x.Id == Id1);

            if (imgToRemove != null && imgToRemove.Length > 0)
            {
                for (int i = 0; i < imgToRemove.Length; i++)
                {
                    var img = Imageidea.SingleOrDefault(x => x.Image.ToUpper() == imgToRemove[i].Trim().Substring(8).ToUpper());
                    _context.IdeaImages.Remove(img);
                }
                _context.SaveChanges();
            };

            foreach (IFormFile file in Request.Form.Files)
            {
                string fileName = Path.GetFileName(file.FileName);
                fileName = string.Format("{0}-{1}", Guid.NewGuid(), fileName);
                var webRoot = _env.WebRootPath;
                //var PathWithFolderName = System.IO.Path.Combine(webRoot, "MyFolder");
                var image = new IdeaImages();
                image.Image = fileName;
                image.Id = Id1;
                _context.IdeaImages.Add(image);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(fileStream);
                }
            }


            if (IdeaTitle != null)
                postedIdea.Title = IdeaTitle;
            if (Description != null)
                postedIdea.Description = Description;
            if (video != null)
                postedIdea.Video = video;

            _context.Update(postedIdea);
            _context.SaveChanges();
            return Json("success");
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
