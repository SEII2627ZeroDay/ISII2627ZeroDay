using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    // HTTP is a stateless protocol, so this service (registered in Program.cs) keeps the information
    // of the referee group that the user is creating while he/she moves between pages.
    public class RefereeGroupStateContainer
    {
        // we create an instance of RefereeGroupDTO when an instance of RefereeGroupStateContainer is created
        public RefereeGroupDTO RefereeGroup { get; private set; } = new RefereeGroupDTO()
        {
            RefereeAssignedTos = new List<RefereeAssignedToDTO>()
        };

        // the selected referees, with the data needed to show them in the next steps
        // (RefereeAssignedToDTO only has the id of the referee, not his/her name, years or rating)
        public List<RefereeForRefereeGroupDTO> SelectedReferees { get; private set; } = new List<RefereeForRefereeGroupDTO>();

        // sport of the game whose referees are being selected (so the page remembers it when the user comes back)
        public int SportId { get; set; }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddRefereeToGroup(RefereeForRefereeGroupDTO referee)
        {
            // before adding a referee we check whether it has been already added
            if (!RefereeGroup.RefereeAssignedTos.Any(a => a.RefereeId == referee.Id))
            {
                // we add it if it is not in the list: the referee is pending of assignment
                // (the role and its description are filled in later by the user)
                RefereeGroup.RefereeAssignedTos.Add(new RefereeAssignedToDTO()
                {
                    RefereeId = referee.Id,
                    AcceptedAssignement = false
                });
                SelectedReferees.Add(referee);
                NotifyStateChanged();
            }
        }

        // to delete a referee from the list of selected referees
        public void RemoveRefereeFromGroup(RefereeForRefereeGroupDTO referee)
        {
            var assignment = RefereeGroup.RefereeAssignedTos.FirstOrDefault(a => a.RefereeId == referee.Id);
            if (assignment != null)
            {
                RefereeGroup.RefereeAssignedTos.Remove(assignment);
            }
            SelectedReferees.RemoveAll(r => r.Id == referee.Id);
            NotifyStateChanged();
        }

        // we eliminate all the referees from the list
        public void ClearSelectedReferees()
        {
            RefereeGroup.RefereeAssignedTos.Clear();
            SelectedReferees.Clear();
            NotifyStateChanged();
        }

        // we have already finished the process of adding the referee group, thus, we create a new one
        public void RefereeGroupProcessed()
        {
            RefereeGroup = new RefereeGroupDTO()
            {
                RefereeAssignedTos = new List<RefereeAssignedToDTO>()
            };
            SelectedReferees = new List<RefereeForRefereeGroupDTO>();
            NotifyStateChanged();
        }
    }
}