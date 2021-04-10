using System.Text;

public class MapInfo {
    public byte VersionMajor;
    public byte VersionMinor;

    public short Width;
    public short Height;

    public short StartingX;
    public short StartingY;

    public TileData[] Tiles;

    public int ExtraDataLength;
    public byte[] ExtraData;

    // ExtraData Payload
    
    // TODO
}

public interface TileData {
    GridType GetGridType();
    short GetDataSize();
    byte[] GetData();
    void Deserialize(byte[] data);
}

// Basic Tiles
public class BasicTileInfo : TileData {
    GridType type;
    public BasicTileInfo(GridType type) {
        this.type = type;
    }

    public GridType GetGridType() {
        return type;
    }

    public short GetDataSize() {
        return 0;
    }

    public byte[] GetData() {
        return new byte[0];
    }

    public void Deserialize(byte[] data) {
        System.Diagnostics.Debug.Assert(data.Length == 0);
        return;
    }
}

public class GroundTileInfo : BasicTileInfo { public GroundTileInfo() : base(GridType.Ground) {} }
public class NoneTileInfo   : BasicTileInfo { public NoneTileInfo  () : base(GridType.None  ) {} }
public class WelderTileInfo : BasicTileInfo { public WelderTileInfo() : base(GridType.Welder) {} }
public class WallTileInfo   : BasicTileInfo { public WallTileInfo  () : base(GridType.Wall  ) {} }

// Color tiles
public class ColorTileInfo : TileData {
    public GridType Type;
    public ObjectColor Color;
    
    public ColorTileInfo(GridType type) {
        this.Type = type;
    }

    public ColorTileInfo(GridType type, ObjectColor color) : this(type) {
        this.Color = color;
    }

    public GridType GetGridType() {
        return Type;
    }

    public short GetDataSize() {
        return 1;
    }

    public byte[] GetData() {
        return new byte[] { (byte)Color };
    }

    public void Deserialize(byte[] data) {
        System.Diagnostics.Debug.Assert(data.Length == 1);
        Color = (ObjectColor)data[0];
    }
}

public class SpawnerTileInfo : ColorTileInfo { 
    public SpawnerTileInfo() : base(GridType.Spawner) {}
    public SpawnerTileInfo(ObjectColor color) : base(GridType.Spawner, color) {} 
}
public class TargetTileInfo  : ColorTileInfo { 
    public TargetTileInfo () : base(GridType.Target) {}
    public TargetTileInfo (ObjectColor color) : base(GridType.Target, color) {} 
}
public class BlockTileInfo   : ColorTileInfo {
    public BlockTileInfo ()  : base(GridType.Block) {}
    public BlockTileInfo (ObjectColor color) : base(GridType.Block, color) {}
}

// Robot tiles
public class RobotTileInfo : TileData {
    public string Code;
    public void Deserialize(byte[] data) {
        Code = Encoding.ASCII.GetString(data);
    }

    public byte[] GetData() {
        return Encoding.ASCII.GetBytes(Code);
    }

    public short GetDataSize() {
        if (Code == null) Code = "";
        return (short)Encoding.UTF8.GetBytes(Code).Length;
    }

    public GridType GetGridType() {
        return GridType.Robot;
    }
}