using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class EditorController : MonoBehaviour {
    // Configuration
    public EditorTile EditorTilePrefab;
    public Transform TilesParent;
    public Image GroundLayerVisibilityButton;
    public Image ObjectLayerVisibilityButton;

    public Sprite[] VisibilityButtonSprites;
    public GameObject TilesPanel;
    public GameObject PropertiesPanel;
    public Transform HighlightRect;

    public bool Debug;

    // Runtime
    public static EditorController Instance {get; set;}
    
    TileSelect SelectedBrush;
    Dictionary<Vector3Int, EditorTile> Tiles = new Dictionary<Vector3Int, EditorTile>();

    void Awake() {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    void Update() {
        HighlightRect.position = Vector3.Lerp(HighlightRect.position, Util.MouseTilePosition(Camera.main).WithZ(5), 0.4f);
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
            info.Tiles = new TileData[numberOfTiles];
            int tileIndex = 0;
            for (int layer = GridLayer.Ground; layer <= GridLayer.Object; layer++) {
                for (int y = info.StartingY; y < info.StartingY + info.Height; y++) {
                    for (int x = info.StartingX; x < info.StartingX + info.Width; x++) {
                        var position = new Vector3Int(x, y, layer);
                        if (Tiles.ContainsKey(position)) {
                            info.Tiles[tileIndex] = Tiles[position].Data;
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

        int tileIndex = 0;
        for (int layer = GridLayer.Ground; layer <= GridLayer.Object; layer++) {
            for (int y = info.StartingY; y < info.StartingY + info.Height; y++) {
                for (int x = info.StartingX; x < info.StartingX + info.Width; x++) {
                    TileData tileInfo = info.Tiles[tileIndex++];
                    if (tileInfo != null) CreateEditorTile(tileInfo, x, y, layer);
                }
            }
        }
    }

    public void CreateEditorTile(TileData info, int x, int y, int layer) {
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

    bool groundLayerVisible = true;
    bool objectLayerVisible = true;
    
    public bool IsGroundLayerVisible() => groundLayerVisible;
    public bool IsObjectLayerVisible() => objectLayerVisible;

    public void SetGroundLayerVisible(bool visible) {
        if (groundLayerVisible != visible) {
            groundLayerVisible = visible;
            UpdateVisibility();
        }
    }

    public void SetObjectLayerVisible(bool visible) {
        if (objectLayerVisible != visible) {
            objectLayerVisible = visible;
            UpdateVisibility();
        }
    }

    public void ClickGroundLayer() {
        groundLayerVisible = !groundLayerVisible;
        UpdateVisibility();
        
    }

    public void ClickObjectLayer() {
        objectLayerVisible = !objectLayerVisible;
        UpdateVisibility();
        
    }

    void UpdateVisibility() {
        ObjectLayerVisibilityButton.sprite = VisibilityButtonSprites[objectLayerVisible ? 0 : 1];
        GroundLayerVisibilityButton.sprite = VisibilityButtonSprites[groundLayerVisible ? 2 : 3];
        foreach (var tile in Tiles.Values) {
            if (tile.Location.z == GridLayer.Ground) {
                tile.SetVisible(groundLayerVisible);
            }
            else if (tile.Location.z == GridLayer.Object) {
                tile.SetVisible(objectLayerVisible);
            }
        }
    }

    public void ClickPropertiesButton() {
        TilesPanel.SetActive(false);
        TilesPanel.GetComponent<TileSelectPanel>().enabled = false;
        PropertiesPanel.SetActive(true);
        PropertiesPanel.GetComponent<PropertiesPanel>().ShowPanel();
    }

    public void ClickTilesButton() {
        TilesPanel.SetActive(true);
        TilesPanel.GetComponent<TileSelectPanel>().enabled = true;
        PropertiesPanel.GetComponent<PropertiesPanel>().HidePanel();
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