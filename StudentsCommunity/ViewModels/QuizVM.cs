using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.viewModels
{

    public class QuizVM
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public List<SelectListItem> ListOfQuizz { get; set; }

    }

    public class QuestionVM
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string Anwser { get; set; }
        public int QuizId { get; set; }
        public string ChoiceText { get; set; }
        
        public ICollection<ChoiceVM> Choices { get; set; }
        public QuizVM Quizs { get; set; }
    }

    public class ChoiceVM
    {
        public int ChoiceId { get; set; }
        public string ChoiceText { get; set; }
        public QuestionVM Question { get; set; }

    }

    public class QuizAnswersVM
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerQ { get; set; }
        public bool isCorrect { get; set; }


    }
}