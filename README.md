# ECS 7016 Project - Troll Lair

## The game

### Description
The Game is set in a Troll Lair. Trolls live in this dungeon and protect their treasure. A sneaky Theif has managed to sneak into the dungeon and wants to steall all their treasure.

If the theif manages to steal all the treasure without being attacked by trolls, it wins the game. But it needs to be carefull, trolls guard their treasure with great integrity.

The theif is nible and agile whilst the Trolls are slow and sluggish, but there are a lot of trolls and only 1 theif...
The theif must use it's agility to duke and dodge the trolls, lure them away from the treasure and outsmart them in order to win.

### Video
A video of the game in action, outlining the procedural level generation and the agents' AI in action can be found [here](https://drive.google.com/file/d/1jQsD5R1FZFYiYPAPR-wYtj34nfsoLzX3/view?usp=sharing).


## Design

Key AI methods used:
- Behaviour Trees
- Context Maps
- AStar search
- Cellular automata

### Dungeon
The Dungeon is represented in two data structures:

<u>DungeonTile:</u>

A `DungeonTile` represents the tiles in the dungeon. 
They store their neighbours and can provide their position on the Unity `Tilemap` or the wold. 
Tiles will record their 3x3 moor neighbours as well as the neighbours that can be accesed by moving up, down, left and right from them.

<u>DungeonGrid</u>

The DungeonGrid is the main data structure of the dungeon. It is a collection of `DungeonTile`s.
The Dungeon Grid initially instantiates all of the tiles, and registers all of the neighbours.

It contains methods to perform Cellular Automata:
- `Randomise` randomises the tiles at the beginning
- `Iterate` performs 1 round of Cellular Automata.

It also contains a `PostProcess` method which ensures that all dungeon tiles are accessible, preventing any disconnected clusters of dungeon occuring.
This is required to prevent spawning treasure in a un-accessable place.
This is done by iteratively building clusters of connected tiles, and then connecting each cluster to the largest cluster by replacing walls with dungeon in an x and y axis tunnel.

It contains a `Render` method which will render the dungeon on a Unity `Tilemap`.
It also contains tile access methods for other elements of the game to access tiles:
- `FirstOrDefault` returns the first tile matching a predicate or null
- `SelectRandomTile` returns a random tile matching a predicate.


The dungeon grid also contains data that is required to be accessed by other entities in the game:
- TreasureTiles: A Collection of Treasure `GameObject`s and the tiles where they are found
- Theif: The theif `GameObject` 
- Trolls: A List of troll `GameObject` 

<u>DungeonGenerator</u>

`DungeonGenerator` is a MonoBehavior script that is attached to the unity `Tilemap`.
It contains the following parameters accesible from the editor:

- Tiles
    - FloorTile: the Unity `Tile` asset to use to render the dungeon floor.
    - WallTile: the Unity `Tile` asset to use to render the dungeon walls.
- Size
    - Width: the width of the dungeon
    - Height: the height of the dungeon
- Cellular Automata
    - RandomSeed: the seed for PSG generation
    - Initial Dungeon Density: the proportion of dungeon tiles to set when randomising the dungeon before cellular automata.
    - Iterations: the number of Cellular Automata iterations to perform
- Game Configuration
    - Treasure Count: the amount of treasure to spawn
    - Troll Count: the amount of trolls to spawn
- Prefabs
    - Treasure: the treasure `Prefab`
    - Theif: the Theif `Prefab`
    - Troll: the Troll `Prefab`
    - Game Over Screen: The Game over canvas `Prefab`

In its `Startup` method, the Generator will:

1) Instantiate a DungeonGrid instance.
2) Randomise the grid
3) Peform Cellular Automata
4) Post-Process the grid
5) Spawn Treasure
6) Spawn the Theif
7) Spawn Trolls
8) Render the dungeon

In its `Update` method, the Generator will check for the end of game conditions and display the GameOver screen accordingly. 
This should probablity be handled by a GameManager, but this also works.


### Agents

Agents are controlled by NPBehave's Behaviour Trees. 

<u>Behaviors</u>

A Behaviour is a Sequence with the following nodes:
1) A Condition Node
2) A Wait node

The agent's behaviour tree is populated with a single Selector Node at the Root, which will select one of the agent's behaviours.

Each Agent contains a collection of behaviours. At each step, the behavior tree will execute the first behaviour that meets its conditions.

Note that due to this design, the order of these behaviours is very important, as only 1 behaviour will be executed at each timestep.

This pattern is simplistic but allows for the desired functionality of this simple game.
The wait node is used to have the Theif and Trolls operate at different speeds. This is to make it slightly easier for the Theif to beat the game.

<u>Behavior Factories</u>

The class `ABehaviourFactory` is an abstract class that implements all behaviours.

The `Make` method will return a new Sequence to be executed.
Children must implement:
- Delay: the amount of time to wait
- Condition() - wether to perform the behavior or not
- PerformAction() - performs the action

The class `AMoveBehaviourFactory` is an abstract Behavior that results in the agent moving to a `DungeonTile`.
Children can use this method to path find to a desired Tile.

Children must implement:
- Delay: the amount of time to wait
- Condition() - wether to perform the behavior or not
- GetDestTile() - returns the tile to move to

<u> Abstract Agent </u>

The class `ADungeonAgent` is the base class for all agents in the game, it creates and populates the behaviour tree.
Children only need to implement the `GetBehaviours` method, which must return a list of behaviour factories. 

<u>AStarSearch</u>

`AStarSearch` is a static class with a custom implementation of the AStar algorithm for use by behaviours to traverse the dungeon.

#### Theif

The `Theif` class implements the `ADungeonAgent` class with the following behaviours:

1) Pickup treasure behaviour: Theif will pickup treasure if it is on the same tile as treasure.
2) Move to treasure behaviour: Theif will move towards a treasure tile if there is treasure remaining.

The theif's `MoveToTreasureBeahviourFactory` determines movement via the context map `TreasurePathContextMap` which balances the dangers and intresets of the theif.

The danger map score is inversily proportional to:
- Number of trolls in direction of travel relative to theif
- The distance of these trolls
- Only trolls within a certain distance threshold are concidered

The intrest map is a weighted sum of:
- AStar path length from tile in that direction
- Birds eye distance from tile to treasure (in order to break ties where two tiles have same path length)


#### Troll
The `Troll` class implements the `ADungeonAgent` class with the following behaviours:

Each Troll is assigned a particular treasure to protect. 

1) Attack Theif Behaviour: The Troll will attack the Theif if they are on the same tile (this will end the game).
2) Move toward Theif Behaviour: The Troll will move towards the theif if it is within a certain range.
3) Move to assigned Treasure Behavior: The troll will move towards its assigned treasure if it is too far from it.
4) Patrol assigend Treasure Beahvior: The troll will randomly move around its assigned treasure.

## References
All code was written by myself with inspiration drawn from the lab materials and online forums, with the exception of the `PriorityQueue` class, which was implemented with the help of ChatGPT (version 3.5) as the target DotNet framework does not include a PriorityQueue which I needed for AStar.

### Imported Packages
- NPBehave: for behaviour trees.
- 2DBasicsPlaformerPack: for the treasure prefab.
- Tilesetter: for the dungeon and wall tiles.
