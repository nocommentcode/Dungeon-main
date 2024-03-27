using NPBehave;
using UnityEngine;

/// <summary>
/// An Abstract Behavior Factory
/// A behavior is a Sequenece of Condition node and Wait node
/// 
/// Children must implement:
///     - Delay - the amount of time to wait
///     - Condition() - wether to perform the behavior or not
///     - PerformAction() - performs the action
/// </summary>
public abstract class ABehaviourFactory{
    /// <summary>
    /// The amount of time to wait after performing behaviour
    /// </summary>
    protected abstract float Delay {get;}

    protected DungeonGrid _dungeonGrid;
    protected Transform _transform;

    public ABehaviourFactory(DungeonGrid dungeonGrid, Transform transform){
        _dungeonGrid = dungeonGrid;
        _transform = transform;
    }

    /// <summary>
    /// Returns current transform's tile
    /// </summary>
    /// <returns>Dungeon tile</returns>
    protected DungeonTile GetCurrentTile(){
        return _dungeonGrid.FirstOrDefault(tile => tile.GetGlobalPosition() == _transform.position);
    }

    /// <summary>
    /// Returns a new Sequence of this behavior
    /// </summary>
    /// <returns></returns>
    public Sequence Make(){
        return new Sequence(
            new Condition(Condition, new Action(PerformAction)),
            new Wait(Delay)
        );
    }

    /// <summary>
    /// Wether to perform this behavior or not. 
    /// Children must implement this
    /// </summary>
    public abstract bool Condition();

    /// <summary>
    /// Perform the behaviour.
    /// Children must implement this
    /// </summary>
    public abstract void PerformAction();

}
