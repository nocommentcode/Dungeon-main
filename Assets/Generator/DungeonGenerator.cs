using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Generates a Dungeon Grid.
/// </summary>
public class DungeonGenerator : MonoBehaviour {
    
    [Header("Tiles")]
    public Tile floorTile;
    public Tile wallTile;
    
    [Header("Size")]
    public int width;
    public int height;

    [Header("Cellular Automata")]
    [Range(1, 10000)]
    public int randomSeed = 10;
    
    [Range(0, 1)]
    public float initialDungeonDensity = 0.5f;

    [Range(1, 50)]
    public int iterations = 5;

    [Header("Game configuration")]
    [Range(1, 200)]
    public int treasureCount = 10;
    
    [Range(1, 10)]
    public int trollCount = 1;

    [Header("Prefabs")]
    public GameObject treasure;
    public GameObject theif;
    public GameObject troll;   
    public GameObject gameOverScreen;

    /// <summary>
    /// The Dungeon Grid instance that will be accesed by all other components of the game
    /// </summary>
    public DungeonGrid dungeonGrid;


    public void Start(){
        // get tilemap and random generator
        var tilemap = GetComponent<Tilemap>();
        var random = new System.Random(randomSeed);

        // generate dungeon
        dungeonGrid = GenerateDungeon(random, tilemap);
        
        // spawn treasure, theif and trolls
        dungeonGrid.TreasureTiles = SpawnTreasure(); 
        dungeonGrid.Theif = SpawnTheif();
        dungeonGrid.Trolls = SpawnTrolls();

        // render the dungeon
        dungeonGrid.Render(floorTile, wallTile);

    }

    public void Update(){
        // end game if theif is dead
        if(dungeonGrid.Theif == null){
            EndGame();
        }

        // end game if all the treasure has been collected
        if(dungeonGrid.TreasureTiles.Count == 0){
            EndGame();
        }
    } 

    /// <summary>
    /// Ends the game and displays the end screen
    /// </summary>
    private void EndGame() {
        Time.timeScale = 0f;
        var scoreText = gameOverScreen.transform.Find("Score").GetComponent<TMP_Text>();
        var treasureCollected = treasureCount - dungeonGrid.TreasureTiles.Count;
        scoreText.text = "Final Score: " + treasureCollected + "!";
        Instantiate(gameOverScreen, transform.parent.parent);        
        gameOverScreen.SetActive(true);
    }

    private DungeonGrid GenerateDungeon(System.Random random, Tilemap tilemap){
        var dungeonGrid = new DungeonGrid(width, height, random, tilemap);
        
        // PCG step 1: randomise tiles
        dungeonGrid.Randomise(initialDungeonDensity);
        
        // PCG step 2: cellular automata
        for (int i =0; i<iterations; i++){
            dungeonGrid.Iterate();
        }

        // PCG step 3: post process
        dungeonGrid.PostProcess();


        return dungeonGrid;
    }

    private GameObject SpawnTheif(){
        var spawnTile = dungeonGrid.SelectRandomTile(tile => tile.IsDungeon && !dungeonGrid.TreasureTiles.ContainsKey(tile));
        var theifObject = Instantiate(theif, spawnTile.GetGlobalPosition(), Quaternion.identity, transform);
        return theifObject;
    }
    
    private List<GameObject> SpawnTrolls(){
        var trolls = new List<GameObject>();
        for(int i=0; i<trollCount; i++){
            var spawnTile = dungeonGrid.SelectRandomTile(tile => tile.IsDungeon && !dungeonGrid.TreasureTiles.ContainsKey(tile) );
            var trollObject = Instantiate(troll, spawnTile.GetGlobalPosition(), Quaternion.identity, transform);
            trolls.Add(trollObject);
        }

        return trolls;
    }

    private Dictionary<DungeonTile, GameObject> SpawnTreasure(){
        var treasures = new Dictionary<DungeonTile, GameObject>();
        for(int i=0; i<treasureCount; i++){
            var treasureTile = dungeonGrid.SelectRandomTile(tile => tile.IsDungeon && !treasures.ContainsKey(tile));
            var treasureObject = Instantiate(treasure, treasureTile.GetGlobalPosition(), Quaternion.identity, transform );
            treasures[treasureTile] = treasureObject;
        }

        return treasures;
    }

    

    
    

    
}
