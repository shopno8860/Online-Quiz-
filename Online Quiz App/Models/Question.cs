using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Online_Quiz_App.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        public ICollection<Option> Options { get; set; }

        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}

