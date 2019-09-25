using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdeaTree.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;

namespace IdeaTree.Controllers
{
    public class HomeController : Controller
    {
        private readonly IdeaTreeContext _context;

        public HomeController(IdeaTreeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new HomeModel();
            model.Ideas = new List<Idea>();
            model.Ideas.AddRange(_context.Idea.Include(i => i.PostedBy).OrderByDescending(t => t.PostDate).Take(10).ToList());
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ideas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateIdeaModel model)
        {
            if (ModelState.IsValid)
            {
                Idea i = _context.Idea.FirstOrDefault(temp => temp.Title.ToLower().Trim() == model.Title.Trim().ToLower());
                if (i == null)
                {
                    i = new Idea();
                    //i.Category = model.Category;
                    i.Description = model.Description;
                    i.PostDate = DateTime.UtcNow;
                    if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
                    {
                        Member m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);
                        if (m != null)
                        {
                            i.PostedBy = m;
                        }
                    }
                    i.Status = StatusType.Active;
                    i.Title = model.Title;
                    _context.Add(i);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("DuplicateTitle", "Another idea with same title is found.");
                }
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult login(string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        public IActionResult signup(string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult signup(SignupModel model, string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                Member m = _context.Member.FirstOrDefault(t => t.Phone == model.Phone || t.Name == model.Name);
                if (m == null)
                {
                    m = new Member();
                    m.Phone = model.Phone;
                    m.Name = model.Name;
                    m.Newsletter = model.Newsletter;
                    m.CreateDate = DateTime.UtcNow;
                    m.MType = MemberType.Member;
                    m.Status = StatusType.Unapproved;
                    Random random = new Random();
                    string otp = string.Format("{0}{1}{2}{3}{4}{5}", random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString());
                    m.Password = otp;
                    m.PasswordGenerateDate = DateTime.UtcNow;
                    _context.Add(m);
                    _context.SaveChanges();
                    if (string.IsNullOrEmpty(ReturnUrl))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return Redirect(ReturnUrl);
                    }
                }
                else
                {
                    if (_context.Member.Count(t => t.Phone == m.Phone) > 0)
                    {
                        ModelState.AddModelError("DuplicatePhone", "Phone is registered.");
                    }
                    if (_context.Member.Count(t => t.Name == m.Name) > 0)
                    {
                        ModelState.AddModelError("DuplicateName", "Choose a unique profile name.");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult generateotp(string phone, string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            try
            {
                Member m = _context.Member.FirstOrDefault(t => t.Phone == phone);
                if (m != null)
                {
                    Random random = new Random();
                    string otp = string.Format("{0}{1}{2}{3}{4}{5}", random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString(), random.Next(0, 9).ToString());
                    m.Password = otp.ToString(); //Encrypt(otp.ToString());
                    m.PasswordGenerateDate = DateTime.UtcNow;
                    _context.Update<Member>(m);
                    _context.SaveChanges();
                    return Json(new { result = true, message = "success" });
                }
                else
                {
                    return Json(new { result = false, message = "notfound" });
                }
            }
            catch
            {
                return Json(new { result = false, message = "notfound" });
            }
        }

        [HttpPost]
        public IActionResult login(LoginModel model, [FromQuery] string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                Member m = _context.Member.FirstOrDefault(t => t.Phone == model.Phone && t.Password == model.Password && t.Status != StatusType.Deleted);
                if (m != null)
                {
                    if (m.PasswordGenerateDate < DateTime.UtcNow.AddMinutes(-20))
                    {
                        ModelState.AddModelError("Error", "OTP Expired, generate new one.");
                    }
                    else
                    {
                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, m.Phone),
                            new Claim("FullName", m.Name),
                            new Claim(ClaimTypes.Role, m.MType.ToString()),
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            //AllowRefresh = <bool>,
                            // Refreshing the authentication session should be allowed.

                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24),
                            // The time at which the authentication ticket expires. A 
                            // value set here overrides the ExpireTimeSpan option of 
                            // CookieAuthenticationOptions set with AddCookie.

                            IsPersistent = true,
                            // Whether the authentication session is persisted across 
                            // multiple requests. When used with cookies, controls
                            // whether the cookie's lifetime is absolute (matching the
                            // lifetime of the authentication ticket) or session-based.

                            IssuedUtc = DateTime.UtcNow
                            // The time at which the authentication ticket was issued.

                            //RedirectUri = <string>
                            // The full path or absolute URI to be used as an http 
                            // redirect response value.
                        };
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        //set last logon date 
                        m.LastLogon = DateTime.UtcNow;
                        _context.Update<Member>(m);
                        _context.SaveChanges();
                        if (string.IsNullOrEmpty(ReturnUrl))
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return Redirect(ReturnUrl);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "Invalid log in information.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> signout()
        {
            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Profile(string name = "")
        {
            ProfilePublicModel model = new ProfilePublicModel();
            Member m = null;
            if (name != "")
            {
                m = _context.Member.FirstOrDefault(temp => temp.Name == name);
            }
            else if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
            {
                m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);
            }
            if (m != null)
            {
                model.Member = m;
                model.Ideas = _context.Idea.Where(t => t.PostedBy.ID == m.ID).ToList();
                return View(model);
            }
            else
            {
                Response.StatusCode = 404;
                return View("ProfileNotFound");
            }
        }

        [Authorize]
        public IActionResult Manage()
        {
            if (HttpContext.User.Identities.Any(u => u.IsAuthenticated))
            {
                Member m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);
                ManageProfileModel model = new ManageProfileModel();
                model.Bio = m.Bio;
                model.Email = m.Email;
                model.FullName = m.FullName;
                model.Image = m.Image;
                model.Name = m.Name;
                model.Newsletter = m.Newsletter;
                model.Phone = m.Phone;

                return View(model);
            }
            else
            {
                return RedirectToAction("login");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Manage(ManageProfileModel model)
        {
            try
            {
                if (model.Image != null)
                {
                    var imageName = string.Format("{0}.jpg", Guid.NewGuid());
                    string base64 = model.Image.Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(base64);

                    using (Image image = Image.FromStream(new MemoryStream(bytes)))
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), @IdeaTree.Utility.ProfileImagePath, imageName);
                        image.Save(path, ImageFormat.Jpeg);
                        model.Image = imageName;
                    }
                }

                Member m = _context.Member.FirstOrDefault(temp => temp.Phone == HttpContext.User.Identity.Name);
                m.Bio = model.Bio;
                m.Email = model.Email;
                m.FullName = model.FullName;
                m.Image = model.Image;
                m.Newsletter = model.Newsletter;
                _context.Attach(m).State = EntityState.Modified;
                _context.SaveChanges();


                return RedirectToAction("Profile", new { name = m.Name });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "We facing a technical issue, please try after some time.");
                Utility.ReportException(ex.Message, "Error in Home/Manage.");
                return View(model);
            }
        }

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "HGFTR5632548EWSFHBVCD458@#%";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        //private string Decrypt(string cipherText)
        //{
        //    string EncryptionKey = "HGFTR5632548EWSFHBVCD458@#%";
        //    byte[] cipherBytes = Convert.FromBase64String(cipherText);
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        encryptor.Key = pdb.GetBytes(32);
        //        encryptor.IV = pdb.GetBytes(16);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        //            {
        //                cs.Write(cipherBytes, 0, cipherBytes.Length);
        //                cs.Close();
        //            }
        //            cipherText = Encoding.Unicode.GetString(ms.ToArray());
        //        }
        //    }
        //    return cipherText;
        //}
    }
}
