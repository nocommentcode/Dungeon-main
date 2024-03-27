using System.Collections.Generic;

/// <summary>
/// Theif agent.
/// The Theif attempts to collect all the treasure in the dungeon.
/// 
/// Its behaviors are:
///     1) Try pickup the treasure if on one
///     2) Move to next treasure
/// </summary>
public class Theif : ADungeonAgent
{
    protected override List<ABehaviourFactory> GetBehaviours()
    {
        return new List<ABehaviourFactory>(){
            MakePickupTreasureBehaviour(),
            MakeMoveToTreasureBehaviour()
        };
    }

    private PickupTreasureBehaviourFactory MakePickupTreasureBehaviour(){
        DungeonTile PickupTreasure(DungeonTile treasureTile){
            Destroy(dungeonGrid.TreasureTiles[treasureTile]);
            dungeonGrid.TreasureTiles.Remove(treasureTile);
            return treasureTile;
        }

        return new PickupTreasureBehaviourFactory(dungeonGrid, transform, PickupTreasure);        
    }

    private MoveToTreasureBeahviourFactory MakeMoveToTreasureBehaviour(){
        return new MoveToTreasureBeahviourFactory(dungeonGrid, transform);
    }

}
