using AppForSEII.API.Models;

namespace AppForSEII.UT;

public class TeamInvitationTests
{
    [Fact]
    public void NewInvitationHasPendingStatusAndDefaultMessage()
    {
        var invitation = new TeamInvitation();

        Assert.False(invitation.InvitationAccepted);
        Assert.Equal("Invitation Message for a team member", invitation.InvitationMessage);
    }
}