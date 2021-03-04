using UnityEngine;
using System;

public class Move
{
    public Vector2Int From;
    public Vector2Int To;
    public GridObject Object;
    public bool Blocked;
    public Move Dependency; // A move that depends on this move to complete successfully

    public Move(GridObject obj, Vector2Int from, Vector2Int to) {
        Object = obj;
        From = from;
        To = to;
    }
}