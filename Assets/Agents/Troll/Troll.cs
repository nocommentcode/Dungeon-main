using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Troll agent.
/// The troll moves to its assigned treasure and patrols around it.
/// Behaviors:
///     1) Attack Theif if on the same tile
///     2) Move towards Theif if in range
///     1) Move towards assigned treasure
///     2) Patrol around treasure if close enough
/// </summary>
public class Troll : ADungeonAgent {
    public DungeonTile assignedTreasure;
    System.Random random = new System.Random();

    public void Update() {
        // assigned a new treasure if not assigned
        if (assignedTreasure == null) {
            AssignNewTreasure();
        }

        // assigned a new treasure if previous one has been stolen by the theif
        if (dungeonGrid.TreasureTiles.FirstOrDefault(tile => tile.Key == assignedTreasure).Key == null) {
            AssignNewTreasure();
        }
    }

    /// <summary>
    /// Assigns the troll a new treasure to defend
    /// </summary> 
    private void AssignNewTreasure() {
        var index = random.Next(0, dungeonGrid.TreasureTiles.Count);
        assignedTreasure = dungeonGrid.TreasureTiles.Keys.ToArray()[index];
    }

    protected override List<ABehaviourFactory> GetBehaviours()
    {
        void AttackTheif() {
            Destroy(dungeonGrid.Theif);
            dungeonGrid.Theif = null;
        }

        return new List<ABehaviourFactory>(){
            new AttackTheifBehavior(dungeonGrid, transform, AttackTheif),
            new MoveTowardsTheifBehavior(dungeonGrid, transform),
            new MoveToAssignedTreasureBehaviour(dungeonGrid, transform, this),
            new PatrolAssignedTreasureBehaviour(dungeonGrid, transform, this)
        };
    }

   

    
}
