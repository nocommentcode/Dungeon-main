using System;
using System.Linq;
using System.Numerics;
using UnityEngine;

/// <summary>
/// Pickup Treasure Behavior Factory
/// Picks up treasure if the theif is on one
/// </summary>
public class PickupTreasureBehaviourFactory : ABehaviourFactory
{
    // function to call to destroy treasure object
    private Func<DungeonTile, DungeonTile> _pickupTreasure;

    public PickupTreasureBehaviourFactory(
        DungeonGrid dungeonGrid, 
        Transform transform,
        Func<DungeonTile, DungeonTile> destroyTreasure) : base(dungeonGrid, transform){
            _pickupTreasure = destroyTreasure;
        }

    protected override float Delay => TheifSettings.TheifSpeed;

    public override bool Condition()
    {
        // pickup treasure if theif is on one
        var currentTile = GetCurrentTile();
        return _dungeonGrid.TreasureTiles.FirstOrDefault(treasure => treasure.Key == currentTile).Key != null;
    }

    public override void PerformAction()
    {
        var treasureTile = GetCurrentTile();
        _pickupTreasure(treasureTile);
    }
}
