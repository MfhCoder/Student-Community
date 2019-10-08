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
using StudentsCommunity.ViewModels;

namespace StudentsCommunity.Controllers
{
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Teams(int? id)
        {
            var Teams = db.Teams.Where(x=>x.CourseId==id).ToList();

            return View(Teams);
        }

        // GET: Teams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        public ActionResult Create(int? id)
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Team Teams,int id)
        {
            var UserId = User.Identity.GetUserId();

            StudentsInTeams StudentsInTeams = new StudentsInTeams();
            if (ModelState.IsValid)
            {
                Teams.StudentId = UserId;
                Teams.CourseId = id;
                StudentsInTeams.StudentId = UserId;
                StudentsInTeams.TeamId = Teams.Id;
                db.Teams.Add(Teams);
                db.StudentsInTeams.Add(StudentsInTeams);
                db.SaveChanges();
                return RedirectToAction("MyTeams/" + id);
            }
            return View(Teams);
        }

        public ActionResult AddToTeam(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToTeam(StudentsInTeams StudentsInTeams, int id,int StudentId)
        {
            var PersonalInformation = db.PersonalInformations.Where(x => x.StudentId == StudentId).FirstOrDefault<PersonalInformation>();

            var UserId = User.Identity.GetUserId();

            bool hasValue;

            if (PersonalInformation == null)
                hasValue = false;
            else
                hasValue = true;
            //Get the course that containing the team
            int CourseId = db.Teams.Where(x => x.Id == id).Select(x => x.CourseId).FirstOrDefault();
            //students registered course
            var students = db.Enrollment.Where(x => x.CoursesId == CourseId).Select(x => x.StudentId).ToList();
            //Teams in the course
            var Teams = db.Teams.Where(x => x.CourseId == CourseId).Select(x => x.Id).ToList();
            //StudentsInTeams in the course
            var StudentsInTeam = db.StudentsInTeams.Where(x => Teams.Contains(x.TeamId)).Select(x => x.StudentId).ToList();
            //Get num of students in the team
            int studentsInTeam = db.StudentsInTeams.Where(x => x.TeamId == id).Count();
            //Get the team limit
            int GroupNum = db.Courses.Where(x => x.Id == CourseId).Select(x => x.GroupNum).FirstOrDefault();
                       //check the team limit
            if (studentsInTeam == GroupNum)
            {
                ModelState.AddModelError("TeamLimit", "this team is complite");
            }
            //check if this student not registered in the course
            else if (!hasValue || !students.Contains(PersonalInformation.UserId))
                {
                
                ModelState.AddModelError("IsStudentRegistered", "this student not registered in the course");
            }
            //check if this student is in team
            else if (StudentsInTeam.Contains(PersonalInformation.UserId))
            {
                ModelState.AddModelError("IsInTeam", "this student is in team");
            }


            TeamRequests TeamRequests = new TeamRequests();

            if (ModelState.IsValid)
            {
                TeamRequests.TeamId = id;
                TeamRequests.FromId = UserId;
                TeamRequests.Date= DateTime.Now;
                TeamRequests.ToId = PersonalInformation.UserId;
                TeamRequests.Accept = false;
                db.TeamRequests.Add(TeamRequests);
                db.SaveChanges();
                return RedirectToAction("MYTeams/"+ id);
            }
            return View(StudentsInTeams);
        }


        public ActionResult LeaveTeam(int? Id)
        {   
            //Get the course that containing the team
            int CourseId = db.Teams.Where(x => x.Id == Id).Select(x => x.CourseId).FirstOrDefault();
            var UserId = User.Identity.GetUserId();
            var id = db.StudentsInTeams.Where(x => x.TeamId == Id && x.StudentId == UserId).Select(x => x.Id).FirstOrDefault();
            StudentsInTeams StudentsInTeams = db.StudentsInTeams.Find(id);
            db.StudentsInTeams.Remove(StudentsInTeams);
            db.SaveChanges();
            return RedirectToAction("MyTeams/" + CourseId);
        }


        public ActionResult UsersNotInTeams(int? Id)
        {
            
            IQueryable<UsersInCourseVM> Users = db.Enrollment
                                                 .Select(p => new UsersInCourseVM
                                                 {
                                                     EnrollmentId = p.Id,
                                                     Users = new UserVM
                                                     {
                                                         UserID = p.StudentId,
                                                         Username = p.User.UserName,
                                                         imageProfile = p.User.PersonalInformation.UserImage,
                                                         Email=p.User.Email,
                                                         Phone=p.User.PhoneNumber,
                                                         StudentId=p.User.PersonalInformation.StudentId,
                                                     },
                                                     Courses = new CourseVM
                                                     {
                                                         CoursesId = p.CoursesId
                                                     }

                                                 }).AsQueryable();
            //select students registered course
            IQueryable<UsersInCourseVM> User = Users.Where(x => x.Courses.CoursesId == Id).AsQueryable();

            //select students Not In Teams
            IQueryable<UsersInCourseVM> UsersNotInTeams = User.Where(user => !db.StudentsInTeams.Any(f => f.StudentId == user.Users.UserID)).AsQueryable();

            return View(UsersNotInTeams);

        }
        public ActionResult UsersInTeams(int? Id)
        {
            IQueryable<UsersInTeamsVM> Users = db.StudentsInTeams
                                                 .Select(p => new UsersInTeamsVM
                                                 {
                                                     StudentsInTeamsId = p.Id,
                                                     Users = new UserVM
                                                     {
                                                         UserID = p.StudentId,
                                                         Username = p.Student.UserName,
                                                         imageProfile = p.Student.PersonalInformation.UserImage,
                                                         Email = p.Student.Email,
                                                         Phone = p.Student.PhoneNumber,
                                                     },
                                                     Teams = new TeamsVM
                                                     {
                                                         TeamId = p.TeamId
                                                         //TeamName = p.Team.Name
                                                     }

                                                 }).AsQueryable();
            ViewBag.Converstion = db.Conversation.Where(x => x.TeamId == Id);
            IQueryable<UsersInTeamsVM> User = Users.Where(x => x.Teams.TeamId == Id).AsQueryable();
            return View(User);

        }



        public ActionResult MYTeams()
        {
            var UserId = User.Identity.GetUserId();

            //var Teams = db.Teams.Where(x=>x.StudentId==UserId).ToList();
            var Teams = db.StudentsInTeams.Where(x => x.StudentId == UserId).ToList();

            return View(Teams);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "CourseName", team.CourseId);
            ViewBag.StudentId = new SelectList(db.Users, "Id", "UserType", team.StudentId);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CourseId,StudentId")] Team team)
        {
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "CourseName", team.CourseId);
            ViewBag.StudentId = new SelectList(db.Users, "Id", "UserType", team.StudentId);
            return View(team);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Get the course that containing the team
            int CourseId = db.Teams.Where(x => x.Id == id).Select(x => x.CourseId).FirstOrDefault();
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("MyTeams/"+CourseId);
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
