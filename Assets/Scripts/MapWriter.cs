using System.IO;

public static class MapWriter {
    public static void WriteMap(MapInfo info, string path) {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))) {
            writer.Write(info.VersionMajor);
            writer.Write(info.VersionMinor);
            Version v = new Version(info.VersionMajor, info.VersionMinor);

            writer.Write(info.NumberOfTiles);

            foreach (var tile in info.Tiles) {
                WriteTile(writer, tile, v);
            }
        }
    }

    public static void WriteTile(BinaryWriter writer, TileData tile, Version v) {
        writer.Write((byte)tile.GetGridType());
        var (x, y, layer) = tile.GetPosition();
        writer.Write(x);
        writer.Write(y);
        writer.Write(layer);
        writer.Write(tile.GetDataSize(v));
        writer.Write(tile.GetData(v));
    }
}

public static class MapReader {
    public static MapInfo ReadMap(string path) {
        MapInfo info = new MapInfo();
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open))) {
            info.VersionMajor = reader.ReadByte();
            info.VersionMinor = reader.ReadByte();
            Version v = new Version(info.VersionMajor, info.VersionMinor);

            info.NumberOfTiles = reader.ReadInt16();
            info.Tiles = new TileData[info.NumberOfTiles];

            for (int i = 0; i < info.NumberOfTiles; i++) {
                info.Tiles[i] = ReadTile(reader, v);
            }
        }

        return info;
    }

    public static TileData ReadTile(BinaryReader reader, Version v) {
        GridType type = (GridType)reader.ReadByte();
        TileData info = Util.ToTileInfo(type);
        (short, short, sbyte) position = ((short)reader.ReadInt16(), (short)reader.ReadInt16(), (sbyte)reader.ReadSByte());
        info.SetPosition(position);

        short size = reader.ReadInt16();
        byte[] data = reader.ReadBytes(size);

        if (info != null) {
            info.Deserialize(data, v);
        }

        return info;
    }
}