using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections; // for enumerators
using Unity.AI.Navigation;
using NavMeshPlus.Components;


public class DungeonGenerator : MonoBehaviour {
    public static DungeonGenerator Instance { get; private set; }
    public event System.Action OnGenerationComplete; // like signals but for C#
    public bool GenerationDone;

    [Header("Map Settings")]
    public int width = 50;
    public int height = 50;
    public int roomCount = 8;
    public Vector2Int roomMinSize = new Vector2Int(4, 4);
    public Vector2Int roomMaxSize = new Vector2Int(10, 10);

    [Header("Tiles")]
    public Tilemap gridTilemap;
    public Tilemap wallTilemap;
    public Tilemap floorTilemap;
    public TileBase gridTile;
    public TileBase wallTile;
    public TileBase floorTile;

    [Header("Random Seed")]
    public bool useSeed = true;
    public int seed = 0;

    [Header("Player")]
    public GameObject playerPrefab;

    private int[,] map;
    private List<RectInt> rooms;
    public enum TileType {
        Empty,
        Floor,
        Wall
    }
    TileType[,] dungeonMap;
    public NavMeshPlus.Components.NavMeshSurface surface;
    public List<Vector2Int> walkable;

    void Start() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // DESTROY duplicates
        }

        dungeonMap = new TileType[width, height];
        //surface = GetComponent<NavMeshSurface>();
        surface = GetComponent<NavMeshPlus.Components.NavMeshSurface>();
        surface.BuildNavMeshAsync();

        if (useSeed) {
            Random.InitState(seed);
            // set unity's randomness generator
        }

        Generate();
        DrawMap();
        SpawnPlayer();
    }

    void Generate() {
        map = new int[width, height];
        rooms = new List<RectInt>();

        for (int i = 0; i < roomCount; i++) {
            int w = Random.Range(roomMinSize.x, roomMaxSize.x + 1);
            int h = Random.Range(roomMinSize.y, roomMaxSize.y + 1);
            int x = Random.Range(1, width - w - 1);
            int y = Random.Range(1, height - h - 1);

            RectInt newRoom = new RectInt(x, y, w, h);

            bool overlaps = false;
            foreach (var room in rooms) {
                if (newRoom.Overlaps(room)) {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps) {
                rooms.Add(newRoom);
                CarveRoom(newRoom);

                if (rooms.Count > 1) {
                    Vector2Int prevCenter = RoomCenter(rooms[rooms.Count - 2]);
                    Vector2Int newCenter = RoomCenter(newRoom);
                    CarveCorridor(prevCenter, newCenter);
                }
            }
        }

        AddWalls();
    }

    void CarveRoom(RectInt room) {
        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++) {
                map[x, y] = 1;
                dungeonMap[x, y] = TileType.Floor;
            }
        }
    }

    void CarveCorridor(Vector2Int from, Vector2Int to) {
        if (Random.value < 0.5f) {
            CarveHorizontal(from.x, to.x, from.y);
            CarveVertical(from.y, to.y, to.x);
        }
        else {
            CarveVertical(from.y, to.y, from.x);
            CarveHorizontal(from.x, to.x, to.y);
        }
    }

    void CarveHorizontal(int xStart, int xEnd, int y) {
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++) {
            map[x, y] = 1;
            dungeonMap[x, y] = TileType.Floor;
        }
    }

    void CarveVertical(int yStart, int yEnd, int x) {
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++) {
            map[x, y] = 1;
            dungeonMap[x, y] = TileType.Floor;
        }
    }

    void AddWalls() {
        for (int x = 1; x < width - 1; x++) {
            for (int y = 1; y < height - 1; y++) {
                if (map[x, y] == 0) {
                    // if any neighbor is floor, place wall
                    if (HasAdjacentFloor(x, y))
                        map[x, y] = 2;
                        dungeonMap[x, y] = TileType.Wall;
                }
            }
        }
    }

    bool HasAdjacentFloor(int x, int y) {
        return map[x + 1, y] == 1 || map[x - 1, y] == 1 ||
               map[x, y + 1] == 1 || map[x, y - 1] == 1;
    }

    // void MapTranslation() { // numbers to enum
    //     for (int x = 0; x < width; x++) {
    //         for (int y = 0; y < height; y++) {
    //             if (map[x, y] == 1)
    //                 dungeonMap[x, y] = TileType.Floor;
    //             else if (map[x, y] == 2)
    //                 dungeonMap[x, y] = TileType.Wall;
    //         }
    //     }
    // } // this is just integrated into the generator

    void DrawMap() {
        wallTilemap.ClearAllTiles();
        gridTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();

        // for old system with 1s and 2s
        // for (int x = 0; x < width; x++) {
        //     for (int y = 0; y < height; y++) {
        //         if (map[x, y] == 1)
        //             gridTilemap.SetTile(new Vector3Int(x, y, 0), gridTile);
        //         else if (map[x, y] == 2)
        //             wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
        //     }
        // }
        walkable = new List<Vector2Int>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (dungeonMap[x, y] == TileType.Floor) {
                    gridTilemap.SetTile(new Vector3Int(x, y, 0), gridTile);
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);

                    //EnemyHandler.Instance.walkableTiles.Add(new Vector2Int(x, y));
                    walkable.Add(new Vector2Int(x, y));
                }
                else if (dungeonMap[x, y] == TileType.Wall) {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
        StartCoroutine(BakeAfterDelay());
    }

    IEnumerator BakeAfterDelay() {
        yield return null; // wait one frame for tilemap to finalize
        Physics2D.SyncTransforms(); // ensure colliders are registered
        yield return new WaitForFixedUpdate(); // ensure physics runs
        //surface.BuildNavMesh();
        //surface.BuildNavMeshAsync();
        surface.UpdateNavMesh(surface.navMeshData);
    }

    void SpawnPlayer() {
        if (playerPrefab == null) {
            //playerPrefab = FindAnyObjectByType<PlayerMovement>().gameObject;
            Debug.LogWarning("please set player prefab.");
        }
        
        if (rooms.Count == 0) {
            Debug.LogWarning("set to generate 0 rooms.");
            return;
        }

        Vector2Int spawnPos = RoomCenter(rooms[0]);
        Vector3Int sp3 = new Vector3Int(spawnPos.x, spawnPos.y, 0);
        // this was converting to make the player inbetween four tiles (not ideal)
        //Vector3 worldPos = gridTilemap.CellToWorld(new Vector3Int(spawnPos.x, spawnPos.y, 0));
        Vector3 worldPos = gridTilemap.CellToWorld(sp3) + gridTilemap.cellSize / 2f;
        //Instantiate(playerPrefab, worldPos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);

        Vector2 spawnAt;
        spawnAt.x = worldPos.x;
        spawnAt.y = worldPos.y;
        //Debug.Log(worldPos);
        //Debug.Log(spawnAt);

        Instantiate(playerPrefab, worldPos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        playerPrefab.GetComponent<PlayerMovement>().InitiateAt(spawnAt);
        //Debug.Log("player spawned in successfully");

        OnGenerationComplete?.Invoke();
        GenerationDone = true;
    }

    Vector2Int RoomCenter(RectInt room) {
        return new Vector2Int(room.xMin + room.width / 2, room.yMin + room.height / 2);
    }

    // Vector2Int PosToWorld(RectInt room) {
    //     return new Vector2Int(room.xMin + room.width / 2, room.yMin + room.height / 2);
    // }

}
