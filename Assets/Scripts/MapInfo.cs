public class MapInfo {
    public byte VersionMajor;
    public byte VersionMinor;

    public byte Width;
    public byte Height;
    public byte Layers;

    public TileInfo[] Tiles;

    public int ExtraDataLength;
    public byte[] ExtraData;

    // ExtraData Payload
    public byte NumberOfRobots;
    // TODO
}

public interface TileInfo {
    GridType GetGridType();
    byte GetDataSize();
    byte[] GetData();

}

// Basic Tiles
public class BasicTileInfo : TileInfo {
    GridType type;
    public BasicTileInfo(GridType type) {
        this.type = type;
    }

    public GridType GetGridType() {
        return type;
    }

    public byte GetDataSize() {
        return 0;
    }

    public byte[] GetData() {
        return new byte[0];
    }
}

public class GroundTileInfo : BasicTileInfo { public GroundTileInfo() : base(GridType.Ground) {} }
public class NoneTileInfo   : BasicTileInfo { public NoneTileInfo  () : base(GridType.None  ) {} }
public class RobotTileInfo  : BasicTileInfo { public RobotTileInfo () : base(GridType.Robot ) {} }

// Color tiles
public class ColorTileInfo : TileInfo {
    GridType type;
    ObjectColor color;

    public ColorTileInfo(GridType type, ObjectColor color) {
        this.type = type;
        this.color = color;
    }
    
    public GridType GetGridType() {
        return type;
    }

    public byte GetDataSize() {
        return 1;
    }

    public byte[] GetData() {
        return new byte[] { (byte)color };
    }
}

public class SpawnerTileInfo : ColorTileInfo { public SpawnerTileInfo(ObjectColor color) : base(GridType.Spawner, color) {} }
public class TargetTileInfo  : ColorTileInfo { public TargetTileInfo (ObjectColor color) : base(GridType.Spawner, color) {} }