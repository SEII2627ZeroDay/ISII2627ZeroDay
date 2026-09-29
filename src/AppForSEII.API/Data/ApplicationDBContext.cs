using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;

namespace AppForSEII.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);

        builder.Entity<TeamInvitation>()
            .HasKey(invitation => new { invitation.UserId, invitation.TeamId });

    }


    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Referee> Referee { get; set;}
    public DbSet<Team> Teams { get; set; }

    public DbSet<Game> Games {get;set;} //get and set are like getters and setters of java
    public DbSet<Team> Gender { get; set; }
    public DbSet<TeamInvitation> TeamInvitations { get; set; }
    public DbSet<Sport> Sports { get; set; }
    public DbSet<RefereeGroup> RefereeGroup { get; set; }
    




}