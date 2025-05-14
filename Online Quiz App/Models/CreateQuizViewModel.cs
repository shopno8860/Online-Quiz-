using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Online_Quiz_App.Models
{
    public class CreateQuizViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public List<CreateQuestionViewModel> Questions { get; set; } = new List<CreateQuestionViewModel>();
    }

    public class CreateQuestionViewModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        public List<string> Options { get; set; } = new List<string>();
    }
}
