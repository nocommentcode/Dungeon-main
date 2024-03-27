using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

/// <summary>
/// A context map for theif to balance moving towards closest treasure and not getting attacked by trolls
/// </summary>
public class TreasurePathContextMap{
    private List<Direction> _validDirections;
    private Dictionary<Direction, float> _intrestMap;
    private Dictionary<Direction, float> _dangerMap;
    private DungeonTile _currentTile;

    public TreasurePathContextMap(DungeonTile currentTile, DungeonTile treasureTile, List<Vector3> trollLocations){
        _currentTile = currentTile; 
        _validDirections = GetValidDirections();
        _dangerMap = BuildDangerMap(trollLocations);
        _intrestMap = BuildIntrestMap(treasureTile);
    }

    /// <summary>
    /// Returns the directions from current tile that are not walls
    /// </summary>
    private List<Direction> GetValidDirections(){
        return _currentTile.Neighbours.Where(tile => tile.Value.IsDungeon)
                                        .Select(tile => tile.Key)
                                        .ToList();
    }


    /// <summary>
    /// Builds the danger map from the current tile.
    /// Danger is inversily proportional to:
    ///     - Number of trolls in direction of travel relative to theif
    ///     - The distance of these trolls
    ///     - Only trolls within a certain distance threshold are concidered
    /// </summary>
    /// <param name="trollLocations">The locations of the trolls</param>
    /// <returns>The danger map</returns>
    private Dictionary<Direction, float> BuildDangerMap(List<Vector3> trollLocations){
        var tileLocation = _currentTile.GetGlobalPosition();
        float currX = tileLocation.x;
        float currY = tileLocation.y;

        // Returns the distance of a troll in a particular direction or 0 if not in that direction
        float TrollDistanceInDirection(Direction direction, Vector3 trollLocation){
            int IsInDirection(Func<float, bool> xSelector, Func<float, bool> ySelector){
                return xSelector(trollLocation.x) && ySelector(trollLocation.y) ? 1 : 0;
            }

            var distance = Vector3.Distance(trollLocation, tileLocation);

            // ignore trolls past a certain threshold
            if (distance > TheifSettings.TrollDistanceThreshold){
                return 0f;
            }

            return direction switch
            {
                Direction.UP => IsInDirection(x => true, y => y > currY) * distance,
                Direction.DOWN => IsInDirection(x => true, y => y < currY) * distance,
                Direction.RIGHT => IsInDirection(x => x > currX, y => true) * distance,
                Direction.LEFT => IsInDirection(x => x < currX, y => true) * distance,
                _ => 0,
            };
        }

        // Returns the sum of troll distance scores for all trolls
        float TrollDistancesInDirection(Direction direction) {
            return trollLocations.Aggregate(0f, (acc, troll) =>
            {
                var distance = TrollDistanceInDirection(direction, troll);
                if (distance == 0){
                    return acc;
                }

                // score is inversely proportional to distance
                return acc + 1 / distance;
            });
        }

        return _validDirections.ToDictionary(direction => direction, direction => TrollDistancesInDirection(direction));
    }

    /// <summary>
    /// Builds the intrest map of each direction.
    /// Intrest is a weighted sum:
    ///     - AStar path length from tile in that direction
    ///     - Birds eye distance from tile to treasure (to break ties)
    /// It is also inversely proportional 
    /// </summary>
    /// <param name="treasureTile">The desired treasure tile to move to</param>
    /// <returns>the intrest map</returns>
    private Dictionary<Direction, float> BuildIntrestMap(DungeonTile treasureTile){
        
        float GetIntrest(Direction direction){
            int pathLength =  AStarSearch.FindPath(_currentTile.Neighbours[direction], treasureTile).Count;
            float birdsEyeDist = Vector3.Distance(_currentTile.Neighbours[direction].GetGlobalPosition(), treasureTile.GetGlobalPosition());
            
            // intrest is inversely prop. to weighted sum of path length and birds eye distance
            return 1f / pathLength + TheifSettings.CrowDistWeight * (1f / birdsEyeDist);
        }

        return _validDirections.ToDictionary(direction => direction, direction => GetIntrest(direction));
    }

    /// <summary>
    /// Returns the tile to move to based on combination of danger and interest map
    /// Past a certain danger threshold, theif will not move in that direction
    /// </summary>
    /// <returns>tile to move to</returns>
    public DungeonTile GetBestTile(){
        float GetDirectionScore(Direction direction){
            var intrest = _intrestMap[direction];
            var danger = _dangerMap[direction];


            // if danger is greater than threshold, score is only danger score
            if(danger >= TheifSettings.MaxDangerThreshold){
                return -danger;
            }

            // otherwise merge the two scores
            var finalscore = intrest - TheifSettings.DangerWeight * danger;
            return finalscore;
            
        }

        var direction = _validDirections.OrderByDescending(GetDirectionScore).First();
        return _currentTile.Neighbours[direction];
    }
}
