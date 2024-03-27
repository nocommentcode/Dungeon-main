using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class AStarSearch{

    /// <summary>
    /// Runs A star and returns best tile to move to
    /// </summary>
    /// <param name="desitination">The tile to get to</param>
    /// <returns>The next tile to move to</returns> 
    public static DungeonTile NextTile(DungeonTile origin, DungeonTile desitination){
        var path = FindPath(origin, desitination);
        return path[1];
    }

    /// <summary>
    /// A Star Implementation
    /// </summary>
    /// <param name="desitination">The tile to reach</param>
    /// <returns>Path to take</returns>
    public static List<DungeonTile> FindPath(DungeonTile origin, DungeonTile desitination){
        // measures the distance bewteen two tiles (fitness function)
        float Distance(DungeonTile from, DungeonTile to){
            return Vector3Int.Distance(from.GetTilemapPosition(), to.GetTilemapPosition());
        }

        // reconstructs the path from detination
        List<DungeonTile> ReconstructPath(Dictionary<DungeonTile, DungeonTile> cameFrom, DungeonTile dest)
        {
            var path = new List<DungeonTile>();
            while (cameFrom.ContainsKey(dest))
            {
                path.Add(dest);
                dest = cameFrom[dest];
                if(dest == null){
                    path.Reverse();
                    return path;
                }
            }
            return null;
        }
        
        // initialise data structures
        var frontier = new PriorityQueue<DungeonTile, float>();
        var cameFrom = new Dictionary<DungeonTile, DungeonTile>();
        var costSoFar = new Dictionary<DungeonTile, float>();

        // add starting position
        frontier.Enqueue(origin, Distance(origin, desitination));
        cameFrom[origin] = null;
        costSoFar[origin] = 0;


        while(!frontier.IsEmpty()){
            var currentTile = frontier.Dequeue();

            // goal test
            if(currentTile == desitination){
                return ReconstructPath(cameFrom, desitination);
            }

            // add neighbours with cost
            foreach(var neighbour in currentTile.Neighbours.Values.Where(tile => tile.IsDungeon).ToList()){
                float cost = costSoFar[currentTile] + 1;
                if(!costSoFar.ContainsKey(neighbour) || cost < costSoFar[neighbour]){
                    costSoFar[neighbour] = cost;
                    cameFrom[neighbour] = currentTile;
                    frontier.Enqueue(neighbour, cost  + Distance(neighbour, desitination));
                }
            }

        }
        
        return null;
    }
    
}
