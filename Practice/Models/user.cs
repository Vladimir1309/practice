namespace Practice.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public int IdPost { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }

        // Навигационные свойства
        public Post Post { get; set; }
        public Role Role => Post?.Role;
    }
}