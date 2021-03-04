using UnityEngine;

public enum Side {
    Right,
    Up,
    Left,
    Down
}

public class SideUtil {
    public static Vector2Int ToVector(Side side) {
        switch (side) {
            case Side.Right: 
                return Vector2Int.right;
            case Side.Up:
                return Vector2Int.up;
            case Side.Left:
                return Vector2Int.left;
            case Side.Down:
                return Vector2Int.down;
        }
        return Vector2Int.zero;
    }
}