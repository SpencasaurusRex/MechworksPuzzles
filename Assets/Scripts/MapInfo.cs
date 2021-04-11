using System.Text;

public class MapInfo {
    public byte VersionMajor;
    public byte VersionMinor;

    public short NumberOfTiles;

    public TileData[] Tiles;

    public int ExtraDataLength;
    public byte[] ExtraData;
}

public struct Version {
    public byte Major;
    public byte Minor;

    public Version(byte major, byte minor) {
        Major = major;
        Minor = minor;
    }
}

public interface TileData {
    GridType GetGridType();
    (short, short, sbyte) GetPosition();
    void SetPosition((short, short, sbyte) position);
    short GetDataSize(Version v);
    byte[] GetData(Version v);
    void Deserialize(byte[] data, Version v);
}

// Basic Tiles
public class BasicTileInfo : TileData {
    GridType type;
    public (short, short, sbyte) Position;
    
    public BasicTileInfo(GridType type) {
        this.type = type;
    }

    public GridType GetGridType() {
        return type;
    }

    public void SetPosition((short, short, sbyte) position) {
        Position = position;
    }

    public (short, short, sbyte) GetPosition() {
        return Position;
    }

    public short GetDataSize(Version v) {
        return 0;
    }

    public byte[] GetData(Version v) {
        return new byte[0];
    }

    public void Deserialize(byte[] data, Version v) {
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
    public (short, short, sbyte) Position;

    public ColorTileInfo(GridType type) {
        this.Type = type;
    }

    public ColorTileInfo(GridType type, ObjectColor color) : this(type) {
        this.Color = color;
    }

    public GridType GetGridType() {
        return Type;
    }

    public void SetPosition((short, short, sbyte) position) {
        Position = position;
    }

    public (short, short, sbyte) GetPosition() {
        return Position;
    }

    public short GetDataSize(Version v) {
        return 1;
    }

    public byte[] GetData(Version v) {
        return new byte[] { (byte)Color };
    }

    public void Deserialize(byte[] data, Version v) {
        System.Diagnostics.Debug.Assert(data.Length == 1);
        Color = (ObjectColor)data[0];
    }
}

public class SpawnerTileInfo : ColorTileInfo { 
    public SpawnerTileInfo() : base(GridType.Spawner) {}
    public SpawnerTileInfo(ObjectColor color) : base(GridType.Spawner, color) {} 
}
public class BlockTileInfo   : ColorTileInfo {
    public BlockTileInfo  () : base(GridType.Block) {}
    public BlockTileInfo (ObjectColor color) : base(GridType.Block, color) {}
}

// Robot tiles
public class RobotTileInfo : TileData {
    public string Code;
    public (short, short, sbyte) Position;

    public void Deserialize(byte[] data, Version v) {
        Code = Encoding.ASCII.GetString(data);
    }

    public void SetPosition((short, short, sbyte) position) {
        Position = position;
    }

    public (short, short, sbyte) GetPosition() {
        return Position;
    }

    public byte[] GetData(Version v) {
        return Encoding.ASCII.GetBytes(Code);
    }

    public short GetDataSize(Version v) {
        if (Code == null) Code = "";
        return (short)Encoding.UTF8.GetBytes(Code).Length;
    }

    public GridType GetGridType() {
        return GridType.Robot;
    }

}

// Target tiles
public class TargetTileInfo : TileData { 
    public ObjectColor Color;
    public byte GoalCount;
    public (short, short, sbyte) Position;

    public TargetTileInfo() {
        Color = ObjectColor.None;
        GoalCount = 0;
    }

    public TargetTileInfo (ObjectColor color, byte goalCount) {
        this.Color = color;
        this.GoalCount = goalCount;
    }

    public void Deserialize(byte[] data, Version v) {
        Color = (ObjectColor)data[0];
        GoalCount = data[1];
    }

    public GridType GetGridType() {
        return GridType.Target;
    }

    public void SetPosition((short, short, sbyte) position) {
        Position = position;
    }

    public (short, short, sbyte) GetPosition() {
        return Position;
    }

    public short GetDataSize(Version v) {
        return 2;
    }

    public byte[] GetData(Version v) {
        return new byte[] { (byte)Color, GoalCount };
    }
}