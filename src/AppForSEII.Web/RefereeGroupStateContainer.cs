using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    public class RefereeGroupStateContainer
    {
        public RefereeGroupDTO RefereeGroup { get; private set; } = new RefereeGroupDTO()
        {
            RefereeAssignedTos = new List<RefereeAssignedToDTO>()
        };

        // The selected referees with the data needed to show them in the next steps.
        // RefereeAssignedToDTO only has the id of the referee, not his/her name, years or rating.
        public List<RefereeForRefereeGroupDTO> SelectedReferees { get; private set; } = new List<RefereeForRefereeGroupDTO>();

        // Sport of the game whose referees are being selected (so the page remembers it when the user comes back)
        public int SportId { get; set; }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddRefereeToGroup(RefereeForRefereeGroupDTO referee)
        {
            if (RefereeGroup.RefereeAssignedTos == null)
            {
                RefereeGroup.RefereeAssignedTos = new List<RefereeAssignedToDTO>();
            }

            if (!RefereeGroup.RefereeAssignedTos.Any(a => a.RefereeId == referee.Id))
            {
                // the referee is pending of assignment; the role and its description are filled in the next step
                RefereeGroup.RefereeAssignedTos.Add(new RefereeAssignedToDTO()
                {
                    RefereeId = referee.Id,
                    AcceptedAssignement = false,
                    Role = "",
                    RoleDescription = ""
                });
                SelectedReferees.Add(referee);
                NotifyStateChanged();
            }
        }

        public void RemoveRefereeFromGroup(RefereeForRefereeGroupDTO referee)
        {
            var assignment = RefereeGroup.RefereeAssignedTos?.FirstOrDefault(a => a.RefereeId == referee.Id);
            if (assignment != null)
            {
                RefereeGroup.RefereeAssignedTos?.Remove(assignment);
            }
            SelectedReferees.RemoveAll(r => r.Id == referee.Id);
            NotifyStateChanged();
        }

        public void ClearRefereeGroupCart()
        {
            RefereeGroup.RefereeAssignedTos?.Clear();
            SelectedReferees.Clear();
            NotifyStateChanged();
        }

        public void RefereeGroupProcessed()
        {
            // We have finished the referee group creation process so we create a new object without data
            RefereeGroup = new RefereeGroupDTO()
            {
                RefereeAssignedTos = new List<RefereeAssignedToDTO>()
            };
            SelectedReferees = new List<RefereeForRefereeGroupDTO>();
            SportId = 0;
            NotifyStateChanged();
        }
    }
}