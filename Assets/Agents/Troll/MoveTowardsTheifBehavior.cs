using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Move towards theif behaviour factory
/// Will move the troll towards the theif if close enough
/// </summary> 
public class MoveTowardsTheifBehavior : AMoveBehaviourFactory
{
    public MoveTowardsTheifBehavior(DungeonGrid dungeonGrid, Transform transform) : base(dungeonGrid, transform)
    {
    }

    protected override float Delay => TrollSettings.TrollSpeed;

    public override bool Condition()
    {
        // if theif is too far do not run
        if(Vector3.Distance(_transform.position, _dungeonGrid.Theif.transform.position) > TrollSettings.VisibleRange){
            return false;
        }

        return true;
    }

    protected override DungeonTile GetDestTile()
    {
        // use a star to move towards theif
        var theifTile = _dungeonGrid.FirstOrDefault(tile => tile.GetGlobalPosition() == _dungeonGrid.Theif.transform.position);
        return AStarSearch.NextTile(GetCurrentTile(), theifTile);
    }
}
