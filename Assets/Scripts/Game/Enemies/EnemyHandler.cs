using UnityEngine;
using System.Collections; // for enumerators
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.Android.Gradle;
using UnityEditor.Tilemaps;
//using System.Linq;

public class EnemyHandler : MonoBehaviour {
    public static EnemyHandler Instance { get; private set; }

    [Header("Reference table")]
    public EnemySpawnTable spawnTable;
    
    [Header("Enemies we're using right now")]
    public HashSet<EnemySpawnEntry> currentEnemies;
    public Theme currentTheme;
    public int maxEnemies = 5;
    public int checkIntervalS = 5;
    public int safeRadius = 5;

    [Header("Random Seed Stuff")]
    public bool useSeed = true;
    public int seed = 0;
    private System.Random pureRandom = new System.Random(); // new net random generator, not influenced by seed
    // influences WHICH SPECIFIC enemies generate and WHERE

    // dictionary spanning the dungeon's size which tells us where the enemies are
    public Dictionary<Vector2Int, GameObject> enemyMap;
        // this WILL need to be updated at every turn

    public Transform player;

    public GameObject enemiesLayer;
    public void SetPlayer(Transform plIn) {
        player = plIn;
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // destroy duplicates
        }
    }

    void Start() {
        
        enemyMap = new Dictionary<Vector2Int, GameObject>();
        currentEnemies = new HashSet<EnemySpawnEntry>();
        
        if (DungeonGenerator.Instance != null) {
            if (DungeonGenerator.Instance.GenerationDone) {
                //Debug.Log("GenerationDone true");
                BeginHandling();
            } else {
                DungeonGenerator.Instance.OnGenerationComplete += BeginHandling;
                //Debug.Log("waiting for system action");
            }
        } else {
            Debug.LogWarning("DungeonGenerator.Instance doesn't exist!");
        }

    }

    void BeginHandling() {
        //Debug.Log("begin handling");
        enemiesLayer = GameObject.FindGameObjectsWithTag("EnemiesLayer")[0];
        //walkableTiles = DungeonGenerator.Instance.walkable;
        EnemyThemeSelector();
        StartCoroutine(CheckPeriodically());
    }

    public void EnemyThemeSelector(Theme theme = Theme.Testing) {
        // filter
        foreach (EnemySpawnEntry entry in spawnTable.enemies) {
            //Debug.Log(entry+", "+spawnTable.enemies+", "+theme);
            if (entry.themes.Count == 0) {
                Debug.Log("themes list empty");
            } else if (entry.themes.Contains(theme)) {
                currentEnemies.Add(entry);
            }
        }
    }

    private IEnumerator CheckPeriodically() {
        // do this every x s
        WaitForSeconds wait = new WaitForSeconds(checkIntervalS);
        while (true) {
            yield return wait;
            CheckEnemiesMax();
        }
    }

    private void CheckEnemiesMax() {
        // if no of enemies present < max enemies, spawn one
        if (enemyMap.Count < maxEnemies) {  
            //Debug.Log(enemyMap.Count+" enemies on map and "+maxEnemies+" max enemies");
            SpawnEnemy(GenerateRandomEnemy());
        }

    }

    bool IsTooCloseToPlayer(Vector2Int pos) { // are you within the player's safe zone? don't spawn there!
        return (pos - Player.Instance.coordinates).sqrMagnitude <= safeRadius * safeRadius;
    }

    Vector2Int GetValidSpawnPosition() {
        int attempts = 0;

        while (attempts < 50) {
            Vector2Int randomPos = DungeonGenerator.Instance.walkableList[Random.Range(0, DungeonGenerator.Instance.walkableList.Count)];

            // check distance from player
            if (IsTooCloseToPlayer(randomPos)) {
                continue;
            }

            // check if an item exists here?

            // check if an enemy already exists here:
            if (enemyMap.ContainsKey(randomPos)) {
                // if the key is there then something is there already
                attempts++;
                continue; // restart while loop
            }
            //Debug.Log("spawning at walkable position: "+randomPos);
            return randomPos;
        }

        Debug.LogWarning("gave up trying to find walkable tile");
        return Vector2Int.zero;
    }

    public SpawnRank GenerateRandomEnemyRank() {
        Debug.Log("attempting to generate random enemy");
        float genRank = pureRandom.Next(0, 1);
        SpawnRank rank;

        if (genRank < 0.01) {
            rank = SpawnRank.Mythical; 
        } else if (genRank < 0.1) {
            rank = SpawnRank.Elite;
        } else if (genRank < 0.4) {
            rank = SpawnRank.Rare;
        } else {
            rank = SpawnRank.Common;
        }
        return rank;
    }

    public EnemySpawnEntry GenerateRandomEnemy() {

        // Group enemies by rank first
        Dictionary<SpawnRank, List<EnemySpawnEntry>> rankGroups = new Dictionary<SpawnRank, List<EnemySpawnEntry>>();

        foreach (var entry in currentEnemies) {
            if (!rankGroups.ContainsKey(entry.spawnRank))
                rankGroups[entry.spawnRank] = new List<EnemySpawnEntry>();
            rankGroups[entry.spawnRank].Add(entry);
        }

        if (rankGroups.Count == 0)
            return null;

        // Pick a random valid rank from the keys
        //var validRanks = rankGroups.Keys.ToList(); // use LINQ
        List<SpawnRank> validRanks = new List<SpawnRank>();
        foreach (var key in rankGroups.Keys) {
            validRanks.Add(key);
        }
        var randomRank = validRanks[pureRandom.Next(0, validRanks.Count)];

        // Pick a random enemy from that rank
        var pool = rankGroups[randomRank];
        return pool[pureRandom.Next(0, pool.Count)];
        
        //return enemy;
    }

    void SpawnEnemy(EnemySpawnEntry enemyEntry) {
        Vector2Int spawnAt = GetValidSpawnPosition();
        if (spawnAt == Vector2Int.zero) {
            return; // failed to gen position
        } else {

            // EVERYTHING SPAWNS IN WALLS IDK WHAT TO DO
            Vector3Int sp3 = new Vector3Int(spawnAt.x, spawnAt.y, 0);
            //Tilemap tm = DungeonGenerator.Instance.floorTilemap;
            // Vector3 worldPos = tm.CellToWorld(sp3) + tm.cellSize / 2f;
            //Vector3 worldPos = Cell.GridToWorldCentered(spawnAt);
            //Vector3 worldPos = Cell.GridToWorldCenteredUnity(spawnAt);
            //Vector3 worldPos = DungeonGenerator.Instance.floorTilemap.GetCellCenterWorld(new Vector3Int(spawnAt.x, spawnAt.y, 0));

            // screw it, my tiles are 1x1x1
            Vector3 worldPos = new Vector3(spawnAt.x, spawnAt.y, 0);
            GameObject thing = Instantiate(enemyEntry.enemyPrefab, worldPos, Quaternion.identity, enemiesLayer.transform);
            Debug.Log($"enemy spawned at {spawnAt}, world pos {worldPos}");
        }
    }

    public void EnemyMoved(Vector2Int oldPos, Vector2Int newPos) {
        if (!enemyMap.ContainsKey(oldPos)) {
            Debug.LogWarning($"EnemyMoved: No enemy found at {oldPos} — cannot move to {newPos}.");
            return;
        }

        GameObject enemy = enemyMap[oldPos];
        enemyMap.Remove(oldPos);
        enemyMap[newPos] = enemy;
        Debug.Log($"enemy moved from {oldPos} to {newPos}");
    }

    public void EnemyDied(Vector2Int atPos) {
        enemyMap.Remove(atPos);
    }

    public bool IsOccupied(Vector2Int position) {
        return enemyMap.ContainsKey(position);
    }

    public GameObject GetEnemyAt(Vector2Int position) {
        var isThere = IsOccupied(position);
        Debug.Log($"Checking if there is an enemy at {position}: {isThere}");

        if (!enemyMap.ContainsKey(position)) {
            Debug.LogWarning($"EnemyMoved: No enemy found at {position}.");
        }
        enemyMap.TryGetValue(position, out GameObject e);
        return e;
    }

    //public void RegisterEnemy(Vector2Int pos, Enemy enemy) {
    public void RegisterEnemy(Vector2Int pos, GameObject enemy) {
        enemyMap[pos] = enemy;
        //Debug.Log("registering enemy at "+ pos);
    }

    public void UnregisterEnemy(Vector2Int pos) {
        if (enemyMap.ContainsKey(pos)) enemyMap.Remove(pos);
    }

}

