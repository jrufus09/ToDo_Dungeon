using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class BoardDataManager : MonoBehaviour {

    public static BoardDataManager Instance { get; private set; } // Singleton ("static")

    void Start() {
        Debug.Log($"persistent data path on {Application.platform} is: {Application.persistentDataPath}");
        
        currentSessionData = LoadData(folderPath);
        
        // should probably have a more robust system dealing with corruption and missing boards
        // for the time being, the Load function below resets it.
        // let's check how boards are in the save data we got.
        if (currentSessionData.boards.Count == 0) {
            Debug.Log("no boards found in this save data.");
            // Make a new save data
            NewSaveData();
        } else {
            Debug.Log($"loaded {currentSessionData.boards.Count} boards.");
            //boards = currentSessionData.boards;
            LoadBoardNames();
            LoadAllIcons();
        }

        StartCoroutine(AutoSaveRoutine());
    }

    public string folderPath;

    [TextArea]
    [Tooltip("save every x seconds")]
    public float saveInterval = 30f;

    public BoardData currentSessionData; // this is ALL boards loaded to the current session
    //[SerializeField] private List<Board> boards; // for checking in inspector

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
        this.SaveData(true);
    }

    // public String GetFilePath(String boardName) {
    //     return Path.Combine(this.folderPath, boardName);
    // }

    //public void SaveData(String filePath, BoardData data) {  //SaveData(folderPath, currentSessionData);
        // string json = JsonUtility.ToJson(data, true);
        // File.WriteAllText(filePath, json);
        // Debug.Log("Data saved to " + filePath);
    // since this class manages the session, and all boards are kept in the same file,
    // FILEPATH isn't necessary: they reach into the same class-wide variables
    public void SaveData(bool overwrite = false) {
        BoardData saveThis = currentSessionData;
        if (overwrite == true) {
            saveThis = new BoardData();
        }
        string json = JsonUtility.ToJson(saveThis, true);
        File.WriteAllText(folderPath, json);
        Debug.Log("data saved to " + folderPath);
        LoadBoardNames();
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

    private HashSet<string> boardNamesSet; // for quick lookup

    private void LoadBoardNames() { // call this whenever json is loaded / updated
        boardNamesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Board board in currentSessionData.boards) {
            boardNamesSet.Add(board.name);
        }
    }

    public bool IsBoardNameUnique(string newBoardName) {
        return !boardNamesSet.Contains(newBoardName);
    }

    public void NewBoard(string boardName) {
        Board newBoard = new Board { name = boardName };
        currentSessionData.boards.Add(newBoard);
        SaveData();
    }

    public void NewList(string boardName, string listName) {
        Board board = currentSessionData.boards.Find(b => b.name == boardName); // find board
        if (board != null) {
            board.lists.Add(new ListData { name = listName });
            SaveData();
        }
        else {
            Debug.LogError($"board not found'{boardName}'");
        }
    }

    public void NewItem(string boardName, string listName, string itemName, string dueDate, bool isCompleted = false)
    {
        Board board = currentSessionData.boards.Find(b => b.name == boardName);
        if (board != null) {
            ListData list = board.lists.Find(l => l.name == listName);
            if (list != null) {
                list.items.Add(new Item { name = itemName, dueDate = dueDate, isCompleted = isCompleted });
                SaveData();
            }
            else {
                Debug.LogError($"list not found: '{listName}'");
            }
        } else {
            Debug.LogError($"board not found:'{boardName}'");
        }
    }

    private IEnumerator AutoSaveRoutine() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            SaveData();
            Debug.Log("autosaved at " + System.DateTime.Now);
        }
    }

    public GameObject iconPrefab; 
    public Transform contentArea;
    // public void GenerateIcon(string boardNameIn) {
    //     //for (int i = 0; i < count; i++) {
    //     GameObject newIcon = Instantiate(iconPrefab, contentArea);
    //     newIcon.name = "Icon_" + boardNameIn;
    //     newIcon.GetComponent<BoardIcon>().boardName = boardNameIn;
    //     //newIcon.GetComponent<Image>().color = Random.ColorHSV(); // Example color
    //     //}
    // }

    public void LoadAllIcons() {
        foreach (Board board in currentSessionData.boards) {
            GameObject newIcon = Instantiate(iconPrefab, contentArea);
            newIcon.name = "Icon_" + board.name;
            BoardIcon icon = newIcon.GetComponent<BoardIcon>();
            icon.boardName = board.name;
            icon.Initialize(); // update text label, etc.
        }
    }
        
}


