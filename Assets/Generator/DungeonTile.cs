using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Implements a Tile in the Dungeon.
/// Used throughout the game to interact with the Dungeon.
/// </summary>
public class DungeonTile{

    /// <summary>
    /// Whether the tile is part of the dungone or a wall
    /// </summary>
    public bool IsDungeon {get; set;}

    /// <summary>
    /// Coordinates
    /// </summary>
    public int X {get;}
    public int Y {get;}

    /// <summary>
    /// Tiles accicible from this tile in each direction
    /// </summary>
    public Dictionary<Direction, DungeonTile> Neighbours = new Dictionary<Direction, DungeonTile>();
    
    
    // 3x3 Moor neighbours
    private List<DungeonTile> _moorNeighbours {get;set;} = new List<DungeonTile>();
    
    // The tilemap object
    private Tilemap _tilemap;
    
    public DungeonTile(int x, int y, Tilemap tilemap, bool isFilled=false){
        IsDungeon = isFilled;
        X = x;
        Y = y;
        _tilemap = tilemap;
    }
    
    /// <summary>
    /// Gets the postion of the tile in the tilemap
    /// </summary>
    /// <returns>Vector3Int</returns>
    public Vector3Int GetTilemapPosition(){
        return new Vector3Int(X * 16, Y * 16);
    }

    /// <summary>
    /// Gets the position of the tile in the world
    /// </summary>
    /// <returns>Vector3</returns>
    public Vector3 GetGlobalPosition(){
        return _tilemap.CellToWorld(GetTilemapPosition());
    }

    /// <summary>
    /// Checks if tile is a neighbour and adds it to its neighbours
    /// </summary>
    /// <param name="neighbour">The tile to check</param>
    public void TryRegisterNeighbour(DungeonTile neighbour){
        int xOffset = X - neighbour.X;
        int yOffset = Y - neighbour.Y;

        // if same tile skip
        if(xOffset == 0 && yOffset == 0){
            return;
        }

        // if not in 3 x 3 grid skip
        if (xOffset > 1 || xOffset < -1){
            return;
        }
        if (yOffset > 1 || yOffset < -1){
            return;
        }

        // add moore neighbour
        _moorNeighbours.Add(neighbour);

        // if diagonal skip
        if(xOffset != 0 && yOffset != 0){
            return;
        }

        // find neihbour's direction
        Direction? direction = null;
        if (xOffset == 0){
            direction = yOffset > 0 ? Direction.DOWN : Direction.UP;
        }

        if(yOffset == 0){
            direction = xOffset > 0 ? Direction.LEFT : Direction.RIGHT;
        }
        if(!direction.HasValue){
            return;
        }

        // set neighbour
        Neighbours[direction.Value] = neighbour; 
    }

    /// <summary>
    /// Gets the number of dungeon tiles of the 3x3 Moor neighbour 
    /// </summary>
    /// <returns># dungeon tiles in 3x3 moor neighbours</returns>
    public int MoorNeighbourDungeonCount(){
        return _moorNeighbours.Select(tile => tile.IsDungeon ? 1: 0).Sum();
    }
}
