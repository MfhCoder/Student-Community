using Microsoft.AspNet.Identity;
using StudentsCommunity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //var UserId = User.Identity.GetUserId();
            //List <Courses> MyCourses = db.Courses.Where(x => x.DoctorId== UserId).ToList();
            //Session["Courses"] = MyCourses;
            var UserId = User.Identity.GetUserId();
            List<Courses> MyCourses = db.Courses.Where(x => x.DoctorId == UserId).ToList();
            List<int> MyRCoursesId = db.Enrollment.Where(d => d.StudentId == UserId && d.IsActive==true).Select(d => d.CoursesId).ToList();
            List<Courses> MyRCourses = new List<Courses>();
            foreach (var CoursesId in MyRCoursesId)
                {
                //MyRCourses = db.Courses.Where(x => x.Id == CoursesId).ToList();
                MyRCourses.Add(db.Courses.Where(x => x.Id == CoursesId).FirstOrDefault());
                }


            if (User.IsInRole("Instructors"))
            { Session["Courses"] = MyCourses; }
            else
            {
                Session["Courses"] = MyRCourses;
            }
            var Requests = db.TeamRequests.Where(x => x.ToId == UserId && x.Accept == false).ToList();
            var IsRead = db.Conversation.Where(x => x.ToId == UserId && x.IsRead == false).ToList();
            Session["Requests"] = Requests;
            Session["IsRead"] = IsRead;
            return View();
        }
        //public JsonResult GetNotificationContacts()
        //{
        //    var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
        //    NotificationComponent NC = new NotificationComponent();
        //    var list = NC.GetContacts(notificationRegisterTime);
        //    //update session here for get only new added contacts (notification)
        //    Session["LastUpdate"] = DateTime.Now;
        //    return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Contact(ContactModel contact)
        {
            var mail = new MailMessage();
            var loginInfo = new NetworkCredential("mohamedfathi853.mf@gmail.com", "PassWord");
            mail.From = new MailAddress(contact.Email);
            mail.To.Add(new MailAddress("mohamedfathi853.mf@gmail.com"));
            mail.Subject = contact.Subject;
            mail.IsBodyHtml = true;
            string body = "Name:" + contact.Name + "<br>" +
                            "Email: " + contact.Email + "<br>" +
                            "Subject: " + contact.Subject + "<br>" +
                            "Message: <b>" + contact.Message + "</b>";
            mail.Body = body;

            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(mail);
            return RedirectToAction("Index");
        }


    }
}