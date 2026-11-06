using AppForSEII.API.Models;

namespace AppForSEII.UT;

public class TeamInvitationTests
{
    [Fact]
    public void NewInvitationHasPendingStatusAndDefaultMessage()
    {
        var user = new ApplicationUser("1", "Elena", "Navarro", "elena@uclm.es", new DateOnly(1980, 1, 1), 40, Gender.Female);
        var sport = new Sport("Football", 11, 3, "Football", "Rules");
        var team = new Team("Eagles", "Desc", 15, 18, 35, user, sport);
        var invitation = new TeamInvitation(user, team, "Invitation Message for a team member");

        Assert.False(invitation.InvitationAccepted);
        Assert.Equal("Invitation Message for a team member", invitation.InvitationMessage);
    }
}