using Microsoft.EntityFrameworkCore;


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Thread> Threads { get; set; }

         public DbSet<AppUser> AppUsers { get; set; }


    }
    public class Thread
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StartUpMessage {get;set;}

    }


    public class UserThread
    {
        public string Id {get;set;}
        public string ThreadId{get;set;}
        public Guid AppUserId{get;set;}
    }

    public class AppUser
    {
        public Guid Id{get;set;}
        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public bool Gender {get;set;}

        public int Age {get;set;}

        public bool HasAccess {get;set;}

        public DateTime JoinedDate{get;set;}
    }
