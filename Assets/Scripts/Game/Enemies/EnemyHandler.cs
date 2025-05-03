using UnityEngine;
using System.Collections; // for enumerators
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.Android.Gradle;
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

    [Header("Random Seed Stuff")]
    public bool useSeed = true;
    public int seed = 0;
    private System.Random pureRandom = new System.Random(); // new net random generator, not influenced by seed
    // influences WHICH SPECIFIC enemies generate and WHERE

    // dictionary spanning the dungeon's size which tells us where the enemies are
    public Dictionary<Vector2Int, Enemy> enemyMap;
        // this WILL need to be updated at every turn

    public List<Vector2Int> walkableTiles = new List<Vector2Int>();
    public Transform player;

    public GameObject enemiesLayer;
    public void SetPlayer(Transform plIn) {
        player = plIn;
    }

    void Start() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // destroy duplicates
        }
        
        enemyMap = new Dictionary<Vector2Int, Enemy>();
        currentEnemies = new HashSet<EnemySpawnEntry>();
        
        if (DungeonGenerator.Instance != null) {
            if (DungeonGenerator.Instance.GenerationDone) {
                Debug.Log("GenerationDone true");
                BeginHandling();
            } else {
                DungeonGenerator.Instance.OnGenerationComplete += BeginHandling;
                Debug.Log("waiting for system action");
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
        Debug.Log("begin handling");
        enemiesLayer = GameObject.FindGameObjectsWithTag("EnemiesLayer")[0];
        walkableTiles = DungeonGenerator.Instance.walkable;
        EnemyThemeSelector();
        StartCoroutine(CheckPeriodically());
    }

    public void EnemyThemeSelector(Theme theme = Theme.Testing) {

        // filter
        foreach (EnemySpawnEntry entry in spawnTable.enemies) {
            Debug.Log(entry+", "+spawnTable.enemies+", "+theme);
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
            Debug.Log(enemyMap.Count+" enemies on map and "+maxEnemies+" max enemies");
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

    Vector2Int GetValidSpawnPosition() {
        int attempts = 0;
        while (attempts < 50) {
            Vector2Int randomPos = walkableTiles[Random.Range(0, walkableTiles.Count)];

            // check distance from player?
            // check if an item exists here?

            // check if an enemy already exists here:
            if (enemyMap.ContainsKey(randomPos)) {
                // if the key is there then something is there already
                attempts++;
                continue; // restart while loop
            }

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
        //EnemySpawnEntry enemy = null;
        // while (enemy == null) {
        //     SpawnRank rank = GenerateRandomEnemyRank();

        //     // go through and find all enemies of that rank
        //     List<EnemySpawnEntry> pool = new List<EnemySpawnEntry>();
        //     foreach (EnemySpawnEntry thing in currentEnemies) {
        //         if (thing.spawnRank == rank) {
        //             pool.Add(thing);
        //         }
        //     }
        //     if (pool.Count == 0) { // empty, meaning none of that rank exists
        //         continue; // restart while loop / regenerate rank
        //     } else { // random of list
        //         int index = pureRandom.Next(0, pool.Count);
        //         enemy = pool[index];
        //         break;
        //     }
        // }

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
            GameObject thing = Instantiate(enemyEntry.enemyPrefab, worldPos, Quaternion.identity, enemiesLayer.transform);
            thing.GetComponent<Enemy>().SetPlayer(player);
            Debug.Log("pop, i made an enemy");
        }

        // foreach (var entry in spawnTable.enemies) {
        //     cumulative += entry.spawnRank;
        //     if (roll <= cumulative) {
        //         //Instantiate(entry.enemyPrefab, CellToWorld(position), Quaternion.identity);
        //         Vector3 worldPos = floorTM.CellToWorld(sp3) + floorTM.cellSize / 2f;
        //         break;
        //     }
        // }
    }

    public void UpdateDictionaryEnemySpawned(Vector2Int pos, Enemy enemy) {
        enemyMap[pos] = enemy;
    }

    public void UpdateDictionaryEnemyMoved(Vector2Int oldPos, Vector2Int newPos) {
        Enemy enemy = enemyMap[oldPos];
        enemyMap.Remove(oldPos);
        enemyMap[newPos] = enemy;
    }

    public void UpdateDictionaryEnemyDied(Vector2Int atPos) {
        enemyMap.Remove(atPos);
    }

}