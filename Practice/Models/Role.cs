using System.Collections.Generic;

namespace Practice.Models
{
    public class Role
    {
        public int IdRole { get; set; }
        public string RoleName { get; set; }

        // Навигационное свойство
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}