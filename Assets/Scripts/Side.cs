using UnityEngine;

public enum Side {
    Right,
    Up,
    Left,
    Down
}

public class SideUtil {
    public static Vector3Int ToVector(Side side) {
        switch (side) {
            case Side.Right: 
                return Vector3Int.right;
            case Side.Up:
                return Vector3Int.up;
            case Side.Left:
                return Vector3Int.left;
            case Side.Down:
                return Vector3Int.down;
        }
        return Vector3Int.zero;
    }

    public static Vector3Int ToVector(int i) {
        return ToVector((Side)i);
    }
}