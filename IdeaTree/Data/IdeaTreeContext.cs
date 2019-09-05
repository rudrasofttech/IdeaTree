using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdeaTree.Models;

namespace IdeaTree.Models
{
    public class IdeaTreeContext : DbContext
    {
        public IdeaTreeContext (DbContextOptions<IdeaTreeContext> options)
            : base(options)
        {
        }

        public DbSet<Idea> Idea { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<Vote> Vote { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<IdeaTree.Models.ManageProfileModel> ManageProfileModel { get; set; }
    }
}
