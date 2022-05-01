using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Models;
using RopeyDVDs.Models.ViewModels;

namespace RopeyDVDs.DBContext
{
    public class ApplicationDBContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<RopeyDVDs.Models.Actor> Actor { get; set; }
        public DbSet<RopeyDVDs.Models.CastMember> CastMember { get; set; }
        public DbSet<RopeyDVDs.Models.DVDCategory> DVDCategory { get; set; }
        public DbSet<RopeyDVDs.Models.DVDCopy> DVDCopy { get; set; }
        public DbSet<RopeyDVDs.Models.DVDTitle> DVDTitle { get; set; }
        public DbSet<RopeyDVDs.Models.Loan> Loan { get; set; }
        public DbSet<RopeyDVDs.Models.LoanType> LoanType { get; set; }
        public DbSet<RopeyDVDs.Models.Member> Member { get; set; }
        public DbSet<RopeyDVDs.Models.MembershipCategory> MembershipCategory { get; set; }
        public DbSet<RopeyDVDs.Models.Producer> Producer { get; set; }
        public DbSet<RopeyDVDs.Models.Studio> Studio { get; set; }
        public DbSet<RopeyDVDs.Models.User> User { get; set; }
        //public DbSet<RopeyDVDs.Models.ViewModels.UserDetailsViewModel> UserDetailsViewModel { get; set; }
       
    }
}
