using System;
using System.ComponentModel.DataAnnotations;

namespace Online_Quiz_App.Models
{
    //public class User
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(100)]
    //    public string FullName { get; set; }

    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    [Required]
    //    public string Password { get; set; }

    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //}

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Add Role property

        public DateTime CreatedAt { get; set; }
    }


}
