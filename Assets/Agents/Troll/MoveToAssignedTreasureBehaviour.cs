using UnityEngine;

/// <summary>
/// Move to assigned treasure behavior
/// If the troll is assigned to a treasure, it will navigate towards it.
/// Will only execute if it is a ceratin distance away
/// </summary>
public class MoveToAssignedTreasureBehaviour : AMoveBehaviourFactory
{
    private Troll _troll;
    public MoveToAssignedTreasureBehaviour(DungeonGrid dungeonGrid, Transform transform, Troll troll) : base(dungeonGrid, transform)
    {
        _troll = troll;
    }

    protected override float Delay => TrollSettings.TrollSpeed;

    public override bool Condition()
    {
        // if no assigned treasure do not perform
        if(_troll.assignedTreasure == null){
            return false;
        }

        // if close enough to treasure do not perform
        if(Vector3.Distance(_transform.position, _troll.assignedTreasure.GetGlobalPosition()) <= TrollSettings.MinDistanceToTreasure){
            return false;
        }

        return true;
    }

    protected override DungeonTile GetDestTile()
    {
        // use a star to move towards assigned treasure
        return AStarSearch.NextTile(GetCurrentTile(), _troll.assignedTreasure);
    }
}
