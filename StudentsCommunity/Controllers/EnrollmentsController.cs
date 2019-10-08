using Microsoft.AspNet.Identity;
using StudentsCommunity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Controllers
{   [Authorize]
    public class EnrollmentsController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();



        [HttpGet]
        public ActionResult GetCourse()
        {
            ViewBag.CoursesId = new SelectList(db.Courses, "Id", "CourseName");
            return View();
        }

        [HttpPost]
        public ActionResult GetCourse(string CourseCode,int CoursesId)
        {
            Courses Course = db.Courses.Where(u => u.CourseCode.ToLower() == CourseCode.ToLower()&& u.Id == CoursesId)
                                 .FirstOrDefault();
            var UserId = User.Identity.GetUserId();
            List<Enrollment> MyCourses = db.Enrollment.Where(x => x.StudentId == UserId).ToList();
            bool IsCourseRegistered = false;
            foreach (var item in MyCourses)
            {
                if (item.Courses.CourseCode== CourseCode)
                {
                    IsCourseRegistered = true;
                }
            }

            var enrollment = new Enrollment();
            if (Course != null)
            {
               if (IsCourseRegistered == false)
                {
                        enrollment.StudentId = UserId;
                        enrollment.CoursesId = Course.Id;
                        db.Enrollment.Add(enrollment);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    ModelState.AddModelError("IsCourseRegistered", "you already Registered this Course");
                }
            }
            else
            {
                ModelState.AddModelError("CourseCode", "CourseCode Dosn't Exists");
            }

            ViewBag.CoursesId = new SelectList(db.Courses, "Id", "CourseName");
            return View();
        }

        public ActionResult MyCourses()
        {
            var UserId = User.Identity.GetUserId();
            var Enrollments = db.Enrollment.Where(a => a.StudentId == UserId);
            return View(Enrollments.ToList());
        }
        // GET: Enrollments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get enrollment Id
            int EnrollmentId = db.Enrollment.Where(x => x.CoursesId == id).FirstOrDefault().Id;

            Enrollment Enrollments = db.Enrollment.Find(EnrollmentId);
            if (Enrollments == null)
            {
                return HttpNotFound();
            }
            return View(Enrollments);
        }
        // GET: Enrollments/Create

        // GET: Enrollments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int EnrollmentId = db.Enrollment.Where(x => x.CoursesId == id).FirstOrDefault().Id;

            Enrollment Enrollment = db.Enrollment.Find(EnrollmentId);
            if (Enrollment == null)
            {
                return HttpNotFound();
            }
            return View(Enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int EnrollmentId = db.Enrollment.Where(x => x.CoursesId == id).FirstOrDefault().Id;

            Enrollment Enrollment = db.Enrollment.Find(EnrollmentId);
            db.Enrollment.Remove(Enrollment);
            db.SaveChanges();
            return RedirectToAction("MyCourses");
        }
    }
}
