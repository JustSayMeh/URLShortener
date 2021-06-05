using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class Link
    {
        public int Id { get; set; }
        public string Short { get; set; }
        public string Original { get; set; }
        public Link() { }
        public Link(string link, string shorty)
        {
            this.Original = link;
            this.Short = shorty;
        }
    }
    public class SQLiteDbContext : DbContext
    {
        public DbSet<Link> Links { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "Database.db" };
            string connectionString = connectionStringBuilder.ToString();
            SqliteConnection connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }

    }
}
