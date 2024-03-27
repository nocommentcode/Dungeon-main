using System.Linq;
using UnityEngine;

/// <summary>
/// Patrol Assigned Treasure Behavior
/// Troll will occasionaly patrol around it's assigned treasure if:
///     - It is assigned a treasure
///     - it is close enough to it
/// 
/// </summary>
public class PatrolAssignedTreasureBehaviour : AMoveBehaviourFactory
{
    private Troll _troll;
    private System.Random _random;

    public PatrolAssignedTreasureBehaviour(DungeonGrid dungeonGrid, Transform transform, Troll troll) : base(dungeonGrid, transform)
    {
        _troll = troll;
        _random = new System.Random();
    }

    protected override float Delay => TrollSettings.TrollSpeed;

    public override bool Condition()
    {
        // if no assigned treasure do not perform
        if(_troll.assignedTreasure == null){
            return false;
        }

        // if not close enough to treasure do not perform
        if(Vector3.Distance(_transform.position, _troll.assignedTreasure.GetGlobalPosition()) > TrollSettings.MinDistanceToTreasure){
            return false;
        }

        // most of the time do not patrol (so that the troll doesnt spaz around randomly)
        if(_random.NextDouble() > TrollSettings.PatrolProb){
            return false;
        }

        return true;
    }

    protected override DungeonTile GetDestTile()
    {   
        // select a random dungeon tile neighbouring troll
        var currentTile = GetCurrentTile();
        var neighbours = currentTile.Neighbours.Values
                            .Where(tile => tile.IsDungeon)
                            .ToList();
        
        // randomly select
        var index = _random.Next(0, neighbours.Count);
        return neighbours[index];
    }
}