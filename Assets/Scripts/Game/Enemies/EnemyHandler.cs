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
    //public Dictionary<Vector2Int, Enemy> enemyMap;
    public Dictionary<Vector2Int, GameObject> enemyMap;
        // this WILL need to be updated at every turn

    //public List<Vector2Int> walkableTiles = new List<Vector2Int>();
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

        // onstart: Decide upon a pool of enemies (hashset/array currentEnemies) from reference table
        // Check every x s
        // Generate a random one based off of rarity
            // use cumulative rarity
        // Find a random position to put it in (that isn't occupied)
        // Generate it

    // Subscribe to the signal that tells you when dungeon generation is complete
    // void OnEnable() {
    //     if (DungeonGenerator.Instance != null)
    //         DungeonGenerator.Instance.OnGenerationComplete += BeginHandling;
    // }

    // void OnDisable() {
    //     if (DungeonGenerator.Instance != null)
    //         DungeonGenerator.Instance.OnGenerationComplete -= BeginHandling;
    // }

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

    // public void SpawnEnemies() {
    //     // for (int i = 0; i < maxEnemies; i++) {
    //     //     Vector2Int spawnPos = GetValidSpawnPosition();
    //     //     Vector3 worldPos = new Vector3(spawnPos.x, spawnPos.y, 0);

    //     //     //GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
            
    //     //     }
    // }

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
            Debug.Log("walkable position: "+randomPos);
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
            Vector3Int sp3 = new Vector3Int(spawnAt.x, spawnAt.y, 0);
            Tilemap tm = DungeonGenerator.Instance.floorTilemap;
            Vector3 worldPos = tm.CellToWorld(sp3) + tm.cellSize / 2f;
            //Vector3 worldPos = Cell.GridToWorldCentered(spawnAt);
            GameObject thing = Instantiate(enemyEntry.enemyPrefab, worldPos, Quaternion.identity, enemiesLayer.transform);
            //Debug.Log("pop, i made an enemy");
            //RegisterEnemy(spawnAt, thing.GetComponent<Enemy>());
            //RegisterEnemy(spawnAt, thing); // decided to register at its startup instead
        }
    }

    //public void EnemySpawned(Vector2Int pos, Enemy enemy) {
    // public void EnemySpawned(Vector2Int pos, GameObject enemy) {
    //     enemyMap[pos] = enemy;
    // }

    public void EnemyMoved(Vector2Int oldPos, Vector2Int newPos) {
        if (!enemyMap.ContainsKey(oldPos)) {
            Debug.LogWarning($"EnemyMoved: No enemy found at {oldPos} — cannot move to {newPos}.");
            return;
        }

        GameObject enemy = enemyMap[oldPos];
        enemyMap.Remove(oldPos);
        enemyMap[newPos] = enemy;
    }

    public void EnemyDied(Vector2Int atPos) {
        enemyMap.Remove(atPos);
    }

    public bool IsOccupied(Vector2Int position) {
        return enemyMap.ContainsKey(position);
    }

    //public Enemy GetEnemyAt(Vector2Int position) {
        //enemyMap.TryGetValue(position, out Enemy e);
    public GameObject GetEnemyAt(Vector2Int position) {
        if (!enemyMap.ContainsKey(position)) {
            Debug.LogWarning($"EnemyMoved: No enemy found at {position}.");
        }
        enemyMap.TryGetValue(position, out GameObject e);
        return e;
    }

    //public void RegisterEnemy(Vector2Int pos, Enemy enemy) {
    public void RegisterEnemy(Vector2Int pos, GameObject enemy) {
        enemyMap[pos] = enemy;
        Debug.Log("registering enemy at "+ pos);
    }

    public void UnregisterEnemy(Vector2Int pos) {
        if (enemyMap.ContainsKey(pos)) enemyMap.Remove(pos);
    }

    // accidentally made EnemyMovedV2
    // public void UpdateEnemyPosition(Vector2Int oldPos, Vector2Int newPos, Enemy enemy) {
    //     UnregisterEnemy(oldPos);
    //     RegisterEnemy(newPos, enemy);
    // }

}

