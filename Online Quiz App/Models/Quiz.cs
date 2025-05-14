using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Online_Quiz_App.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
