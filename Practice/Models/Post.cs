using System.Collections.Generic;

namespace Practice.Models
{
    public class Post
    {
        public int IdPost { get; set; }
        public int IdRole { get; set; }
        public string PostName { get; set; }

        // Навигационные свойства
        public virtual Role Role { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}