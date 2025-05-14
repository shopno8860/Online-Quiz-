using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Quiz_App.Models
{
    public class Option
    {
        [Key]
        public int OptionId { get; set; }

        [Required]
        public string Text { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}

