using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Phone {  get; set; }
        public string Email { get; set; }
        public string Birthday { get; set; }
        public string Address { get; set; }
    }
}
