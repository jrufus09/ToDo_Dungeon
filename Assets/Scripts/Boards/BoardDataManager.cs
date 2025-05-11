using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEditor.Search;
using System.Linq;

public class BoardDataManager : MonoBehaviour {

    public static BoardDataManager Instance { get; private set; } // Singleton ("static")

    public void Start() {
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
            //Debug.Log($"loaded {currentSessionData.boards.Count} boards.");
            //boards = currentSessionData.boards;
            LoadBoardNames();
            LoadAllBoardIcons();
        }

        StartCoroutine(AutoSaveRoutine());
    }

    public string folderPath;

    [TextArea]
    [Tooltip("save every x seconds")]
    public float saveInterval = 30f;

    public BoardData currentSessionData; // this is ALL boards loaded to the current session
    //[SerializeField] private List<Board> boards; // for checking in inspector
    public Board currentlyOpenBoard;
    public ListData currentlyEditingList;

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

        if (boardContentArea == null) {
            boardContentArea = FindContentArea("Board").transform;
        }
    }

    public ContentArea FindContentArea(string type) {
        ContentArea output = null;
        var found = FindObjectsByType<ContentArea>(FindObjectsSortMode.None); // array of all ContentAreas found
        // try string to enum:
        if (Enum.TryParse<ContentArea.TypeOfContentArea>(type, out var parsedType)) {
            // parsedType is the type we're trying to find. iterate through array and find of type
            foreach (ContentArea obj in found) {
                if (obj.type == parsedType) {
                    output = obj;
                    break;
                }
            }
        } else {
            Debug.LogWarning("invalid type input to FindContentArea()");
        }
        return output;
    }

    public void SetOpenBoard(Board boardIn) {
        currentlyOpenBoard = boardIn;
    }
    public void SetCurrentList(ListData listIn) {
        currentlyEditingList = listIn;
    }

    public void NewSaveData() {
        //String filePath = GetFilePath(boardName);
        this.SaveData(true);
    }

    // public String GetFilePath(String boardName) {
    //     return Path.Combine(this.folderPath, boardName);
    // }

    public void SaveData(bool overwrite = false) {
        BoardData saveThis = currentSessionData;
        if (overwrite == true) {
            saveThis = new BoardData();
        }
        string json = JsonUtility.ToJson(saveThis, true);
        File.WriteAllText(folderPath, json);
        Debug.Log("data saved to " + folderPath + " at " + System.DateTime.Now);
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
    [SerializeField]private HashSet<string> listNamesSet;

    public void LoadBoardNames() { // call this whenever json is loaded / updated
        boardNamesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Board board in currentSessionData.boards) {
            boardNamesSet.Add(board.name);
        }
    }

    public bool IsBoardNameUnique(string newBoardName) {
        return !boardNamesSet.Contains(newBoardName);
    }

    public void LoadListNames(string boardName = null) { // call this when boardview first loaded, and when updated
        if (boardName == null) { // if not declared, get data for currently open board
            boardName = currentlyOpenBoard.name;
        }

        listNamesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Board board = GetBoard(boardName);
        if (board != null) {
            // get each list in the board
            foreach (ListData list in board.lists) {
                //Debug.Log(list);
                listNamesSet.Add(list.name);
            }
        }
    }

    public bool IsListNameUnique(string listName) {
        return !listNamesSet.Contains(listName);
    }

    public void NewBoard(string boardName) {
        Board newBoard = new Board { name = boardName };
        currentSessionData.boards.Add(newBoard);
        SaveData();
        RefreshBoardIcons();
    }

    public void NewList(string boardName, string listName) {
        Board board = GetBoard(boardName);
        if (board != null) {
            board.lists.Add(new ListData { name = listName });
            SaveData();
        }
        // refreshlist has to be called from wherever has access to the content area
    }

    public void NewItem(string boardName, string listName, string itemName, string dueDate, bool isCompleted = false)
    {
        //Board board = currentSessionData.boards.Find(b => b.name == boardName);
        Board board = GetBoard(boardName);
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

    public Board GetBoard(string boardName) {
        Board board = currentSessionData.boards.Find(b => b.name == boardName); // find board
        if (board != null) {
            return board;
        }
        else {
            Debug.LogError($"board not found'{boardName}'");
            return null;
        }
    }

    public ListData GetList(string listName, string fromBoard) {
        Board board = currentSessionData.boards.Find(b => b.name == fromBoard); // find board
        if (board != null) {
            ListData list = board.lists.Find(l => l.name == listName);
            if (list != null) {
                return list;
            } else {
                Debug.LogError($"list not found'{list}'");
                return null;
            }
        }
        else {
            Debug.LogError($"board not found'{fromBoard}'");
            return null;
        }
    }

    public List<ListData> AllLists(Board fromBoard) {
        List<ListData> listsOut = new List<ListData>();
        if (fromBoard != null) {
            // get each list in the board
            foreach (ListData list in fromBoard.lists) {
                //Debug.Log(list);
                listsOut.Add(list);
            }
        } else {
            Debug.LogWarning($"BoardDataManager/AllLists: board in is null");
        }
        return listsOut;
    }

    public List<Item> AllItems(ListData fromList) {
        List<Item> itemsOut = new List<Item>();
        if (fromList != null) {
            // get each list in the board
            foreach (Item item in fromList.items) {
                //Debug.Log(list);
                itemsOut.Add(item);
            }
        } else {
            Debug.LogWarning($"BoardDataManager/AllLists: list in is null");
        }
        return itemsOut;
    }

    public int CountDoneTasks(ListData fromList) {
        int doneCount = 0;
        List<Item> itemsOut = AllItems(fromList);
        foreach (Item item in fromList.items) {
            if (item.isCompleted == true) {
                doneCount++;
            }
        }
        return doneCount;
    }

    private IEnumerator AutoSaveRoutine() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            SaveData();
            //Debug.Log("autosaved at " + System.DateTime.Now);
        }
    }

    public GameObject boardIconPrefab;
    public GameObject listIconPrefab;
    public GameObject itemIconPrefab;
    public GameObject newTaskPrefab;
    public Transform boardContentArea;

    public void RefreshBoardIcons() { // clear icons and reload
        // Destroy all children of content.transform (in the scrollview)
        foreach(Transform child in boardContentArea){
            Destroy(child.gameObject);
        }
        LoadAllBoardIcons();
    }

    public void LoadAllBoardIcons() {
        foreach (Board board in currentSessionData.boards) {
            GameObject newIcon = Instantiate(boardIconPrefab, boardContentArea);
            newIcon.name = "Icon_" + board.name;
            BoardIcon icon = newIcon.GetComponent<BoardIcon>();
            icon.SetName(board.name);
        }
    }

    public void RefreshListIcons() { // clear icons and reload
        // use boardview Instance to get where content area is
        if (BoardView.Instance != null) { // if we can find boardview instance
            Transform contentArea = BoardView.Instance.contentPane;
            foreach(Transform child in contentArea){
                Destroy(child.gameObject);
            }
            LoadAllListIcons(contentArea);
        } else {
            Debug.LogWarning("attempted to refresh list icons, but no boardview was found");
        }
    }

    public void LoadAllListIcons(Transform contentArea) { // designed to be called from the BoardView scene
        //Debug.Log("loading list icons");
        foreach (ListData list in currentlyOpenBoard.lists) {
            //Debug.Log(list.name);
            GameObject newIcon = Instantiate(listIconPrefab, contentArea.transform); //parent to content area
                    // I'm aware this is a Transform.transform but idk it works
            newIcon.name = "Icon_" + list.name;
            ListIcon icon = newIcon.GetComponent<ListIcon>();
            icon.SetName(list.name);
            //icon.Initialize(); // update text label, etc. // moved to above
        }
        LoadListNames(); // hashset stuff
    }

    public void LoadAllTaskIcons(Transform contentArea, ListData listIn) {
        // designed to be called from the object that holds the container for the items
        //Debug.Log($"loading task icons for {listIn.name}");

        // LOAD +NEW TASK FIRST
        GameObject newTaskBtn = Instantiate(newTaskPrefab, contentArea.transform);

        // iterate through given listdata for each task
        foreach (Item item in listIn.items) {
            //Debug.Log(item.name);
            GameObject newIcon = Instantiate(itemIconPrefab, contentArea.transform);
            newIcon.name = "Icon_" + item.name;
            TaskIcon icon = newIcon.GetComponent<TaskIcon>();
            icon.SetName(item.name);
        }
    }

    // public void ToggleTask(Item task, bool toggle) {
    //     List<Item> items = AllItems(currentlyEditingList);
    //     int index = items.IndexOf(task); 
    //     if (index != -1) { // returns -1 if not in list
    //         EditItem(task.name, item => {
    //         item.isCompleted = toggle;
    //     });

    //     } else {
    //         Debug.LogWarning("item is not in currently editing list.");
    //     }
    // }

    public void RefreshTaskIcons(Transform contentArea, ListData listData) { // clear icons and reload
        //Debug.Log("Refreshing task icons");
        // use boardview Instance to get where content area is
        foreach(Transform child in contentArea){
            Destroy(child.gameObject);
        }
        LoadAllTaskIcons(contentArea, listData);
    }

    public void EditItem(string itemName, Action<Item> editCallback) { // assumes current board, current list
        // Step 1: Find the list
        ListData targetList = currentlyEditingList;

        if (targetList == null) {
            Debug.LogWarning($"please set currentlyEditingList before updating items.");
            return;
        }

        // Step 2: Find the item
        Item targetItem = targetList.items.FirstOrDefault(i => i.name == itemName);

        if (targetItem == null) {
            Debug.LogWarning($"Item '{itemName}' not found ;-;");
            return;
        }

        // Step 3: Edit it directly
        editCallback?.Invoke(targetItem);

        // example edit:
        // EditItem("sit down and cry", item => {
        //     item.isCompleted = true;
        //     //item.dueDate = DateTime.Now.AddDays(2);
        // });

    }

}


