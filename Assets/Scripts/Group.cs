using System.Collections.Generic;

public class Group {
    public List<GridObject> Objects = new List<GridObject>();
    HashSet<GridObject> hash = new HashSet<GridObject>();

    // Testing
    public void Add(GridObject go) {
        if (!Contains(go)) {
            Objects.Add(go);
        }
    }

    public bool Contains(GridObject go) {
        return hash.Contains(go);
    }
}