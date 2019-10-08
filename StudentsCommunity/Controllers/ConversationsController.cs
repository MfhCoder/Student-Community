using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentsCommunity.Models;
using StudentsCommunity.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Controllers
{
    public class ConversationsController : Controller
    {
        public ApplicationDbContext dbContext = new ApplicationDbContext();

        public ActionResult AddConversation(int? id,string ToId, int TeamId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddConversation(Conversation Conversations, int id, string ToId,int TeamId)
        {
            var UserId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                 Conversations.FromId = UserId;
                 Conversations.ToId = ToId;
                Conversations.TeamId = TeamId;
                 Conversations.CourseId = id;
                 Conversations.Date = DateTime.Now;
                dbContext. Conversation.Add( Conversations);
                dbContext.SaveChanges();
                return RedirectToAction("GetConversations/" + Conversations.Id);
            }
            return View( Conversations);
        }


        [HttpGet]
        public ActionResult GetAllConversations(int? Id)
        {

            var UserId = User.Identity.GetUserId();

            IQueryable<ConversationsVM>  Conversations = dbContext.Conversation
                                                 .Select(p => new  ConversationsVM
                                                 {
                                                      ID = p.Id,
                                                     Title = p.Title,
                                                      Date = p.Date,
                                                     FromId=p.FromId,
                                                     ToId=p.ToId,
                                                     Users = new UserVM
                                                     {
                                                         UserID = p.FromId,
                                                         Username = p.From.UserName,
                                                         imageProfile = p.From.PersonalInformation.UserImage
                                                     },
                                                     Courses = new CourseVM
                                                     {
                                                         CoursesId = p.CourseId
                                                     }

                                                 }).AsQueryable();

            IQueryable< ConversationsVM>  Conversation =  Conversations.Where(x => x.FromId == UserId || x.ToId== UserId).AsQueryable();
            return View( Conversation);
        }

        [HttpGet]
        public ActionResult GetConversations(int? Id)
        {
            ViewBag.ID = Id;
            //Make Conversation as read
            Conversation IsRead = dbContext.Conversation.SingleOrDefault(x => x.Id == Id);
            if(IsRead.ToId == User.Identity.GetUserId()) {
                IsRead.IsRead = true;
                dbContext.Conversation.Add(IsRead);
                dbContext.Entry(IsRead).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            IQueryable< ConversationsVM> Conversations = dbContext. Conversation
                                                 .Select(p => new  ConversationsVM
                                                 {
                                                      ID = p.Id,
                                                     Title = p.Title,
                                                      Date = p.Date,
                                                     Users = new UserVM
                                                     {
                                                         UserID = p.FromId,
                                                         Username = p.From.UserName,
                                                         imageProfile = p.From.PersonalInformation.UserImage
                                                     },
                                                     Courses = new CourseVM
                                                     {
                                                         CoursesId = p.CourseId
                                                     }

                                                 }).Where(x => x.ID == Id).AsQueryable();

            IQueryable< ConversationsVM>  Conversation =  Conversations.Where(x => x.ID == Id).AsQueryable();
            return View( Conversation);
        }

        public PartialViewResult GetMessages(int ConversationId)
        {

            IQueryable<MessagesVM> Messages = dbContext.Messages.Where(c => c.Conversation.Id == ConversationId)
                                                       .Select(c => new MessagesVM
                                                       {
                                                           ID = c.Id,
                                                           Date = c.Date,
                                                           Msg = c.Message,
                                                           Users = new UserVM
                                                           {
                                                               UserID = c.UserId,
                                                               Username = c.User.UserName,
                                                               imageProfile = c.User.PersonalInformation.UserImage
                                                           }
                                                       }).AsQueryable();

            return PartialView("~/Views/Shared/_MyMessages.cshtml", Messages);
        }

        [HttpPost]
        public ActionResult AddMessage(MessagesVM Message, int ConversationId)
        {
            var userId = User.Identity.GetUserId();
            //bool result = false;

            var Messages = new Messages();

            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var Conversation = dbContext.Conversation.FirstOrDefault(p => p.Id == ConversationId);

            if (Message != null)
            {

                Messages.Message = Message.Msg;

                Messages.Date = Message.Date;

                if (user != null && Conversation != null)
                {
                    Conversation.Messages.Add(Messages);
                    user.Messages.Add(Messages);

                    dbContext.SaveChanges();
                    //result = true;
                }
                //Once the record is inserted , then notify all the subscribers (Clients)
                MessagesHub.NotifyCurrentUserInformationToAllClients();
            }

            return RedirectToAction("GetMessages", "Conversations", new { ConversationId = ConversationId });
        }

    }
}