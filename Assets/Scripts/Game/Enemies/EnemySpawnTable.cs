using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemySpawnTable", menuName = "Game Data/Enemy Spawn Table")]
public class EnemySpawnTable : ScriptableObject {
    public List<EnemySpawnEntry> enemies;
}

public enum Theme {
    Testing
}

public enum SpawnRank {
    // to add/update/ if you want to edit probabilities it's in enemyhandler/generate rank
    Common, // ~50%
    Rare, // ~30%
    Elite, // ~9%
    Mythical // ~1%
}


[System.Serializable]
public class EnemySpawnEntry {
    public GameObject enemyPrefab;
    public SpawnRank spawnRank;
    public List<Theme> themes = new List<Theme>();
}
