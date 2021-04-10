using System.IO;

public class MapWriter {
    public void WriteMap(MapInfo info, string path) {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))) {
            writer.Write(info.VersionMajor);
            writer.Write(info.VersionMinor);
            
            writer.Write(info.Width);
            writer.Write(info.Height);

            writer.Write(info.StartingX);
            writer.Write(info.StartingY);

            foreach (var tile in info.Tiles) {
                WriteTile(writer, tile);
            }
            
            // TODO for extra data payload
        }
    }

    public void WriteTile(BinaryWriter writer, TileData tile) {
        writer.Write((byte)tile.GetGridType());
        writer.Write(tile.GetDataSize());
        writer.Write(tile.GetData());
    }
}

public class MapReader {
    public MapInfo ReadMap(string path) {
        MapInfo info = new MapInfo();
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open))) {
            info.VersionMajor = reader.ReadByte();
            info.VersionMinor = reader.ReadByte();

            info.Width = reader.ReadInt16();
            info.Height = reader.ReadInt16();

            info.StartingX = reader.ReadInt16();
            info.StartingY = reader.ReadInt16();

            const int numberOfLayers = 2;
            int numberOfTiles = info.Width * info.Height * numberOfLayers;

            info.Tiles = new TileData[numberOfTiles];

            int tileIndex = 0;
            for (int layer = GridLayer.Ground; layer <= GridLayer.Object; layer++) {
                for (int y = info.StartingY; y < info.StartingY + info.Height; y++) {
                    for (int x = info.StartingX; x < info.StartingX + info.Width; x++) {
                        info.Tiles[tileIndex++] = ReadTile(reader);
                    }
                }
            }
        }

        return info;
    }

    public TileData ReadTile(BinaryReader reader) {
        GridType type = (GridType)reader.ReadByte();
        short size = reader.ReadInt16();
        byte[] data = reader.ReadBytes(size);

        TileData info = Util.ToTileInfo(type);

        if (info != null) {
            info.Deserialize(data);
        }

        return info;
    }
}