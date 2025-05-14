using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Quiz_App.Data;
using Online_Quiz_App.Models;

namespace Online_Quiz_App.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Home()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateQuizViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateQuizViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create a new Quiz entity
            var quiz = new Quiz
            {
                Title = model.Title,
                Questions = model.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    CorrectAnswer = q.CorrectAnswer,
                    Options = q.Options.Select(o => new Option
                    {
                        Text = o
                    }).ToList()
                }).ToList()
            };

            // Add the quiz to the database
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuiz(int QuizId, string Title)
        {
            var quiz = await _context.Quizzes.FindAsync(QuizId);
            if (quiz == null)
            {
                return NotFound();
            }

            quiz.Title = Title;
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }



        //add question in modal
        [HttpPost]
        public async Task<IActionResult> AddQuestion(int QuizId, string Text, string CorrectAnswer, List<string> Options)
        {
            var quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.QuizId == QuizId);
            if (quiz == null)
            {
                return NotFound();
            }

            // Create a new question
            var question = new Question
            {
                Text = Text,
                CorrectAnswer = CorrectAnswer,
                Options = Options.Select(o => new Option { Text = o }).ToList(),
                QuizId = QuizId
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }




        [HttpPost]
        public async Task<IActionResult> UpdateQuestionWithOptions(int QuestionId, string Text, string CorrectAnswer, List<string> Options)
        {
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.QuestionId == QuestionId);
            if (question == null)
            {
                return NotFound();
            }

            // Update question text and correct answer
            question.Text = Text;
            question.CorrectAnswer = CorrectAnswer;

            // Remove existing options
            _context.Options.RemoveRange(question.Options);

            // Add updated options
            question.Options = Options.Select(o => new Option { Text = o }).ToList();

            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }





        [HttpPost]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }




        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(SubmitQuizViewModel model)
        {

            // Fetch the quiz with its questions and options
            var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.QuizId == model.QuizId);


            if (quiz == null)
            {
                Console.WriteLine($"Quiz with ID {model.QuizId} not found.");
                return NotFound();
            }
            if (model.Answers == null)
            {
                model.Answers = new Dictionary<int, string>();
            }



            // Calculate the total number of questions and correct answers using LINQ
            int totalQuestions = quiz.Questions.Count;
            int correctAnswers = quiz.Questions.Count(question =>
                model.Answers.TryGetValue(question.QuestionId, out var selectedAnswer) &&
                selectedAnswer == question.CorrectAnswer);



            // Calculate the score
            int unanswered = totalQuestions - model.Answers.Count;

            // Redirect to the QuizResult action with the calculated data
            return RedirectToAction("QuizResult", new { quizId = model.QuizId, correctAnswers, totalQuestions, unanswered });

        }



        public IActionResult QuizResult(int quizId, int correctAnswers, int totalQuestions, int unanswered)
        {
            var score = (double)correctAnswers / totalQuestions * 100;

            ViewBag.Score = score;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.Unanswered = unanswered;

            return View();
        }


        //Quiz suffle

        private static void Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }




        public async Task<IActionResult> QuizDetails(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
            {
                return NotFound();
            }

            // Shuffle questions
            var questions = quiz.Questions.ToList();
            Shuffle(questions);
            quiz.Questions = questions;

            // Shuffle options for each question
            foreach (var question in quiz.Questions)
            {
                var options = question.Options.ToList();
                Shuffle(options);
                question.Options = options;
            }

            return View(quiz);
        }



        public async Task<IActionResult> Admin()
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .ToListAsync();

            return View(quizzes);
        }

    }
}
