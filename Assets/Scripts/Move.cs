using UnityEngine;
using System;

public class Move
{
    public Vector3Int From;
    public Vector3Int To;
    public GridObject Object;
    public bool Blocked;
    public Move Dependency; // A move that depends on this move to complete successfully

    public Move(GridObject obj, Vector3Int from, Vector3Int to) {
        Object = obj;
        From = from;
        To = to;
    }
}