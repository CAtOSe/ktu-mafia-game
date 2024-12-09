namespace Mafia.Server.Models.Composite;

public class RoleAssignmentComposite : RoleComponent
{
    private readonly List<(RoleComponent Role, int Count)> _roleList = new();
    
    public override void AssignRole(List<Player> players)
    {
        var unassignedPlayers = new List<Player>(players);
        
        var rng = new Random();
        foreach (var role in _roleList)
        {
            var playerSubset = new List<Player>();
            var count = 0;

            while (count < role.Count && unassignedPlayers.Count > 0)
            {
                var index = rng.Next(unassignedPlayers.Count);
                playerSubset.Add(unassignedPlayers[index]);
                unassignedPlayers.RemoveAt(index);
                count++;
            }

            role.Role.AssignRole(playerSubset);
        }
    }

    public void Add(RoleComponent component, int count)
    {
        _roleList.Add((component, count));
    }
}