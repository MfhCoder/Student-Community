using StudentsCommunity.Models;
using StudentsCommunity.viewModels;
using StudentsCommunity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Controllers
{
    public class QuizzController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quizz
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateQuiz(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateQuiz(Quiz quiz, int id)
        {
            if (ModelState.IsValid)
            {
                quiz.CourseId = id;
                db.Quizs.Add(quiz);
                db.SaveChanges();
                return RedirectToAction("AddQuestions/" + quiz.QuizId);
            }
            return View();
        }

        public ActionResult AddQuestions(int? id)
        {
            // This is only for show by default one row for insert data to the database
            List<Question> ci = new List<Question> { new Question { QuestionId = 0, QuestionText = ""} };
            return View(ci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestions(List<Question> ci, int id)
        {
            if (ModelState.IsValid)
            {

                foreach (var i in ci)
                {
                    i.QuizId = id;
                    db.Questions.Add(i);
                }
                db.SaveChanges();
                ViewBag.Message = "Data successfully saved!";
                return RedirectToAction("AddChoices/" + id);
            }

            return View(ci);
        }

        public ActionResult AddChoices(int? id)
        {
            ViewBag.QuestionId = new SelectList(db.Questions.Where(x=>x.QuizId== id), "QuestionId", "QuestionText");

            // This is only for show by default one row for insert data to the database
            List<Choice> ci = new List<Choice> { new Choice { ChoiceId = 0, ChoiceText = "" } };
            return View(ci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddChoices(List<Choice> ci,int QuestionId)
        {
            var Question = new Question();

            if (ModelState.IsValid)
            {

                foreach (var i in ci)
                {
                    i.QuestionId = QuestionId;
                    db.Choices.Add(i);
                }
                db.SaveChanges();
                ViewBag.Message = "Data successfully saved!";
                //return RedirectToAction("AddAnswers");
                }
            ViewBag.QuestionId = new SelectList(db.Questions, "QuestionId", "QuestionText");

            return View(ci);
        }

        public ActionResult AddAnswers(int? id)
        {
            ViewBag.QuestionId = new SelectList(db.Questions.Where(x=>x.QuizId==id), "QuestionId", "QuestionText");

            // This is only for show by default one row for insert data to the database
            List<Answer> ci = new List<Answer> { new Answer { AnswerId = 0, AnswerText = "" } };
            return View(ci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAnswers(List<Answer> ci, int QuestionId)
        {
            var Question = new Question();

            if (ModelState.IsValid)
            {

                foreach (var i in ci)
                {
                    i.QuestionId = QuestionId;
                    db.Answers.Add(i);
                }
                db.SaveChanges();
                ViewBag.Message = "Data successfully saved!";
                //return RedirectToAction("SelectQuizz");

            }
            ViewBag.QuestionId = new SelectList(db.Questions, "QuestionId", "QuestionText");

            return View(ci);
        }



        [HttpGet]
        public ActionResult SelectQuizz()
        {
            QuizVM quiz = new viewModels.QuizVM();
            quiz.ListOfQuizz = db.Quizs.Select(q => new SelectListItem
            {
                Text = q.QuizName,
                Value = q.QuizId.ToString()

            }).ToList();

            return View(quiz);
        }

        [HttpPost]
        public ActionResult SelectQuizz(QuizVM quiz)
        {
            QuizVM quizSelected = db.Quizs.Where(q => q.QuizId == quiz.QuizId).Select(q => new QuizVM
            {
                QuizId = q.QuizId,
                QuizName = q.QuizName,

            }).FirstOrDefault();

            if (quizSelected != null)
            {
                Session["SelectedQuiz"] = quizSelected;

                return RedirectToAction("QuizTest");
            }

            return View();
        }

        [HttpGet]
        public ActionResult QuizTest()
        {
            QuizVM quizSelected = Session["SelectedQuiz"] as QuizVM;
            IQueryable<QuestionVM> questions = null;

            if (quizSelected != null)
            {
                questions = db.Questions.Where(q => q.Quiz.QuizId == quizSelected.QuizId)
                   .Select(q => new QuestionVM
                   {
                       QuestionId = q.QuestionId,
                       QuestionText = q.QuestionText,
                       Choices = q.Choices.Select(c => new ChoiceVM
                       {
                           ChoiceId = c.ChoiceId,
                           ChoiceText = c.ChoiceText
                       }).ToList()

                   }).AsQueryable();


            }

            return View(questions);
        }

        [HttpPost]
        public ActionResult QuizTest(List<QuizAnswersVM> resultQuiz)
        {
            List<QuizAnswersVM> finalResultQuiz = new List<viewModels.QuizAnswersVM>();

            foreach (QuizAnswersVM answser in resultQuiz)
            {
                QuizAnswersVM result = db.Answers.Where(a => a.QuestionId == answser.QuestionId).Select(a => new QuizAnswersVM
                {
                    QuestionId = a.QuestionId,
                    AnswerQ = a.AnswerText,
                    isCorrect = (answser.AnswerQ.ToLower().Equals(a.AnswerText.ToLower()))

                }).FirstOrDefault();

                finalResultQuiz.Add(result);
            }

            return Json(new { result = finalResultQuiz }, JsonRequestBehavior.AllowGet);
        }


    }
}