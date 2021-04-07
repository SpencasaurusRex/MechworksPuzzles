using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EditorController : MonoBehaviour {
    // Configuration
    public EditorTile EditorTilePrefab;
    public Transform TilesParent;

    public bool Debug;

    // Runtime
    public static EditorController Instance {get; set;}
    
    Vector2 MouseWorldPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition).xy();
    Dictionary<Vector3Int, EditorTile> Tiles = new Dictionary<Vector3Int, EditorTile>();

    void Awake() {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    bool currentlyDragging = false;
    EditorTile dragging;

    const int LEFT = 0;
    const int RIGHT = 1;

    void Update() {

        if (Input.GetMouseButtonDown(LEFT)) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPosition, Vector2.zero);
            if (hits.Length > 0) {
                var tile = hits[0].transform.GetComponent<EditorTile>();
                if (tile != null) {
                    dragging = tile;
                    currentlyDragging = true;
                }
            }
        }
        else if (currentlyDragging && Input.GetMouseButton(LEFT)) {
            dragging.OnDrag(MouseWorldPosition);
        }
        else if (currentlyDragging && !Input.GetMouseButton(LEFT)) {
            dragging.EndDrag(MouseWorldPosition);
            dragging = null;
            currentlyDragging = false;
        }

        if (Input.GetMouseButtonDown(RIGHT)) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPosition, Vector2.zero);
            if (hits.Length > 0) {
                var tile = hits[0].transform.GetComponent<EditorTile>();
                if (tile != null) {
                    tile.RightClick();
                }
            }
        }
    }

    public void Serialize() {
        MapInfo info = new MapInfo();

        if (Tiles.Count > 0) {
            
            info.VersionMajor = 0;
            info.VersionMinor = 1;

            // Get bounding box of tiles
            int startingX = 0;
            int startingY = 0;
            int endingX = 0;
            int endingY = 0;
            
            endingX = startingX = Tiles.First().Key.x;
            endingY = startingY = Tiles.First().Key.y;

            foreach (var kvp in Tiles) {
                var location = kvp.Key;
                var tile = kvp.Value;
                startingX = Mathf.Min(startingX, location.x);
                startingY = Mathf.Min(startingY, location.y);
                endingX = Mathf.Max(endingX, location.x);
                endingY = Mathf.Max(endingY, location.y);
            }

            info.StartingX = (short)startingX;
            info.StartingY = (short)startingY;

            info.Width = (short)(endingX - startingX + 1);
            info.Height = (short)(endingY - startingY + 1);


            // Loop through tiles
            const int numberOfLayers = 2;
            int numberOfTiles = numberOfLayers * info.Width * info.Height;
            info.Tiles = new TileInfo[numberOfTiles];
            int tileIndex = 0;
            for (int layer = GridLayer.Ground; layer <= GridLayer.Object; layer++) {
                for (int y = info.StartingY; y < info.StartingY + info.Height; y++) {
                    for (int x = info.StartingX; x < info.StartingX + info.Width; x++) {
                        var position = new Vector3Int(x, y, layer);
                        if (Tiles.ContainsKey(position)) {
                            info.Tiles[tileIndex] = Tiles[position].Info;
                        }
                        else {
                            info.Tiles[tileIndex] = new NoneTileInfo();
                        }
                        tileIndex++;
                    }
                }
            }

            
            // Extra data
            info.ExtraData = new byte[0];
            info.ExtraDataLength = 0;
        }

        MapWriter writer = new MapWriter();
        writer.WriteMap(info, @"C:\Projects\MechworksPuzzles\Assets\Maps\testmap.mpm");
    }

    public void Reset() {
        Tiles.Clear();
        int children = TilesParent.childCount;
        for (int i = 0; i < children; i++) {
            Destroy(TilesParent.GetChild(i).gameObject);
        }
    }

    public void Deserialize() {
        Reset();

        MapReader reader = new MapReader();
        MapInfo info = reader.ReadMap(@"C:\Projects\MechworksPuzzles\Assets\Maps\testmap.mpm");
        
        print($"Read map with {info.Width} {info.Height} {info.StartingX} {info.StartingY}");

        int tileIndex = 0;
        for (int layer = GridLayer.Ground; layer <= GridLayer.Object; layer++) {
            for (int y = info.StartingY; y < info.StartingY + info.Height; y++) {
                for (int x = info.StartingX; x < info.StartingX + info.Width; x++) {
                    TileInfo tileInfo = info.Tiles[tileIndex++];
                    if (tileInfo != null) CreateEditorTile(tileInfo, x, y, layer);
                }
            }
        }

    }

    public void CreateEditorTile(TileInfo info, int x, int y, int layer) {
        var editorTile = Instantiate(EditorTilePrefab, new Vector3(x, y, layer), Quaternion.identity, TilesParent);
        var position = new Vector3Int(x, y, layer);
        editorTile.Setup(info, position);
        AddTile(position, editorTile);
    }

    public EditorTile GetTile(Vector3Int position) {
        if (Tiles.ContainsKey(position))
            return Tiles[position];
        else return null;
    }

    public void AddTile(Vector3Int position, EditorTile tile) {
        if (!Tiles.ContainsKey(position)) {
            Tiles[position] = tile;
            tile.Location = position;
        }
    }

    public void RemoveTile(Vector3Int position) {
        if (Tiles.ContainsKey(position))
            Tiles.Remove(position);
    }

    void OnDrawGizmos() {
        if (Debug) {
            Gizmos.color = Color.red;
            foreach (var kvp in Tiles) {
                Gizmos.DrawCube(kvp.Key.ToFloat(), Vector3.one * 0.5f);
            }
        }
    }
}