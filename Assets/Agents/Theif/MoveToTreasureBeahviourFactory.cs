using UnityEngine;
using System.Linq;

/// <summary>
/// Move to Treasuer behavior factory
/// Moves the theif towards the treasure if there is still treasure in the dungeon
/// Movement is determined by a context map taking into account distance from closest treasure and troll locations
/// </summary> 
public class MoveToTreasureBeahviourFactory : AMoveBehaviourFactory
{
    public MoveToTreasureBeahviourFactory(DungeonGrid dungeonGrid, Transform transform) : base(dungeonGrid, transform)
    {
    }

    protected override float Delay => TheifSettings.TheifSpeed;

    public override bool Condition()
    {
        // move to treasure if there is treasure left to pickup
        return _dungeonGrid.TreasureTiles.Count > 0;
    }

    /// <summary>
    /// Finds the closest treasure tile
    /// </summary>
    /// <returns>Closest treasure</returns>
    private DungeonTile FindClosestTreasure()
    {        
        return _dungeonGrid.TreasureTiles
                            .OrderBy(treasure => Vector3.Distance(treasure.Value.transform.position, _transform.position))
                            .FirstOrDefault()
                            .Key;
    }

    protected override DungeonTile GetDestTile()
    {
        var closestTreasure = FindClosestTreasure();
        var trollLocations = _dungeonGrid.Trolls.Select(troll => troll.transform.position).ToList();

        // use context map to decide which tile to move to
        var contextMap = new TreasurePathContextMap(GetCurrentTile(), closestTreasure, trollLocations);
        return contextMap.GetBestTile();
    }
}
