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

namespace StudentsCommunity.Controllers
{
    public class TeamRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Requests()
        {
            var UserId = User.Identity.GetUserId();
            var Requests = db.TeamRequests.Where(x => x.ToId == UserId && x.Accept == false).ToList();
            var from = Requests.Select(x=>x.FromId);
            ViewBag.From = from;
            //get senders
            //List<string> Froms = new List<string>();
            //List<string> FromsViewBag = new List<string>();
            //foreach (var Request in Requests)
            //{
            //    Froms.Add(Request.FromId);
            //}
            //foreach (var From in Froms)
            //{
            //    FromsViewBag.Add(db.Users.Where(x => x.Id == From).Select(x => x.UserName).FirstOrDefault());
            //}
            ////senders user name
            //ViewBag.From = FromsViewBag;
            return View(Requests);
        }

        public ActionResult AcceptRequest(int id)
        {
               var UserId = User.Identity.GetUserId();

                StudentsInTeams StudentsInTeams = new StudentsInTeams();
                TeamRequests Request = db.TeamRequests.SingleOrDefault(x => x.Id == id);
                StudentsInTeams.TeamId = Request.TeamId;
                StudentsInTeams.StudentId = UserId;
                Request.Accept = true;
                db.StudentsInTeams.Add(StudentsInTeams);
                db.SaveChanges();
                return RedirectToAction("UsersInTeams/" + Request.TeamId, "Teams");

        }

        public ActionResult RefuseRequest(int id)
        {

            TeamRequests teamRequests = db.TeamRequests.Find(id);
            db.TeamRequests.Remove(teamRequests);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

        // GET: TeamRequests
        public ActionResult Index()
        {
            return View(db.TeamRequests.ToList());
        }

        // GET: TeamRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamRequests teamRequests = db.TeamRequests.Find(id);
            if (teamRequests == null)
            {
                return HttpNotFound();
            }
            return View(teamRequests);
        }

        // GET: TeamRequests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TeamRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,FromId,ToId,TeamId")] TeamRequests teamRequests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamRequests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teamRequests);
        }

        // GET: TeamRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamRequests teamRequests = db.TeamRequests.Find(id);
            if (teamRequests == null)
            {
                return HttpNotFound();
            }
            return View(teamRequests);
        }

        // POST: TeamRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,FromId,ToId,TeamId")] TeamRequests teamRequests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamRequests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamRequests);
        }

        // GET: TeamRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamRequests teamRequests = db.TeamRequests.Find(id);
            if (teamRequests == null)
            {
                return HttpNotFound();
            }
            return View(teamRequests);
        }

        // POST: TeamRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeamRequests teamRequests = db.TeamRequests.Find(id);
            db.TeamRequests.Remove(teamRequests);
            db.SaveChanges();
            return RedirectToAction("Index");
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
