using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.ViewModels
{
    public class PostsVM
    {
        public int PostID { get; set; }
        public string Message { get; set; }
        public bool IsSolved { get; set; }
        public int Likes { get; set; }
        public DateTime PostedDate { get; set; }
        public UserVM Users { get; set; }
        public CourseVM Courses { get; set; }

    }

    public class CommentsVM
    {
        public int ComID { get; set; }
        public string CommentMsg { get; set; }
        public DateTime CommentedDate { get; set; }
        //public int Likes { get; set; }
        public PostsVM Posts { get; set; }
        public UserVM Users { get; set; }
    }

    public class ConversationsVM
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string FromId { get; set; }
        public string ToId { get; set; }
        public DateTime Date { get; set; }
        public UserVM Users { get; set; }
        public CourseVM Courses { get; set; }

    }

    public class MessagesVM
    {
        public int ID { get; set; }
        public string Msg { get; set; }
        public DateTime Date { get; set; }
        public UserVM Users { get; set; }
        public ConversationsVM Conversations { get; set; }
    }

    public class PostedDateVM
    {
        public int ComID { get; set; }
        public string CommentMsg { get; set; }
        public DateTime CommentedDate { get; set; }
        public PostsVM Posts { get; set; }
        public UserVM Users { get; set; }
    }

    public class UserVM
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string imageProfile { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int StudentId { get; set; }

    }
    public class CourseVM
    {
        public int CoursesId { get; set; }

    }
    public class SubCommentsVM
    {
        public int SubComID { get; set; }
        public string CommentMsg { get; set; }
        public DateTime CommentedDate { get; set; }
        public CommentsVM Comment { get; set; }
        public UserVM User { get; set; }
    }


}