using System;
using System.Collections.Generic;

[Serializable]
public class BoardData { // List of all boards
    public List<Board> boards = new List<Board>();
}

[Serializable]
public class Board { // A board is comprised of lists
    public string name;
    public List<ListData> lists = new List<ListData>();
}

[Serializable]
public class ListData { // A list is comprised of tasks
    public string name;
    public List<Item> items = new List<Item>();
}

[Serializable]
public class Item {
    public string name;
    public string dueDate;
    public bool isCompleted;
}
