using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentsCommunity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;

namespace StudentsCommunity.Controllers
{  [Authorize]
    public class PersonalInformationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PersonalInformations
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

            // Look up existing record
            var personalInformation = db.PersonalInformations
                                                .FirstOrDefault(p => p.UserId == currentUserId);
            return View(personalInformation);
        }
        
        // GET: PersonalInformations/Create
        public ActionResult AddOrEdit()
        {
            var currentUserId = User.Identity.GetUserId();
            var PersonalInformation = db.PersonalInformations.Where(x => x.UserId == currentUserId).FirstOrDefault<PersonalInformation>();
            if (PersonalInformation == null)
            {
                return View(new PersonalInformation());
            }
            else
                return View(PersonalInformation);
        }

        // POST: PersonalInformations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit(PersonalInformation personalInformation, HttpPostedFileBase upload)
        {
            var currentUserId = User.Identity.GetUserId();
            personalInformation.UserId = currentUserId;
            var currentInformation = db.PersonalInformations.Where(x => x.UserId == currentUserId).FirstOrDefault<PersonalInformation>();
            var CurrentUser = db.Users.Where(a => a.Id == currentUserId).SingleOrDefault();

            if (currentInformation == null)
            {
                //make upload optional 
                if (upload != null)
                {
                string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);
                upload.SaveAs(path);
                personalInformation.UserImage = upload.FileName;
                }

                db.PersonalInformations.Add(personalInformation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                
                string oldPath = Path.Combine(Server.MapPath("~/Uploads"), personalInformation.UserImage);

                if (upload != null)
                {   //leave defult image
                    if (personalInformation.UserImage != "img.png")
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss")+ upload.FileName);
                    upload.SaveAs(path);
                personalInformation.UserImage = DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + upload.FileName;
            }


            // Update user
            var store = new UserStore<ApplicationUser>(db);
                var manager = new ApplicationUserManager(store);
                var user = manager.Users.FirstOrDefault(u => u.Id == currentUserId);
                user.PersonalInformation.FullName = personalInformation.FullName;
                user.PersonalInformation.StudentId =personalInformation.StudentId ;
                user.PersonalInformation.UserImage = personalInformation.UserImage;
                user.PersonalInformation.Skills = personalInformation.Skills;
                manager.Update(user);
                return RedirectToAction("Index");
            }
            
        }

        
    }
}

