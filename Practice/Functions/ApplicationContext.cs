using System.Data.Entity;
using Practice.Models;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System;

namespace Practice
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ApplicationContext : DbContext
    {
        // Используем конструктор с явной строкой подключения
        public ApplicationContext() : base(CreateConnection(), true)
        {
            Database.SetInitializer<ApplicationContext>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;

            // Логирование SQL запросов
            Database.Log = sql => System.Diagnostics.Debug.WriteLine(sql);
        }

        private static MySqlConnection CreateConnection()
        {
            try
            {
                string connectionString = DatabaseManager.ConnectionString;
                Console.WriteLine($"Создаем подключение к: {GetConnectionInfo(connectionString)}");

                var connection = new MySqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания подключения: {ex.Message}", ex);
            }
        }

        private static string GetConnectionInfo(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return $"Server={builder.Server}, Database={builder.Database}, User={builder.UserID}";
            }
            catch
            {
                return connectionString;
            }
        }

        // Все таблицы БД
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order_Product> Order_Products { get; set; }
        public DbSet<Category_Product> Category_Products { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Accounting> Accountings { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order_Employee> Order_Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Отключаем множественное число для названий таблиц
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            // Специальная конфигурация для столбцов с скобками
            modelBuilder.Entity<Order>()
                .Property(o => o.IdUserClient)
                .HasColumnName("`IdUser(Client)`");

            modelBuilder.Entity<Delivery>()
                .Property(d => d.IdUserClient)
                .HasColumnName("`IdUser(Client)`");

            modelBuilder.Entity<Delivery>()
                .Property(d => d.IdUserEmployee)
                .HasColumnName("`IdUser(Employee)`");

            // Конфигурация связей
            modelBuilder.Entity<User>()
                .HasMany(u => u.OrdersAsClient)
                .WithRequired(o => o.UserClient)
                .HasForeignKey(o => o.IdUserClient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Order_Products)
                .WithRequired(op => op.Order)
                .HasForeignKey(op => op.IdOrder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Order_Products)
                .WithRequired(op => op.Product)
                .HasForeignKey(op => op.IdProduct)
                .WillCascadeOnDelete(false);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при освобождении контекста: {ex.Message}");
            }
        }
    }
}