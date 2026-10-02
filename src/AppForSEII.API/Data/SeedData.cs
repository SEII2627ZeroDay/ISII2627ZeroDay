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
            SeedTeamsAndInvitations(dbContext);
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
                new Sport
                {
                    Name = "Football",
                    MinimumNumberOfPlayers = 11,
                    NumberofReferees = 3,
                    Description = "11 vs 11 association football",
                    BasicRules = "Rules for the game"
                },
                new Sport
                {
                    Name = "Basketball",
                    MinimumNumberOfPlayers = 5,
                    NumberofReferees = 2,
                    Description = "5 vs 5 basketball on standard court",
                    BasicRules = "Rules for the game"
                },
                new Sport
                {
                    Name = "Tennis",
                    MinimumNumberOfPlayers = 2,
                    NumberofReferees = 1,
                    Description = "Singles or doubles tennis match",
                    BasicRules = "Rules for the game"
                },
                new Sport
                {
                    Name = "Padel",
                    MinimumNumberOfPlayers = 4,
                    NumberofReferees = 1,
                    Description = "Doubles padel tournament match",
                    BasicRules = "Rules for the game"
                }
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
                    new Referee { Id = "6", Rating = 5, YearsRefereeing = 8, SportId = football.Id },
                    new Referee { Id = "7", Rating = 4, YearsRefereeing = 4, SportId = football.Id }
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

    public static void SeedTeamsAndInvitations(ApplicationDbContext dbContext)
    {
        if (!dbContext.Teams.Any())
        {
            var football = dbContext.Sports.FirstOrDefault(s => s.Name == "Football");
            var basketball = dbContext.Sports.FirstOrDefault(s => s.Name == "Basketball");

            var teams = new List<Team>();

            if (football != null)
            {
                teams.Add(new Team
                {
                    Name = "Eagles",
                    Description = "University football team",
                    MaxMembers = 15,
                    MinAge = 18,
                    MaxAge = 35,
                    CaptainId = "3", // Peter
                    SportId = football.Id
                });
            }

            if (basketball != null)
            {
                teams.Add(new Team
                {
                    Name = "Titans",
                    Description = "Basketball squad",
                    MaxMembers = 10,
                    MinAge = 18,
                    MaxAge = 30,
                    CaptainId = "1", // Elena
                    SportId = basketball.Id
                });
            }

            dbContext.Teams.AddRange(teams);
            dbContext.SaveChanges();
        }

        if (!dbContext.TeamInvitations.Any())
        {
            var eagles = dbContext.Teams.FirstOrDefault(t => t.Name == "Eagles");
            var titans = dbContext.Teams.FirstOrDefault(t => t.Name == "Titans");

            var invitations = new List<TeamInvitation>();

            if (eagles != null)
            {
                invitations.Add(new TeamInvitation
                {
                    UserId = "4", // Lucas
                    TeamId = eagles.Id,
                    InvitationAccepted = false,
                    InvitationMessage = "Join our football team for the upcoming championship!"
                });
            }

            if (titans != null)
            {
                invitations.Add(new TeamInvitation
                {
                    UserId = "5", // Sofia
                    TeamId = titans.Id,
                    InvitationAccepted = true,
                    InvitationMessage = "We would love to have you on the basketball team!"
                });
            }

            dbContext.TeamInvitations.AddRange(invitations);
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

            var interests = new List<InterestedIn>();

            if (football != null)
            {
                interests.Add(new InterestedIn { SportId = football.Id, UserId = "3", Skill = 4 }); // Peter
                interests.Add(new InterestedIn { SportId = football.Id, UserId = "4", Skill = 5 }); // Lucas
            }

            if (basketball != null)
            {
                interests.Add(new InterestedIn { SportId = basketball.Id, UserId = "3", Skill = 3 }); // Peter
                interests.Add(new InterestedIn { SportId = basketball.Id, UserId = "1", Skill = 4 }); // Elena
            }

            if (tennis != null)
            {
                interests.Add(new InterestedIn { SportId = tennis.Id, UserId = "5", Skill = 4 }); // Sofia
            }

            dbContext.InterestedIns.AddRange(interests);
            dbContext.SaveChanges();
        }
    }
}