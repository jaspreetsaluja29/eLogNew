using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eLog.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [ForeignKey("UserRole")]
        public int UserRoleId { get; set; }

        // Navigation property to related UserRole (if needed)
        public virtual UserRole UserRole { get; set; }

        public string? JobRank { get; set; }
    }
}
