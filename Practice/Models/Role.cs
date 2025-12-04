using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("role")]
    public class Role
    {
        [Key]
        [Column("IdRole")]
        public int IdRole { get; set; }

        [Column("Role")]
        [StringLength(255)]
        public string RoleName { get; set; }

        // Навигационное свойство
        public virtual ICollection<Post> Posts { get; set; }

        public Role()
        {
            Posts = new HashSet<Post>();
        }
    }
}