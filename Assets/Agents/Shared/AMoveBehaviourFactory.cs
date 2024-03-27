using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Abstract Moving behavior Factory.
/// The Behavior will resulting in the transform moving this turn
/// Children must implement:
///     - Delay - the amount of time to wait
///     - Condition() - wether to perform the behavior or not
///     - GetDestTile() - the tile to move to
/// </summary> 
public abstract class AMoveBehaviourFactory : ABehaviourFactory
{
    protected AMoveBehaviourFactory(DungeonGrid dungeonGrid, Transform transform) : base(dungeonGrid, transform)
    {
    }

    /// <summary>
    /// Implements the abstract PerformAction() method by calling GetDestTile()
    /// </summary>
    public override void PerformAction()
    {
        // get destination from abstract method
        var destination = GetDestTile();
        
        // check destination valid
        if(destination == null){
            return;
        }
        if(!destination.IsDungeon){
            return;
        }
        
        // move to destination
        _transform.position = destination.GetGlobalPosition();
    }

    /// <summary>
    /// Returns the destination of the move.
    /// Children must implement this.
    /// </summary>
    /// <returns>The destination tile</returns>
    protected abstract DungeonTile GetDestTile();
}
