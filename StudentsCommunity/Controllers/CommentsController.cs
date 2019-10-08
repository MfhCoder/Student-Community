using Microsoft.AspNet.Identity;
using StudentsCommunity.Models;
using StudentsCommunity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        public ApplicationDbContext dbContext = new ApplicationDbContext();


        public ActionResult AddPost(int? id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPost(Posts posts, int id )
        {
            var UserId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                posts.UserId = UserId;
                posts.CourseId = id;
                posts.PostedDate = DateTime.Now;
                dbContext.Posts.Add(posts);
                dbContext.SaveChanges();
                return RedirectToAction("GetPosts/"+ posts.Id);
            }
            return View(posts);
        }


        [HttpGet]
        public ActionResult GetAllPosts(int? Id)
        {

            IQueryable<PostsVM> Posts = dbContext.Posts
                                                 .Select(p => new PostsVM
                                                 {
                                                     PostID = p.Id,
                                                     Message = p.Message,
                                                     PostedDate = p.PostedDate,
                                                     IsSolved = p.IsSolved,
                                                     Likes = p.Likes.Count(),
                                                     Users = new UserVM
                                                      {
                                                          UserID = p.UserId,
                                                          Username = p.User.UserName,
                                                          imageProfile = p.User.PersonalInformation.UserImage,
                                                     },
                                                     Courses = new CourseVM
                                                     {
                                                         CoursesId=p.CourseId
                                                     }

                                                 }).AsQueryable();

            IQueryable<PostsVM> Post =Posts.Where(x =>x.Courses.CoursesId== Id).AsQueryable();
            return View(Post);
        }

        [HttpGet]
        public ActionResult GetPosts(int? Id)
        {
            ViewBag.ID = Id;
            IQueryable<PostsVM> Posts = dbContext.Posts
                                                 .Select(p => new PostsVM
                                                 {
                                                     PostID = p.Id,
                                                     Message = p.Message,
                                                     PostedDate = p.PostedDate,
                                                     IsSolved=p.IsSolved,
                                                     Likes = p.Likes.Count(),
                                                     Users = new UserVM
                                                     {
                                                         UserID = p.UserId,
                                                         Username = p.User.UserName,
                                                         imageProfile = p.User.PersonalInformation.UserImage
                                                     },
                                                     Courses = new CourseVM
                                                     {
                                                         CoursesId = p.CourseId
                                                     }

                                                 }).Where(x => x.PostID == Id).AsQueryable();

            IQueryable<PostsVM> Post = Posts.Where(x => x.PostID == Id).AsQueryable();
            return View(Post);
        }
        public PartialViewResult GetComments(int postId)
        {

            IQueryable<CommentsVM> comments = dbContext.Comments.Where(c => c.Post.Id == postId)
                                                       .Select(c => new CommentsVM
                                                       {
                                                           ComID = c.Id,
                                                           CommentedDate = c.CommentedDate,
                                                           CommentMsg = c.CommentMsg,
                                                           //Likes=c.Likes.Count(),
                                                           Users = new UserVM
                                                           {
                                                               UserID = c.UserId,
                                                               Username = c.User.UserName,
                                                               imageProfile = c.User.PersonalInformation.UserImage
                                                           }
                                                       }).AsQueryable();

            return PartialView("~/Views/Shared/_MyComments.cshtml", comments);
        }

        [HttpPost]
        public ActionResult AddComment(CommentsVM comment, int postId)
        {
            var userId = User.Identity.GetUserId();
            //bool result = false;

            var Comments = new Comments();

            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var post = dbContext.Posts.FirstOrDefault(p => p.Id == postId);

            if (comment != null)
            {

                Comments.CommentMsg = comment.CommentMsg;

                Comments.CommentedDate = comment.CommentedDate;            

                if (user != null && post != null)
                {
                    post.Comment.Add(Comments);
                    user.Comments.Add(Comments);

                    dbContext.SaveChanges();
                    //result = true;
                }
                //Once the record is inserted , then notify all the subscribers (Clients)
                CommentsHub.NotifyCurrentUserInformationToAllClients();
            }

            return RedirectToAction("GetComments", "Comments", new { postId = postId });
        }

        [HttpGet]
        public PartialViewResult GetSubComments(int ComID)
        {
            IQueryable<SubCommentsVM> subComments = dbContext.SubComments.Where(sc => sc.Comment.Id == ComID)
                                                              .Select(sc => new SubCommentsVM
                                                              {
                                                                  SubComID = sc.Id,
                                                                  CommentMsg = sc.CommentMsg,
                                                                  CommentedDate = sc.CommentedDate,
                                                                  User = new UserVM
                                                                  {
                                                                      UserID = sc.UserId,
                                                                      Username = sc.User.UserName,
                                                                      imageProfile = sc.User.PersonalInformation.UserImage
                                                                  }
                                                              }).AsQueryable();

            return PartialView("~/Views/Shared/_MySubComments.cshtml", subComments);
        }

        [HttpPost]
        public ActionResult AddSubComment(SubCommentsVM subComment, int ComID)
        {
            var userId = User.Identity.GetUserId();


            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var comment = dbContext.Comments.FirstOrDefault(p => p.Id == ComID);

            var subComments = new SubComments();

            if (subComment != null)
            {
                subComments.CommentMsg = subComment.CommentMsg;
                subComments.CommentedDate = subComment.CommentedDate;
              };


                if (user != null && comment != null)
                {
                    comment.SubComment.Add(subComments);
                    user.SubComments.Add(subComments);

                    dbContext.SaveChanges();
                    //result = true;
                }
         

            return RedirectToAction("GetSubComments", "Comments", new { ComID = ComID });

        }

        public ActionResult IsSolved(int PostId, bool IsSolved)
        {
            try
            {
                Posts Post = dbContext.Posts.SingleOrDefault(x => x.Id == PostId);

                Post.IsSolved = IsSolved;
                dbContext.SaveChanges();
                return RedirectToAction("GetPosts/" + PostId);
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void Likes(int PostID)
        {
            var UserId = User.Identity.GetUserId();

            Likes Likes = new Likes();
            var MyLikes = dbContext.Likes.Where(x => x.PostId == PostID && x.UserId == UserId);
            if (!MyLikes.Any())
            {
                Likes.UserId = UserId;
                Likes.PostId = PostID;
                dbContext.Likes.Add(Likes);
                dbContext.SaveChanges();
            }



        }

    }

}