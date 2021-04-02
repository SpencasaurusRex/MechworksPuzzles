using UnityEngine;
using System;
using System.Collections.Generic;

public class Move : IEquatable<Move> {
    public Vector3Int From;
    public Vector3Int To;
    public GridObject Object;
    public bool Blocked;
    public List<Move> Dependents; // A move that depends on this move to complete successfully
    public Vector3Int Delta => To - From;

    public Move(GridObject obj, Vector3Int from, Vector3Int to) {
        Object = obj;
        From = from;
        To = to;
        Dependents = new List<Move>();
    }

    public void Block() {
        if (!Blocked) {
            Blocked = true;
            if (Dependents != null) {
                foreach (var dependent in Dependents) {
                    if (!dependent.Blocked) {
                        dependent.Block();
                    }
                }
            }
        }
    }

    public override bool Equals(object obj) {
        if (obj is Move) return Equals(obj as Move);
        return false;
    }

    public override int GetHashCode() {
        return (From, To, Object).GetHashCode();
    }

    public bool Equals(Move other) {
        return other.From == From && other.To == To && other.Object == Object;
    }

    public override string ToString() {
        return $"{Object.name} {From} {Delta}";
    }
}