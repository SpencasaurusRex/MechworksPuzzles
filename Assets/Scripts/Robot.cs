using UnityEngine;
using System.Collections.Generic;

public class Robot : MonoBehaviour {        
    // Show in Inspector
    [TextArea]
    string Code;
    
    // Runtime
    GridObject gridObject;
    SpriteRenderer[] ConnectionSprites;
    RobotTileInfo Data;
    public int CommandIndex;
    public List<Command> Commands = new List<Command>();

    public void AssignData(TileData tileData) {
        Data = tileData as RobotTileInfo;
    }
    
    public void SetSelected(bool selected) {
        int outlineWidth = selected ? 1 : 0;
        GetComponent<SpriteRenderer>().material.SetFloat("OutlineWidth", outlineWidth);
    }

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Robot;
    }

    void Start() {
        GameController.Instance.OnTick += Tick;

        ConnectionSprites = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++) {
            ConnectionSprites[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();    
        }
        
        gridObject.OnConnect += OnConnect;
        gridObject.OnDisconnect += OnDisconnect;
    }

    public void SetCommand(Command command, int commandIndex) {
        if (Commands.Count == commandIndex) {
            Commands.Add(command);
        }
        else if (Commands.Count > commandIndex) {
            Commands[commandIndex] = command;
        }
        else {
            for (int i = Commands.Count; i < commandIndex; i++) {
                Commands.Add(new WaitCommand());
            }
            Commands.Add(command);
        }

        Code = "";
        foreach (var c in Commands) {
            Code += c.ToText() + "\n";
        }
    }

    void OnDestroy() {
        gridObject.OnConnect -= OnConnect;
        gridObject.OnDisconnect -= OnDisconnect;
    }

    void OnConnect(Side side, GridObject obj) {
        ConnectionSprites[(int)side].enabled = true;
    }

    void OnDisconnect(Side side, GridObject obj) {
        ConnectionSprites[(int)side].enabled = false;
    }

    void Tick() { 
        if (Commands.Count > 0) {
            CommandIndex %= Commands.Count;
            print($"Executing command: {CommandIndex} : {Commands[CommandIndex].ToText()}");
            Commands[CommandIndex++].Execute(gridObject);
        }
    }
}

public interface Command {
    void Execute(GridObject go);
    string ToText();
}

public class MoveCommand : Command {
    public Side Argument;
    
    public MoveCommand(Side arg) {
        Argument = arg;
    }

    public void Execute(GridObject go) {
        var delta = Util.ToDelta(Argument);
        go.RequestMove(delta);
    }

    public string ToText() {
        return "MOVE " + Argument.ToString().ToUpperInvariant();
    }
}

public class GrabCommand : Command {
    public Side Argument;
    
    public GrabCommand(Side arg) {
        Argument = arg;
    }

    public void Execute(GridObject go) {
        go.ConnectSide(Argument);
    }

    public string ToText() {
        return "GRAB " + Argument.ToString().ToUpperInvariant();
    }
}

public class DropCommand : Command {
    public Side Argument;
    
    public DropCommand(Side arg) {
        Argument = arg;
    }

    public void Execute(GridObject go) {
        go.DisconnectSide(Argument);
    }

    public string ToText() {
        return "DROP " + Argument.ToString().ToUpperInvariant();
    }
}

public class WaitCommand : Command {
    public void Execute(GridObject go) {

    }

    public string ToText() {
        return "WAIT";
    }
}

public class SyncCommand : Command {
    public void Execute(GridObject go) {

    }

    public string ToText() {
        return "SYNC";
    }
}