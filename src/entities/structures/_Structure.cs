namespace LibCast {

    public class LevelData
    {
        public Dictionary<ushort, TileDefinition> Tiles { get; set; }
        public int[][] Map { get; set; }
    }

    public class TileDefinition
    {
        public string Type { get; set; }
        public Dictionary<string, string> Textures { get; set; }
    }

    public abstract class Structure {
        public virtual List<Entity> entities {get; set;}
        public virtual List<RoomCell> roomCells {get; set;}
        public virtual List<Entity> map {get; set;}


        public Structure() {
            
        }

        public static void ParseFromFile(string path) {
            
        }
    }
}