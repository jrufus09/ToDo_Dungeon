using UnityEngine;
using UnityEngine.Tilemaps;
using AStar;
using System;
using System.Threading.Tasks;

public static class Cell {

    // helper class for myself because I gotta do everything around here

    // transform.position in, cell coordinates out.
    public static Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3Int cell = DungeonGenerator.gridTilemap.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }
    // for easy tracking in inspector,
    // public Vector2Int cellCoordinates;
    // void Update() {
    // cellCoordinates = Cell.WorldToGrid(transform.position);
    //}

    // and this is the reverse
    // public static Vector3 GridToWorld(Vector2Int gridPos) {
    //     return DungeonGenerator.gridTilemap.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    // }
    // centre on tile / align perfectly
    public static Vector3 GridToWorldCentered(Vector2Int gridPos) {
        Vector3 world = DungeonGenerator.gridTilemap.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        return world + DungeonGenerator.gridTilemap.cellSize / 2f;
    }

    // pathfinding returns (int,int) coordinates in an array which needs conversion to cell, so
    // transform.position, transform position, i'll find the grid myself because it's static in DunGen

    // Vector3[] path = await FindPathWorldPositions(startPos, goalPos); --> from another async method

    // ----- from a non-async method: --------
    // FindPathWorldPositions(start, goal).ContinueWith(task => {
    // Vector3[] path = task.Result;
    // // Now use path...
    // });
    public static async Task<Vector3[]> FindPathWorldPositions(Vector3 start, Vector3 end) { //return Vector3[]
        // takes world positions in and out, still needs grid conversion for pathfinder

        Vector2Int startC = WorldToGrid(start);
        Vector2Int endC = WorldToGrid(end);

        (int, int)[] path;
        // sync would have been fine but im scared of performance issues
        //path = AStarPathfinding.GeneratePathSync(startC.x, startC.y, endC.x, endC.y, DungeonGenerator.walkableMap);
        path = await AStarPathfinding.GeneratePath(startC.x, startC.y, endC.x, endC.y, DungeonGenerator.walkableMap);

        Vector3[] pathOut = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++) {
            Vector2Int pathNode = new Vector2Int(path[i].Item1, path[i].Item2);
            pathOut[i] = GridToWorldCentered(pathNode);
        }

        return pathOut;
    }

    
}