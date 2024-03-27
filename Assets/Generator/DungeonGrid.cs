using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A collection of DungeonTiles representing the dungeon.
/// Also stores the treasure tiles.
/// Functionality:
///     - Generate random dungeon
///     - Perform Cellular Automata
///     - Post process dungeon
/// </summary>
public class DungeonGrid {

    /// <summary>
    /// The location of the treasures in the dungeon and the corresponding game object
    /// </summary>
    public Dictionary<DungeonTile, GameObject> TreasureTiles {get; set;} = new Dictionary<DungeonTile, GameObject>();

    /// <summary>
    /// The Theif game object
    /// </summary>
    public GameObject Theif { get; set; }

    /// <summary>
    /// The Troll Game object
    /// </summary>
    public List<GameObject> Trolls { get; set; }

    // the collection of tiles
    private List<DungeonTile> _flatTiles;
    
    // the tilemap object
    private Tilemap _tilemap;

    // random generator
    private System.Random _random;

    public DungeonGrid(int width, int height, System.Random rng, Tilemap tilemap){
        _random = rng;
        _tilemap = tilemap;
        _tilemap.size = new Vector3Int(width, height, 0);
        _flatTiles = BuildTiles(width, height);        
    }

    
    # region procedular-generation

    /// <summary>
    /// Initialises the Dungeon with random walls and dungeon tiles
    /// </summary>
    /// <param name="density">The density of dungeon tiles to randomise</param>
    public void Randomise(float density){
        _flatTiles.ForEach(tile => {
            if (_random.NextDouble() <= density){
                tile.IsDungeon = true;
            }
        });
    }

    /// <summary>
    /// Iterates with Cellular Automata on 3x3 Moor neighbours
    /// </summary>
    public void Iterate(){
        // get next state of each tile after cellular automata
        var nextValues = _flatTiles.Select(tile => {
            int moorDungeonCount = tile.MoorNeighbourDungeonCount();

            // if >= 5 tiles filled set to dungeon
            if (moorDungeonCount >= 5){
                return true;
            }

            // if at most 2 filled, set to wall
            if(moorDungeonCount <= 2){
                return false;
            }

            // set to previous value
            return tile.IsDungeon;
        }).ToList();

        // set each tile to its new value
        _flatTiles.Zip(nextValues, (tile, filled) => new {tile, filled})
                    .ToList()
                    .ForEach((result) => result.tile.IsDungeon = result.filled);
    }
    
    
    /// <summary>
    /// Post processes the dungeon by linking seaparated clusters
    /// </summary>
    public void PostProcess(){
        LinkUnreachableTiles();
    }

    /// <summary>
    /// Builds each tile and registers neighbours
    /// </summary>
    /// <param name="width">width of dungeon</param>
    /// <param name="height">height of dungeon</param>
    private List<DungeonTile> BuildTiles(int width, int height){
        var tiles = new DungeonTile[width, height];

        // Instantiate emtpy tiles
        for (int x = 0; x<width; x++){
            for (int y = 0; y < height; y++){
                var tile = new DungeonTile(x, y, _tilemap);
                tiles[x, y] = tile;
            }
        }
        
        // flatten 2d array to a list for convinient use
        var flatTiles = new List<DungeonTile>();
        for (int x = 0; x<width; x++){
            for (int y = 0; y < height; y++){
                flatTiles.Add(tiles[x, y]);
            }
        }

        // for each tile present every other tile to register as potential neighbour
        flatTiles.ForEach(tile => {
            flatTiles.ForEach(neighbour => tile.TryRegisterNeighbour(neighbour));
        });

        return flatTiles;
    }



    /// <summary>
    /// Links separated clusters
    /// </summary>
    private void LinkUnreachableTiles(){
        var clusters = new List<HashSet<DungeonTile>>();
        
        // function that finds a tile not in a known cluster
        DungeonTile FindTileNotInClusters(){
            return _flatTiles.Where(tile => tile.IsDungeon && clusters.FirstOrDefault(cluster => cluster.Contains(tile)) == null).FirstOrDefault();
        }

        // iteratively build clusters untill all tiles are accounted for
        var uncluseterdTile = FindTileNotInClusters();
        while(uncluseterdTile != null){
            var newCluster = FindCluster(uncluseterdTile); 
            clusters.Add(newCluster);
            uncluseterdTile = FindTileNotInClusters();
        }

        // link clusters to main cluster one by one
        var largestCluster = clusters.OrderByDescending(cluster => cluster.Count).First();
        clusters.Remove(largestCluster);
        foreach(var cluster in clusters){
            MergeClusters(largestCluster, cluster);
        }
    }

    /// <summary>
    /// Merges clusters by replacing walls to create a tunnel linking the two
    /// </summary>
    /// <param name="mainCluster">The main cluster</param>
    /// <param name="disconnectedCluster">The disconnected cluster to connect</param>
    private void MergeClusters(HashSet<DungeonTile> mainCluster, HashSet<DungeonTile> disconnectedCluster){
        // finds the two closest tiles in each cluster
        Tuple<DungeonTile, DungeonTile> FindClosestTiles(){
            Tuple<DungeonTile, DungeonTile> closestTiles = null;
            float minDistance = float.MaxValue;

            foreach (var tile1 in mainCluster)
            {
                foreach (var tile2 in disconnectedCluster)
                {
                    float distance = Vector3Int.Distance(tile1.GetTilemapPosition(), tile2.GetTilemapPosition());
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTiles = Tuple.Create(tile1, tile2);
                    }
                }
            }

            return closestTiles;    
        }

        var closestTiles = FindClosestTiles();

        // connects the two closest tiles in 1 direction
        DungeonTile ConnectTiles(int delta, Direction direction, DungeonTile startingTile){
            for(int i=0; i< delta; i++){
                startingTile = startingTile.Neighbours[direction];
                startingTile.IsDungeon = true;
            }
            return startingTile;
        }

        // connect clusters in x axis
        int deltaX = closestTiles.Item1.X - closestTiles.Item2.X;
        DungeonTile neighbour = ConnectTiles(Math.Abs(deltaX), deltaX > 0 ? Direction.RIGHT : Direction.LEFT, closestTiles.Item2);
        
        // connect custers in y axis
        int deltaY = closestTiles.Item1.Y - closestTiles.Item2.Y;
        ConnectTiles(Math.Abs(deltaY), deltaY > 0 ? Direction.UP: Direction.DOWN, neighbour);
    }

    /// <summary>
    /// Finds all dungeon tiles connected to a given starting tile.
    /// </summary>
    /// <param name="startingTile">The tile to start the search from</param>
    /// <returns>Hash set of all tiles reachable from starting tile</returns>
    private HashSet<DungeonTile> FindCluster(DungeonTile startingTile){
        var visited = new HashSet<DungeonTile>(){startingTile};
        var frontier = new Queue<DungeonTile>();
        frontier.Enqueue(startingTile);

        // add each tiles neighbours untill all tiles reachable are visited
        while(frontier.Count > 0){
            var currentTile = frontier.Dequeue();

            foreach(var neighbour in currentTile.Neighbours.Values.Where(tile => tile.IsDungeon).ToList()){
                if(!visited.Contains(neighbour)){
                    visited.Add(neighbour);
                    frontier.Enqueue(neighbour);
                }
            }
        }

        return visited;
    }
    
    #endregion


    # region tile-access
    /// <summary>
    /// Returns first tile matching criteria or null
    /// </summary>
    /// <param name="selector">Criteria to select tile on</param>
    /// <returns>Tile or null</returns>
    public DungeonTile FirstOrDefault(Func<DungeonTile, bool> selector){
        return _flatTiles.FirstOrDefault(selector);
    }

    /// <summary>
    /// Returns a random Tile matching a selector
    /// </summary>
    /// <param name="selector">The selector</param>
    /// <returns>A random tile</returns>
    public DungeonTile SelectRandomTile(Func<DungeonTile, bool> selector){
        var candidateTiles = _flatTiles.Where(selector).ToList();
        var index = _random.Next(0, candidateTiles.Count);
        return candidateTiles[index];
    }
    
    #endregion

    /// <summary>
    /// Renders the Dungeon using tilemap
    /// </summary>
    /// <param name="floorTile">The tile to use for the floor</param>
    /// <param name="wallTile">The tile to use for the walls</param>
    public void Render(Tile floorTile, Tile wallTile){
        _tilemap.ClearAllTiles();
        _flatTiles.ForEach(tile => _tilemap.SetTile(tile.GetTilemapPosition(), tile.IsDungeon ? floorTile : wallTile));
    }

   
    

    
}
