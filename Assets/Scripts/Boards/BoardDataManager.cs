using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class BoardDataManager : MonoBehaviour {

    public static BoardDataManager Instance { get; private set; } // Singleton ("static")

    void Start() {
        Debug.Log($"persistent data path on {Application.platform} is: {Application.persistentDataPath}");
        
        currentBoardData = LoadData(folderPath);
        
        // should probably have a more robust system dealing with corruption and missing boards
        // for the time being, the Load function below resets it.
        // let's check how boards are in the save data we got.
        if (currentBoardData.boards.Count == 0) {
            Debug.Log("no boards found in this save data.");
            // Make a new save data
            NewSaveData();
        } else {
            Debug.Log($"loaded {currentBoardData.boards.Count} boards.");
            boards = currentBoardData.boards;
        }
    }

    public string folderPath;

    [TextArea]
    [Tooltip("save every x seconds")]
    public float saveInterval = 30f;

    public BoardData currentBoardData; // this is ALL boards loaded to the current session
    [SerializeField] private List<Board> boards; // for checking in inspector

    void Awake() {
        // initiate instance

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // DESTROY duplicates
        }

        //filePath = Path.Combine(Application.persistentDataPath, "data.json");
        folderPath = Path.Combine(Application.persistentDataPath, "Boards");

        
    }

    public void NewSaveData() {
        //String filePath = GetFilePath(boardName);
        this.SaveData(this.folderPath, new BoardData());
    }

    // public String GetFilePath(String boardName) {
    //     return Path.Combine(this.folderPath, boardName);
    // }

    public void SaveData(String filePath, BoardData data) {  //SaveData(folderPath, currentBoardData);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
    }

    public BoardData LoadData(String filePath) {
        try {
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<BoardData>(json);
            } else {
                Debug.LogWarning("No save file found, or data corrupted.");
                return new BoardData();
            //return null;
            }
        } catch (System.Exception e) {
            Debug.LogError($"Failed to load boards: {e.Message}");
            return null;
        }
    }

    public void NewBoard(string boardName) {
        Board newBoard = new Board { name = boardName };
        currentBoardData.boards.Add(newBoard);
    }

    private IEnumerator AutoSaveRoutine() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            SaveData(folderPath, currentBoardData);
            Debug.Log("autosaved at " + System.DateTime.Now);
        }
    }


        
}


