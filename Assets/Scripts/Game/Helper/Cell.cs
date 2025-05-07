using UnityEngine;
using UnityEngine.Tilemaps;
using AStar;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using System.Text;

public static class Cell {

    // helper class for myself because I gotta do everything around here

    // pass these in after generation!
    public static Tilemap gridTilemap;
    public static Tilemap floorTilemap;
    public static int dungeonHeight;

    // transform.position in, cell coordinates out.
    public static Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3Int cell = gridTilemap.WorldToCell(worldPos);
        // we add that offset to make it centred on grid
        //return new Vector2Int(cell.x, cell.y);
        //return FlipY(new Vector2Int(cell.x, cell.y));

        return new Vector2Int(cell.x, dungeonHeight - 1 - cell.y);
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
        //Vector3 worldPos = DungeonGenerator.gridTilemap.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        // Vector3Int gp3 = new Vector3Int(gridPos.x, gridPos.y, 0);
        // Vector3 worldPos = DungeonGenerator.gridTilemap.GetCellCenterWorld(gp3);
        // //return worldPos + DungeonGenerator.gridTilemap.cellSize / 2f;
        // return worldPos;
        Vector2Int flipped = FlipY(gridPos);
        Vector3 world = gridTilemap.GetCellCenterWorld(new Vector3Int(flipped.x, flipped.y, 0));
        return world;
    }

    // pathfinding returns (int,int) coordinates in an array which needs conversion to cell, so
    // transform.position, transform position, i'll find the grid myself because it's static in DunGen

    // Vector3[] path = await FindPathWorldPositions(startPos, goalPos); --> from another async method

    // ----- from a non-async method: --------
    // FindPathWorldPositions(start, goal).ContinueWith(task => {
    // Vector3[] path = task.Result;
    // // Now use path...
    // });
    //public static async Task<Vector3[]> FindPathWorldPositions(Vector3 start, Vector3 end) { // async
    public static Vector3[] FindPathWorldPositions(Vector3 start, Vector3 end, bool[,] walkableMap)  {
        // takes world positions in and out, still needs grid conversion for pathfinder

        // convert to grid for pathfinder
        Vector2Int startC = WorldToGrid(start);
        Vector2Int endC = WorldToGrid(end);

        (int, int)[] path;
        // sync would have been fine but im scared of performance issues
        //AStarPathfinding.
        path = AStarPathfinding.GeneratePathSync(startC.x, startC.y, endC.x, endC.y, walkableMap);
        //path = await AStarPathfinding.GeneratePath(startC.x, startC.y, endC.x, endC.y, DungeonGenerator.walkableMap);

        Vector3[] pathOut = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++) {
            Vector2Int pathNode = new Vector2Int(path[i].Item1, path[i].Item2);
            pathOut[i] = GridToWorldCentered(pathNode);
        }

        return pathOut;
    }

    public static Vector2Int[] PathToPlayerVec2(Vector3 posIn, bool[,] walkableMap) { // output is grid coordinates
        //PrintWalkableMap(walkableMap);

        // convert to grid for pathfinder
        Vector2Int enemyPos = WorldToGrid(posIn);
        Vector2Int playerPos = WorldToGrid(Player.Instance.transform.position);

        // I learned the algorithm potentially takes in y,x omg
        bool[,] map = TransposeBoolMap(walkableMap); // this is JUST for the pathfind algo btw
        //PrintWalkableMap(map);

        (int, int)[] path;
        DebugPrintMapWithPositions(map, enemyPos, playerPos);
        path = AStarPathfinding.GeneratePathSync(enemyPos.x, enemyPos.y, playerPos.x, playerPos.y, map);

        // Debug.Log($"Enemy Grid Pos: {enemyPos} | Player Grid Pos: {playerPos}");
        // Debug.Log($"Enemy Walkable? {map[enemyPos.y, enemyPos.x]} | Player Walkable? {map[playerPos.y, playerPos.x]}");

        // Debug.Log(path[0] + ", " + path[1] + ", " + path[2]);
        // if (!walkableMap[enemyPos.x, enemyPos.y])
        //     Debug.LogWarning("Enemy starting position is not walkable!");
        // if (!walkableMap[playerPos.x, playerPos.y])
        //     Debug.LogWarning("Player target position is not walkable!");


        // convert back to Vec2Int
        Vector2Int[] pathOut = new Vector2Int[path.Length];
        for (int i = 0; i < path.Length; i++) {
            pathOut[i] = new Vector2Int(path[i].Item1, path[i].Item2);
        }

        return pathOut;
    }

    public static bool[,] TransposeBoolMap(bool[,] original) {
        int height = original.GetLength(0); // rows (y)
        int width = original.GetLength(1);  // columns (x)

        bool[,] flipped = new bool[height, width];

        for (int y = 0; y < height; y++) {
            int flippedY = height - 1 - y;
            for (int x = 0; x < width; x++) {
                flipped[flippedY, x] = original[y, x];
            }
        }

        return flipped;
    }

    public static Vector2Int FlipY(Vector2Int input) {
        return new Vector2Int(input.x, dungeonHeight - 1 - input.y);
    }

    // public static bool[,] FlipWalkableMapY(bool[,] original) {
    //     int width = original.GetLength(0);
    //     int height = original.GetLength(1);

    //     bool[,] flipped = new bool[width, height];

    //     for (int x = 0; x < width; x++) {
    //         for (int y = 0; y < height; y++) {
    //             flipped[x, y] = original[x, height - 1 - y];
    //         }
    //     }
    //     return flipped;
    // }

    public static void PrintWalkableMap(bool[,] mapIn) {
        StringBuilder sb = new StringBuilder();
        int width = mapIn.GetLength(0);
        int height = mapIn.GetLength(1);

        for (int y = height - 1; y >= 0; y--) // Top to bottom
        {
            for (int x = 0; x < width; x++){
                sb.Append(mapIn[x, y] ? "." : "#");
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
    

    public static void DebugPrintMapWithPositions(bool[,] map, Vector2Int enemyPos, Vector2Int playerPos) {
        int height = map.GetLength(0);
        int width = map.GetLength(1);
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++)
            {
                if (x == enemyPos.x && y == enemyPos.y)
                    sb.Append('E');
                else if (x == playerPos.x && y == playerPos.y)
                    sb.Append('P');
                else
                    sb.Append(map[y, x] ? '.' : '#');
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }

}