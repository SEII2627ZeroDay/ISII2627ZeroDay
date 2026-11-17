using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    public class TeamStateContainer
    {
        public TeamForCreateDTO Team { get; private set; } = new TeamForCreateDTO()
        {
            InvitedPlayers = new List<TeamInvitationDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddPlayerToTeam(ApplicationUserForTeamDTO player)
        {
            if (Team.InvitedPlayers == null)
            {
                Team.InvitedPlayers = new List<TeamInvitationDTO>();
            }

            if (!Team.InvitedPlayers.Any(ti => ti.UserId == player.Id))
            {
                Team.InvitedPlayers.Add(new TeamInvitationDTO()
                {
                    UserId = player.Id,
                    Name = player.Name,
                    Surname = player.Surname,
                    Age = player.Age,
                    InvitationMessage = "",
                    InvitationAccepted = false
                });
                NotifyStateChanged();
            }
        }

        public void RemovePlayerFromTeam(TeamInvitationDTO item)
        {
            Team.InvitedPlayers?.Remove(item);
            NotifyStateChanged();
        }

        public void ClearTeamCart()
        {
            Team.InvitedPlayers?.Clear();
            NotifyStateChanged();
        }

        public void TeamProcessed()
        {
            // We have finished the team creation process so we create a new object without data
            Team = new TeamForCreateDTO()
            {
                InvitedPlayers = new List<TeamInvitationDTO>()
            };
            NotifyStateChanged();
        }
    }
}
