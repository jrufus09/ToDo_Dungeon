using System;
using System.Collections.Generic;

[Serializable]
public class BoardData {
    public List<Board> boards = new List<Board>();
}

[Serializable]
public class Board {
    public string name;
    public List<ListData> lists = new List<ListData>();
}

[Serializable]
public class ListData {
    public string name;
    public List<Item> items = new List<Item>();
}

[Serializable]
public class Item {
    public string name;
    public string dueDate;
    public bool isCompleted;
}
