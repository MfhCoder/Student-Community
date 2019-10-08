using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.ViewModels
{
    public class UsersInCourseVM
    {
        public int EnrollmentId { get; set; }
        public bool IsActive { get; set; }
        public UserVM Users { get; set; }
        public CourseVM Courses { get; set; }

    }

    public class UsersInTeamsVM
    {
        public int StudentsInTeamsId { get; set; }
        public UserVM Users { get; set; }
        public TeamsVM Teams { get; set; }

    }

    public class TeamsVM
    {
        public int TeamId { get; set; }
        //public string TeamName { get; set; }


    }

    public class Users_in_Role_ViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}