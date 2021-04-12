using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
    public Vector3 LastMouseWorldPosition;

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

        if (Input.GetMouseButton(2) && !lostFocus) {
            Camera.main.transform.position += (LastMouseWorldPosition - Util.MouseWorldPosition(Camera.main));
        }
        
        LastMouseWorldPosition = Util.MouseWorldPosition(Camera.main);
        
        if (currentFocus) {
            lostFocus = false;
        }
    }

    bool currentFocus;
    bool lostFocus;
    void OnApplicationFocus(bool focusStatus) {
        currentFocus = focusStatus;
        if (!focusStatus) lostFocus = true;
    }

    public void Serialize() {
        MapInfo info = new MapInfo();

        // Check for too many tiles
        if (Tiles.Count > short.MaxValue) {
            throw new System.Exception("Too many tiles to save this map. Why is your map so big??");
        }
        
        info.NumberOfTiles = (short)Tiles.Count;
        
        info.VersionMajor = 0;
        info.VersionMinor = 1;

        // Loop through tiles
        info.Tiles = new TileData[Tiles.Count];
        
        int tileIndex = 0;
        foreach (var kvp in Tiles) {
            var position = kvp.Key;
            var tile = kvp.Value;
            info.Tiles[tileIndex] = Tiles[position].Data;
            info.Tiles[tileIndex++].SetPosition(((short)position.x, (short)position.y, (sbyte)position.z));
        }
        
        // Extra data
        info.ExtraData = new byte[0];
        info.ExtraDataLength = 0;

        MapWriter.WriteMap(info, @"C:\Projects\MechworksPuzzles\Assets\Maps\testmap.mpm");
    }

    public void Reset() {
        Tiles.Clear();
        int children = TilesParent.childCount;
        for (int i = 0; i < children; i++) {
            // https://www.youtube.com/watch?v=EQ8jy7jQ3yY
            Destroy(TilesParent.GetChild(i).gameObject);
        }
    }

    public void Deserialize() {
        Reset();

        MapInfo info = MapReader.ReadMap(@"C:\Projects\MechworksPuzzles\Assets\Maps\testmap.mpm");
        
        for (int i = 0; i < info.NumberOfTiles; i++) {
            TileData tileInfo = info.Tiles[i];
            CreateEditorTile(tileInfo);
        }
    }

    public void CreateEditorTile(TileData info) {
        var (x, y, layer) = info.GetPosition();
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