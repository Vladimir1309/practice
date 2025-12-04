using Practice.Models;
using System;

namespace Practice
{
    public static class TestUserFactory
    {
        public static User CreateAdmin()
        {
            return new User
            {
                IdUser = 1,
                Login = "admin",
                Password = "admin",
                FirstName = "Администратор",
                LastName = "Системный",
                Patronymic = "",
                Phone = "00000000000",
                Email = "admin@system.local",
                Birthday = new DateTime(1990, 1, 1),
                Address = "Системный адрес",
                IdPost = 4,
                Post = new Post
                {
                    IdPost = 4,
                    PostName = "Администратор",
                    IdRole = 2,
                    Role = new Role
                    {
                        IdRole = 2,
                        RoleName = "Администратор"
                    }
                }
            };
        }

        public static User CreateClient()
        {
            return new User
            {
                IdUser = 2,
                Login = "user",
                Password = "user",
                FirstName = "Иван",
                LastName = "Иванов",
                Patronymic = "Иванович",
                Phone = "89991234567",
                Email = "user@example.com",
                Birthday = new DateTime(1995, 5, 15),
                Address = "ул. Пользовательская, д. 1",
                IdPost = 5,
                Post = new Post
                {
                    IdPost = 5,
                    PostName = "Покупатель",
                    IdRole = 1,
                    Role = new Role
                    {
                        IdRole = 1,
                        RoleName = "Пользователь"
                    }
                }
            };
        }

        public static User CreateSales()
        {
            return new User
            {
                IdUser = 3,
                Login = "sales",
                Password = "sales",
                FirstName = "Петр",
                LastName = "Петров",
                Patronymic = "Петрович",
                Phone = "89997654321",
                Email = "sales@example.com",
                Birthday = new DateTime(1988, 8, 25),
                Address = "ул. Продажная, д. 2",
                IdPost = 1,
                Post = new Post
                {
                    IdPost = 1,
                    PostName = "Продавец-консультант",
                    IdRole = 3,
                    Role = new Role
                    {
                        IdRole = 3,
                        RoleName = "Продавец-консультант"
                    }
                }
            };
        }

        public static User CreateDelivery()
        {
            return new User
            {
                IdUser = 4,
                Login = "delivery",
                Password = "delivery",
                FirstName = "Сергей",
                LastName = "Сергеев",
                Patronymic = "Сергеевич",
                Phone = "89998887766",
                Email = "delivery@example.com",
                Birthday = new DateTime(1992, 3, 10),
                Address = "ул. Доставки, д. 3",
                IdPost = 3,
                Post = new Post
                {
                    IdPost = 3,
                    PostName = "Курьер",
                    IdRole = 4,
                    Role = new Role
                    {
                        IdRole = 4,
                        RoleName = "Курьер"
                    }
                }
            };
        }

        // Метод для создания пользователя из реальной БД (если доступна)
        public static User CreateFromRealData(string login, string role = "Пользователь")
        {
            int roleId = role switch
            {
                "Администратор" => 2,
                "Продавец-консультант" => 3,
                "Курьер" => 4,
                _ => 1 // Пользователь
            };

            int postId = role switch
            {
                "Администратор" => 4,
                "Продавец-консультант" => 1,
                "Курьер" => 3,
                _ => 5 // Покупатель
            };

            return new User
            {
                IdUser = 100, // Временный ID
                Login = login,
                Password = "password",
                FirstName = "Тестовый",
                LastName = "Пользователь",
                Phone = "80000000000",
                Email = $"{login}@test.local",
                Birthday = DateTime.Now.AddYears(-25),
                Address = "Тестовый адрес",
                IdPost = postId,
                Post = new Post
                {
                    IdPost = postId,
                    PostName = role,
                    IdRole = roleId,
                    Role = new Role
                    {
                        IdRole = roleId,
                        RoleName = role
                    }
                }
            };
        }
    }
}