using UnityEngine;

public class MapRunner : MonoBehaviour {
    public void LoadMap(string path) {
        MapInfo info = MapReader.ReadMap(path);
    }
}