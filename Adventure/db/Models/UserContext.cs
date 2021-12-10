using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Adventure.Helpers;

namespace Adventure
{
    public class UserContext : DbContext
    {


        public string DbPath { get; }
        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<UserResource> UserResources { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        public UserContext()
        {
            
            DbPath = System.IO.Path.Join(FileManager.DatabaseDirectoryPath, "users.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        public async Task<User> GrabUserAsync(ulong Id)
        {
            var user = await Users.FindAsync(Id);
            if (user == null)
            {
                user = new User() { Id = Id };
                await Users.AddAsync(user);
            }
            return user;
        }

    }

    public class User
    {
        [Key]public ulong Id { get; set; }
        public long Money { get;set; }
        public long Reputation { get; set; }
        public List<UserResource> UserResources { get; set; }
    }

    public class Resource
    {
        [Key]public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Level { get; set; }
        public long TradeValue { get; set; }
        public ResourceCategory Category { get; set; }

    }

    public class UserResource
    {
        [Key]public ulong Id { get; set; }
        public Resource Resource { get; set; }
        public long Amount { get; set; }
    }

    public class ResourceCategory
    {
        [Key] public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}
