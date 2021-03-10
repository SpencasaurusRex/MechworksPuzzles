using UnityEngine;

public class Robot : MonoBehaviour
{
    // Configuration
    [TextArea(5,10)]
    public string Code;
    
    // Runtime
    public string[] Commands;
    public int CommandIndex;
    GridObject gridObject;

    SpriteRenderer[] ConnectionSprites;

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Robot;
    }

    void Start()
    {
        var controller = FindObjectOfType<GameController>();
        controller.OnTick += Tick;
        ParseCode();

        ConnectionSprites = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++) {
            ConnectionSprites[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();    
        }
        
        gridObject.OnConnect += OnConnect;
        gridObject.OnDisconnect += OnDisconnect;

    }

    void OnDestroy() {
        gridObject.OnConnect -= OnConnect;
        gridObject.OnDisconnect -= OnDisconnect;
    }

    public bool debug = false;

    void print(string str) {
        if (debug) 
            MonoBehaviour.print(str);
    }

    void OnConnect(Side side, GridObject obj) {
        ConnectionSprites[(int)side].enabled = true;
    }

    void OnDisconnect(Side side, GridObject obj) {
        ConnectionSprites[(int)side].enabled = false;
    }

    void Tick() {
        string line = Commands[CommandIndex].Trim().ToUpper();
        string[] words = line.Split(' ');
        string command = words[0];
        string argument;
        switch (command) {
            case "MOVE":
                if (words.Length == 1) {
                    print("Expected argument to MOVE command");
                }
                if (words.Length != 2) {
                    print("Invalid format of MOVE command: \"" + Commands[CommandIndex] + "\"");
                }
                argument = words[1];
                if (argument == "LEFT") {
                    print("Moving left");
                    gridObject.RequestMove(Vector3Int.left);
                }
                else if (argument == "RIGHT") {
                    print("Moving right");
                    gridObject.RequestMove(Vector3Int.right);
                }
                else if (argument == "UP") {
                    print("Moving up");
                    gridObject.RequestMove(Vector3Int.up);
                }
                else if (argument == "DOWN") {
                    print("Moving down");
                    gridObject.RequestMove(Vector3Int.down);
                }
                else {
                    print("Unknown argument to MOVE command: " + argument);
                }
                break;
            case "GRAB":
                if (words.Length == 1) {
                    print("Expected argument to GRAB command");
                }
                if (words.Length != 2) {
                    print("Invalid format of GRAB command: \"" + Commands[CommandIndex] + "\"");
                }
                argument = words[1];
                if (argument == "LEFT") {
                    print("Grabbing left");
                    gridObject.ConnectSide(Side.Left);
                }
                else if (argument == "RIGHT") {
                    print("Grabbing right");
                    gridObject.ConnectSide(Side.Right);
                }
                else if (argument == "UP") {
                    print("Grabbing up");
                    gridObject.ConnectSide(Side.Up);
                }
                else if (argument == "DOWN") {
                    print("Grabbing down");
                    gridObject.ConnectSide(Side.Down);
                }
                else {
                    print("Unknown argument to GRAB command: " + argument);
                }
                break;
            case "DROP":
                if (words.Length == 1) {
                    print("Expected argument to DROP command");
                }
                if (words.Length != 2) {
                    print("Invalid format of DROP command: \"" + Commands[CommandIndex] + "\"");
                }
                argument = words[1];
                if (argument == "LEFT") {
                    print("Dropping left");
                    gridObject.DisconnectSide(Side.Left);
                }
                else if (argument == "RIGHT") {
                    print("Dropping right");
                    gridObject.DisconnectSide(Side.Right);
                }
                else if (argument == "UP") {
                    print("Dropping up");
                    gridObject.DisconnectSide(Side.Up);
                }
                else if (argument == "DOWN") {
                    print("Dropping down");
                    gridObject.DisconnectSide(Side.Down);
                }
                else {
                    print("Unknown argument to GRAB command: " + argument);
                }
                break;
            case "SYNC":
                // TODO:
            case "WAIT":
                // noop
                break;
            default:
                print("Unsupported command");
                break;
        }

        CommandIndex = (CommandIndex + 1) % Commands.Length;
    }

    void ParseCode() {
        Commands = Code.Split('\n');
        
        // foreach (var line in codeLines) {
        //     string[] words = line.Trim().Split(" ");
        //     string command = words[0].ToUpper();
        //     List<string> arguments = new List<string>();

        //     switch (command) {
        //         case "MOVE":

        //             break;
        //     }
        // }
    }

    void Update() {
        
    }
}

// public interface Command {
//     void Execute();
// }

// public class MoveCommand : Command
// {
//     public enum MoveCommandArgument {
//         Right,
//         Up,
//         Left,
//         Down
//     }

//     public MoveCommandArgument Argument;
//     public void Execute() {

//     }
// }