using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using System.Linq;

/// <summary>
/// Abstract Dungeon Agent.
/// 
/// Dungeon Agents are controlled by NP Behave Behavior trees.
/// The tree will be constructed from a root with a selector node.
/// The Selector will select a single behavior to run this turn 
/// 
/// Children must implement:
///     - GetBehaviors() - returns a list of behaviors for this agent
/// </summary>
public abstract class ADungeonAgent: MonoBehaviour{
    protected DungeonGrid dungeonGrid;
    private Root tree;
    protected List<ABehaviourFactory> _behaviours;
    
    public virtual void Start(){
        // set dungeon grid instance
        var dungeon = transform.parent.gameObject;
        dungeonGrid = dungeon.GetComponent<DungeonGenerator>().dungeonGrid;

        // build behaviors and behavior tree
        _behaviours = GetBehaviours();
        tree = CreateBehaviourTree();
        tree.Start();
    }


    private Root CreateBehaviourTree(){
        return new Root(new Selector(_behaviours.Select(b => b.Make()).ToArray()));
    }

    protected abstract List<ABehaviourFactory> GetBehaviours();
}
