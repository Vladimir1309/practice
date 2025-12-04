using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("post")]
    public class Post
    {
        [Key]
        [Column("IdPost")]
        public int IdPost { get; set; }

        [Column("IdRole")]
        public int IdRole { get; set; }

        [Column("Post")]
        [StringLength(255)]
        public string PostName { get; set; }

        // Навигационные свойства
        [ForeignKey("IdRole")]
        public virtual Role Role { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public Post()
        {
            Users = new HashSet<User>();
        }
    }
}