using UnityEngine;

public class RobotController : MonoBehaviour {
    // Configruation
    public CameraController CameraController;

    // Runtime
    public Robot SelectedRobot;
    public Command QueuedCommand;

    void Start() {
        if (!CameraController) {
            CameraController = Camera.main.GetComponent<CameraController>();
        }

        GameController.Instance.OnTickComplete += OnTickComplete;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Util.MouseWorldPosition(Camera.main);
            Collider2D col = Physics2D.OverlapPoint(mousePos);
            if (col != null) {
                var robot = col.gameObject.GetComponent<Robot>();
                if (robot) {
                    if (SelectedRobot) {
                        SelectedRobot.SetSelected(false);
                        QueuedCommand = null;
                    }
                    SelectedRobot = robot;
                    robot.SetSelected(true);
                }
            }
            else if (SelectedRobot) {
                SelectedRobot.SetSelected(false);
                SelectedRobot = null;
                QueuedCommand = null;
            }
        }

        CameraController.SetWASDEnabled(SelectedRobot == null);

        if (SelectedRobot) {
            Command command = null;
            if (Input.GetKeyDown(KeyCode.W)) {
                command = new MoveCommand(Side.Up);
            }
            else if (Input.GetKeyDown(KeyCode.A)) {
                command = new MoveCommand(Side.Left);
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                command = new MoveCommand(Side.Down);
            }
            else if (Input.GetKeyDown(KeyCode.D)) {
                command = new MoveCommand(Side.Right);
            }

            if (command != null) {
                // TODO: Frame perfect glitch with double execution
                if (GameController.Instance.PercentComplete == 0) {
                    SelectedRobot.SetCommand(command, GameController.Instance.TickNumber);
                    GameController.Instance.ExecuteTick();
                }
                else {
                    QueuedCommand = command;
                }
            }
        }        
    }

    void OnTickComplete() {
        if (QueuedCommand != null) {
            SelectedRobot.SetCommand(QueuedCommand, GameController.Instance.TickNumber);
            GameController.Instance.ExecuteTick();
            QueuedCommand = null;
        }
        else {
            // TODO: Fix whatever went wrong here
            Command command = null;
            if (Input.GetKey(KeyCode.W)) {
                command = new MoveCommand(Side.Up);
            }
            else if (Input.GetKey(KeyCode.A)) {
                command = new MoveCommand(Side.Left);
            }
            else if (Input.GetKey(KeyCode.S)) {
                command = new MoveCommand(Side.Down);
            }
            else if (Input.GetKey(KeyCode.D)) {
                command = new MoveCommand(Side.Right);
            }

            if (command != null) {
                SelectedRobot.SetCommand(command, GameController.Instance.TickNumber);
                GameController.Instance.ExecuteTick();
            }
        }
    }
}