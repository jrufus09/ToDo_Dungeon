using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class CleanupEnemySpawnThemes
{
    [MenuItem("Tools/Cleanup/Remove Null Theme Entries")]
    public static void RemoveNullThemeEntries()
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemySpawnTable");
        int cleaned = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemySpawnTable table = AssetDatabase.LoadAssetAtPath<EnemySpawnTable>(path);

            if (table == null || table.enemies == null) continue;

            bool changed = false;

            foreach (var entry in table.enemies)
            {
                if (entry == null || entry.themes == null) continue;

                int before = entry.themes.Count;
                entry.themes = entry.themes.Where(t => t != null).ToList();
                if (entry.themes.Count != before)
                {
                    Debug.Log($"Cleaned {before - entry.themes.Count} null theme(s) in entry for asset: {path}");
                    changed = true;
                    cleaned++;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(table);
            }
        }

        if (cleaned > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"✅ Cleaned theme lists in {cleaned} spawn entries.");
        }
        else
        {
            Debug.Log("✨ No null themes found. All good!");
        }
    }
}
