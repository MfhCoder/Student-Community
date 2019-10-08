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
using System.IO;
using StudentsCommunity.ViewModels;

namespace StudentsCommunity.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult MyCourses()
        {
            var UserId = User.Identity.GetUserId();
            var courses = db.Courses.Where(a => a.DoctorId == UserId);

            return View(courses.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = db.Courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            var UserId = User.Identity.GetUserId();

             int TotalPosts = db.Posts.Where(a => a.CourseId == id).Count();
            ViewBag.TotalPosts = TotalPosts;

            int SolvedProblems = db.Posts.Where(a => a.CourseId == id && a.IsSolved ==true).Count();

            int UnSolvedProblems = TotalPosts - SolvedProblems;

            TempData["SolvedProblems"] = SolvedProblems;

            TempData["UnSolvedProblems"] = UnSolvedProblems;

            string InstructorID = db.Courses.Where(a => a.Id == id).FirstOrDefault().DoctorId;

            //All Comments
            int TotalContributions = db.Comments.Where(a => a.Post.CourseId == id).Count();
            ViewBag.TotalContributions = TotalContributions;

            //instructors Comments
            int ITotalContributions = db.Comments.Where(a => a.Post.CourseId == id && a.UserId == InstructorID).Count();
            ViewBag.ITotalContributions = ITotalContributions;

            //Students Comments
            int STotalContributions = TotalContributions - ITotalContributions;
            
            int TotalTeams = db.Teams.Where(a => a.CourseId == id).Count();
            ViewBag.TotalTeams = TotalTeams;

            int TotalStudents = db.Enrollment.Where(a => a.CoursesId == id).Count();
            ViewBag.TotalStudents = TotalStudents;
            TempData["ITotalContributions"] = ITotalContributions;

            TempData["STotalContributions"] = STotalContributions;
            //int IResponse = db.Comments.Where(a => a.UserId == UserId).Count();
            //ViewBag.TotalPosts = TotalPosts;

            return View(courses);
        }


        public JsonResult GetPieRenderer1()
        {
            int a = Convert.ToInt32(TempData["ITotalContributions"]);
            int b = Convert.ToInt32(TempData["STotalContributions"]);

            var y = new[] {
                            new { Response_NAME = "Instructors Response", Response_Number = a },
                            new { Response_NAME = "Students Response", Response_Number = b }
                        };

            return Json(y, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPieRenderer2()
        {
            int a = Convert.ToInt32(TempData["SolvedProblems"]);
            int b = Convert.ToInt32(TempData["UnSolvedProblems"]);

            var y = new[] {
                            new { Response_NAME = "Solved Problems", Response_Number = a },
                            new { Response_NAME = "UnSolved Problems", Response_Number = b }
                        };

            return Json(y, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCourse()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(Courses courses)
        {
            var UserId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                courses.DoctorId = UserId;
                db.Courses.Add(courses);
                db.SaveChanges();
                return RedirectToAction("MyCourses");
            }
            return View(courses);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = db.Courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Courses courses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyCourses");
            }
            return View(courses);
        }

       public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = db.Courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Courses courses = db.Courses.Find(id);
            db.Courses.Remove(courses);
            db.SaveChanges();
            return RedirectToAction("MyCourses");
        }
        public ActionResult Upload(int? id)
        {
            ViewBag.CourseId = id;
            return View();
        }

        [HttpPost]
        public ActionResult Upload(int Id)
        {
            Courses course = db.Courses.Find(Id);

            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    var file = HttpContext.Request.Files["files" + i];

                    //make Folder Name Variable
                    var ImagesPath = course.CourseName;

                    string subPath = "/Files/" + ImagesPath;
                    //CraetePath if note exists 
                    bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

                    if (file != null)
                    {
                        var fileSavePath = Path.Combine(Server.MapPath(subPath), file.FileName);
                        file.SaveAs(fileSavePath);
                       

                    }
                }
                return RedirectToAction("Download/" + Id);
            }
            return View();
        }

        public ActionResult Download(int? id)
        {
            ViewBag.CourseId = id;
            Courses course = db.Courses.Find(id);
            //make Folder Name Variable
            var ImagesPath = course.CourseName;


            string subPath = "/Files/" + ImagesPath;
            string[] files = Directory.GetFiles(Server.MapPath(subPath));
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            ViewBag.Files = files;
            return View();
        }

        public FileResult DownloadFile(string fileName, int Id)
        {
            Courses course = db.Courses.Find(Id);
            //make Folder Name Variable
            var ImagesPath = course.CourseName;

            string subPath = "/Files/" + ImagesPath;

            var filepath = System.IO.Path.Combine(Server.MapPath(subPath), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }

        public ActionResult UsersInCourse(int? Id)
        {
            IQueryable<UsersInCourseVM> Users = db.Enrollment
                                                 .Select(p => new UsersInCourseVM
                                                 {
                                                     EnrollmentId = p.Id,
                                                     IsActive =p.IsActive,
                                                     Users = new UserVM
                                                     {
                                                         UserID = p.StudentId,
                                                         Username = p.User.UserName,
                                                         imageProfile = p.User.PersonalInformation.UserImage,
                                                         Email=p.User.Email,
                                                         Phone=p.User.PhoneNumber
                                                     },
                                                     Courses = new CourseVM
                                                     {
                                                         CoursesId = p.CoursesId
                                                     }

                                                 }).AsQueryable();

            IQueryable<UsersInCourseVM> User = Users.Where(x => x.Courses.CoursesId == Id).AsQueryable();
            return View(User);

        }


        public ActionResult BlockStudent(string StudenId,int CourseId,bool IsActive)
        {
            try
            {
                Enrollment Stu = db.Enrollment.SingleOrDefault(x => x.StudentId == StudenId && x.CoursesId==CourseId);

                Stu.IsActive = IsActive;
                db.SaveChanges();
                return RedirectToAction("UsersInCourse/"+CourseId);
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    



    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
