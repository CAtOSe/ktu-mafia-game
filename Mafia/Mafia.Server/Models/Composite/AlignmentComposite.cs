namespace Mafia.Server.Models.Composite;

public class AlignmentComposite(List<RoleComponent> roleList) : RoleComponent
{
    public override void AssignRole(List<Player> players)
    {
        var rng = new Random();
        foreach (var player in players)
        {
            var roleIndex = rng.Next(roleList.Count);
            roleList[roleIndex].AssignRole([player]);
        }
    }
}