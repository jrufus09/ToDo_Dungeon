using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class BoardDataManager : MonoBehaviour {
    void Start() {
        Debug.Log($"The persistent data path on {Application.platform} is located at: {Application.persistentDataPath}");
    }

    public string folderPath;

    void Awake() {
        //filePath = Path.Combine(Application.persistentDataPath, "data.json");
        folderPath = Path.Combine(Application.persistentDataPath, "/Boards");
    }

    public void NewSaveData(String boardName) {
        String filePath = GetFilePath(boardName);
        this.SaveData(filePath, new BoardData());
    }

    public String GetFilePath(String boardName) {
        return Path.Combine(this.folderPath, boardName);
    }

    public void SaveData(String filePath, BoardData data) {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
    }

    public BoardData LoadData(String filePath) {
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<BoardData>(json);
        } else {
            Debug.LogWarning("No save file found, or data corrupted.");
            //return new BoardData();
            return null;
        }
    }
}


