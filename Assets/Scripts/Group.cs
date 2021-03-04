using System.Collections.Generic;

public class Group {
    public List<GridObject> Objects = new List<GridObject>();

    public void Add(GridObject go) {
        if (go.Group != this) {
            Combine(go.Group);
        }
    }

    public void Combine(Group other) {
        foreach(var go in other.Objects) {
            go.Group = this;
            Objects.Add(go);
        }
    }
}