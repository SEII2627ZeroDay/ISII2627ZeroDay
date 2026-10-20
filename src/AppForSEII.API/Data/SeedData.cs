using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Data;

public class SeedData
{
    public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger)
    {
        List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        try
        {
            SeedRoles(roleManager, rolesNames);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the roles in the Database.");
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        try
        {
            SeedUsers(userManager, rolesNames);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the Users in the Database.");
        }

        try
        {
            SeedSports(dbContext);
            SeedTeams(dbContext);
            SeedInterests(dbContext);
            SeedReferees(dbContext);
            SeedGamesAndInvitations(dbContext);
            SeedRefereeGroupsAndAssignments(dbContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding domain data in the Database.");
        }
    }

    public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles)
    {
        foreach (string roleName in roles)
        {
            if (!roleManager.RoleExistsAsync(roleName).Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                roleManager.CreateAsync(role).Wait();
            }
        }
    }

    public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
    {
        if (userManager.FindByNameAsync("elena@uclm.es").Result == null)
        {
            ApplicationUser user = new ApplicationUser(
                "1",
                "Elena",
                "Navarro Martínez",
                "elena@uclm.es",
                new DateOnly(1985, 5, 20),
                41,
                Gender.Female)
            {
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(user, "Password1234%").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, roles[0]).Wait();
            }
        }

        if (userManager.FindByNameAsync("peter@uclm.es").Result == null)
        {
            ApplicationUser user = new ApplicationUser(
                "3",
                "Peter",
                "Jackson",
                "peter@uclm.es",
                new DateOnly(1998, 8, 15),
                28,
                Gender.Male)
            {
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(user, "OtherPass12$").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, roles[2]).Wait();
            }
        }

        if (userManager.FindByNameAsync("lucas@uclm.es").Result == null)
        {
            ApplicationUser user = new ApplicationUser(
                "4",
                "Lucas",
                "García López",
                "lucas@uclm.es",
                new DateOnly(2000, 3, 10),
                26,
                Gender.Male)
            {
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(user, "LucasPass12$").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, roles[2]).Wait();
            }
        }

        if (userManager.FindByNameAsync("sofia@uclm.es").Result == null)
        {
            ApplicationUser user = new ApplicationUser(
                "5",
                "Sofía",
                "Fernández Ruiz",
                "sofia@uclm.es",
                new DateOnly(2002, 11, 25),
                24,
                Gender.Female)
            {
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(user, "SofiaPass12$").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, roles[2]).Wait();
            }
        }

        if (userManager.FindByNameAsync("carlos@uclm.es").Result == null)
        {
            ApplicationUser user = new ApplicationUser(
                "6",
                "Carlos",
                "Gómez Soto",
                "carlos@uclm.es",
                new DateOnly(1992, 4, 12),
                34,
                Gender.Male)
            {
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(user, "RefereePass12$").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, roles[1]).Wait();
            }
        }

        if (userManager.FindByNameAsync("ana@uclm.es").Result == null)
        {
            ApplicationUser user = new ApplicationUser(
                "7",
                "Ana",
                "Torres Blanco",
                "ana@uclm.es",
                new DateOnly(1995, 9, 30),
                31,
                Gender.Female)
            {
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(user, "RefereePass12$").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, roles[1]).Wait();
            }
        }
    }

    public static void SeedSports(ApplicationDbContext dbContext)
    {
        if (!dbContext.Sports.Any())
        {
            var sports = new List<Sport>
            {
                new Sport("Football", 11, 3, "11 vs 11 association football", "Rules for the game"),
                new Sport("Basketball", 5, 2, "5 vs 5 basketball on standard court", "Rules for the game"),
                new Sport("Tennis", 2, 1, "Singles or doubles tennis match", "Rules for the game"),
                new Sport("Padel", 4, 1, "Doubles padel tournament match", "Rules for the game")
            };

            dbContext.Sports.AddRange(sports);
            dbContext.SaveChanges();
        }
    }

    public static void SeedReferees(ApplicationDbContext dbContext)
    {
        if (!dbContext.Referee.Any())
        {
            var football = dbContext.Sports.FirstOrDefault(s => s.Name == "Football");
            if (football != null)
            {
                var referees = new List<Referee>
                {
                    new Referee(rating: 5, yearsRefereeing: 8, sportId: football.Id),
                    new Referee(rating: 4, yearsRefereeing: 4, sportId: football.Id)
                };

                dbContext.Referee.AddRange(referees);
                dbContext.SaveChanges();
            }
        }
    }

    public static void SeedGamesAndInvitations(ApplicationDbContext dbContext)
    {
        if (!dbContext.Games.Any())
        {
            var football = dbContext.Sports.FirstOrDefault(s => s.Name == "Football");
            var basketball = dbContext.Sports.FirstOrDefault(s => s.Name == "Basketball");

            var elena = dbContext.Users.FirstOrDefault(u => u.Email == "elena@uclm.es");
            var peter = dbContext.Users.FirstOrDefault(u => u.Email == "peter@uclm.es");

            var games = new List<Game>();

            if (football != null)
            {
                games.Add(new Game
                {
                    Name = "Final Cup 2026",
                    Date = DateTime.UtcNow.AddDays(7),
                    Place = "Campus Central Stadium",
                    Description = "University Championship Final Match",
                    SportId = football.Id,
                    ResponsibleForId = "1" // Elena
                });
            }

            if (basketball != null)
            {
                games.Add(new Game
                {
                    Name = "Spring Derby",
                    Date = DateTime.UtcNow.AddDays(14),
                    Place = "Sports Complex Arena A",
                    Description = "Spring Derby Basketball Game",
                    SportId = basketball.Id,
                    ResponsibleForId = "3" // Peter
                });
            }

            dbContext.Games.AddRange(games);
            dbContext.SaveChanges();
        }

        if (!dbContext.GameInvitations.Any())
        {
            var eagles = dbContext.Teams.FirstOrDefault(t => t.Name == "Eagles");
            var finalCup = dbContext.Games.FirstOrDefault(g => g.Name == "Final Cup 2026");
            var titans = dbContext.Teams.FirstOrDefault(t => t.Name == "Titans");
            var springDerby = dbContext.Games.FirstOrDefault(g => g.Name == "Spring Derby");

            if (eagles != null && finalCup != null)
            {
                var invitation = new GameInvitation
                {
                    TeamId = eagles.Id,
                    GameId = finalCup.Id,
                    AcceptedGame = true,
                    Message = "Invitation to participate in the Final Cup 2026 match."
                };

                dbContext.GameInvitations.Add(invitation);
                dbContext.SaveChanges();
            }

            if (titans != null && springDerby != null)
            {
                var invitation = new GameInvitation
                {
                    TeamId = titans.Id,
                    GameId = springDerby.Id,
                    AcceptedGame = true,
                    Message = "Invitation to participate in the Spring Derby match."
                };

                dbContext.GameInvitations.Add(invitation);
                dbContext.SaveChanges();
            }
        }
    }

    public static void SeedRefereeGroupsAndAssignments(ApplicationDbContext dbContext)
    {
        if (!dbContext.RefereeGroup.Any())
        {
            var firstGame = dbContext.Games.FirstOrDefault();
            if (firstGame != null)
            {
                var refereeGroup = new RefereeGroup
                {
                    Name = "Main Ref Group",
                    Description = "Lead referees for championship",
                    Rules = "Rules for the game",
                    GameId = firstGame.Id
                };

                dbContext.RefereeGroup.Add(refereeGroup);
                dbContext.SaveChanges();
            }
        }

        if (!dbContext.RefereeAssignedTo.Any())
        {
            var refGroup = dbContext.RefereeGroup.FirstOrDefault(rg => rg.Name == "Main Ref Group");
            if (refGroup != null)
            {
                var assignments = new List<RefereeAssignedTo>
                {
                    new RefereeAssignedTo
                    {
                        RefereeGroupId = refGroup.Id,
                        RefereeId = "6", // Carlos
                        AcceptedAssignement = true,
                        Role = "Head Referee",
                        RoleDescription = "Chief referee supervising the game"
                    },
                    new RefereeAssignedTo
                    {
                        RefereeGroupId = refGroup.Id,
                        RefereeId = "7", // Ana
                        AcceptedAssignement = false,
                        Role = "Assistant Referee",
                        RoleDescription = "Assistant referee on the touchline"
                    }
                };

                dbContext.RefereeAssignedTo.AddRange(assignments);
                dbContext.SaveChanges();
            }
        }
    }

    public static void SeedTeams(ApplicationDbContext dbContext)
    {
        if (!dbContext.Teams.Any())
        {
            var football = dbContext.Sports.FirstOrDefault(s => s.Name == "Football");
            var basketball = dbContext.Sports.FirstOrDefault(s => s.Name == "Basketball");
            var peter = dbContext.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es");
            var elena = dbContext.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es");

            var teams = new List<Team>();

            if (football != null && peter != null)
            {
                teams.Add(new Team("Eagles", "University football team", 15, 18, 35, peter, football));
            }

            if (basketball != null && elena != null)
            {
                teams.Add(new Team("Titans", "Basketball squad", 10, 18, 30, elena, basketball));
            }

            dbContext.Teams.AddRange(teams);
            dbContext.SaveChanges();
        }
    }

    public static void SeedInterests(ApplicationDbContext dbContext)
    {
        if (!dbContext.InterestedIns.Any())
        {
            var football = dbContext.Sports.FirstOrDefault(s => s.Name == "Football");
            var basketball = dbContext.Sports.FirstOrDefault(s => s.Name == "Basketball");
            var tennis = dbContext.Sports.FirstOrDefault(s => s.Name == "Tennis");

            var peter = dbContext.Users.FirstOrDefault(u => u.UserName == "peter@uclm.es");
            var lucas = dbContext.Users.FirstOrDefault(u => u.UserName == "lucas@uclm.es");
            var elena = dbContext.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es");
            var sofia = dbContext.Users.FirstOrDefault(u => u.UserName == "sofia@uclm.es");

            var interests = new List<InterestedIn>();

            if (football != null)
            {
                if (peter != null) interests.Add(new InterestedIn(football, peter, 4));
                if (lucas != null) interests.Add(new InterestedIn(football, lucas, 5));
            }

            if (basketball != null)
            {
                if (peter != null) interests.Add(new InterestedIn(basketball, peter, 3));
                if (elena != null) interests.Add(new InterestedIn(basketball, elena, 4));
            }

            if (tennis != null && sofia != null)
            {
                interests.Add(new InterestedIn(tennis, sofia, 4));
            }

            dbContext.InterestedIns.AddRange(interests);
            dbContext.SaveChanges();
        }
    }
}
