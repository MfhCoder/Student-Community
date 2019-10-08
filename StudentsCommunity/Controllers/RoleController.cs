using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentsCommunity.Models;
using StudentsCommunity.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Controllers
{
  [Authorize/*(Roles = "Admins")*/]
        public class RolesController : Controller
        {
            ApplicationDbContext db = new ApplicationDbContext();
            // GET: Roles
            public ActionResult Index()
            {
                return View(db.Roles.ToList());
            }

            // GET: Roles/Details/5
            public ActionResult Details(string id)
            {
                var role = db.Roles.Find(id);
                if (role == null)
                {
                    return HttpNotFound();
                }

                return View(role);
            }

            // GET: Roles/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Roles/Create
            [HttpPost]
            public ActionResult Create(IdentityRole role)
            {
                if (ModelState.IsValid)
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(role);

            }

            // GET: Roles/Edit/5
            public ActionResult Edit(string id)
            {
                var role = db.Roles.Find(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }

            // POST: Roles/Edit/5
            [HttpPost]
            public ActionResult Edit([Bind(Include = "Id,Name")]IdentityRole role)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(role).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(role);
            }

            // GET: Roles/Delete/5
            public ActionResult Delete(string id)
            {

                var role = db.Roles.Find(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }

            // POST: Roles/Delete/5
            [HttpPost]
            public ActionResult Delete(IdentityRole role)
            {
                // TODO: Add delete logic here
                var myRole = db.Roles.Find(role.Id);
                db.Roles.Remove(myRole);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

        public ActionResult UsersWithRoles()
        {
            var usersWithRoles = (from user in db.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      LockoutEnabled = user.LockoutEnabled,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in db.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new Users_in_Role_ViewModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                      LockoutEnabled = p.LockoutEnabled,
                                      Role = string.Join(",", p.RoleNames)
                                  });


            return View(usersWithRoles);
        }

        public ActionResult BlockUser(string userId, bool IsActive)
        {
            try
            {
                var store = new UserStore<ApplicationUser>(db);
                var manager = new ApplicationUserManager(store);
                var lockoutEndDate = new DateTime(2999, 01, 01);
                //manager.SetLockoutEnabled(userId, IsActive);
                //manager.SetLockoutEndDate(userId, lockoutEndDate);
                //db.SaveChanges();
                var user = manager.Users.FirstOrDefault(u => u.Id == userId);
                user.LockoutEnabled = IsActive;
                user.LockoutEndDateUtc = lockoutEndDate;
                manager.Update(user);
                return RedirectToAction("UsersWithRoles", "Roles", new { area = "", });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}




