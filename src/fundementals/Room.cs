
using System.Text;
using Raylib_cs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace LibCast {

    public class RoomCell : Global {
        public char character;
        public double x;
        public double y;

        public RoomCell(double x, double y, char character) {
            this.x = x;
            this.y = y;
            this.character = character;
        }
    }

    public class WallCell : RoomCell {
        public Raylib_cs.Color color;
        public bool isTextured = false;
        public string? texture { get; set; }
        public WallCell(double x, double y, char character, Raylib_cs.Color color) : base(x, y, character) {
            this.color = color;
        }
    }

    public class PlayerCell : RoomCell {
        public PlayerCell(double x, double y, char character) : base(x, y, character) {}
    }

    public class EmptyCell : RoomCell {
        public EmptyCell(double x, double y, char character) : base(x, y, character) { }
    }

    public abstract class Room : Global {
        public abstract List<List<char>> roomRaw { get; }
        public List<List<RoomCell>> room = new List<List<RoomCell>>(){};

        public abstract string name { get; }
        public List<Entity> entities = new List<Entity>();
        public Dictionary<WallCell, Raylib_cs.Color[,]> textures = new Dictionary<WallCell, Raylib_cs.Color[,]>();
        public Player? player = null;


        public Room() {
            var raw = roomRaw;
            room = new List<List<RoomCell>>();
            for (int i = 0; i < raw.Count; i++) {
                room.Add(new List<RoomCell>());
                for (int j = 0; j < raw[i].Count; j++) {
                    room[i].Add(ParseRoomChar(raw[i][j], j, i));
                }
            }
            Console.WriteLine("Printing room test");
            foreach (List<RoomCell> row in room) {
                Console.WriteLine();
                foreach (RoomCell col in row) {
                    Console.Write(col.character.ToString());
                }
            }
            foreach (Entity entity in entities) {
                if(entity is Player) {
                    player = (Player)entity;
                }
            }
        }

        public RoomCell ParseRoomChar(char character, int x, int y) {
            switch (character) {
                case '#':
                    return new BrickCell(x, y, character, Raylib_cs.Color.Black);
                case ' ':
                    return new EmptyCell(x, y, character);
                case '@':
                    return new PlayerCell(x, y, character);
                default:
                    throw new Exception("Unaccounted for room cell character.");
            }
        }

        public int getWidth() {
            return room[0].Count;
        }

        public int getHeight() {
            return room.Count;
        }

        

        public void LoadTextures() {
            textures = new Dictionary<WallCell, Raylib_cs.Color[,]>();
            foreach (List<RoomCell> row in room) {
                foreach (RoomCell cell in row) {
                    if (cell.isTextured) {
                        string texturePath = ((WallCell)cell).texture;
                        if (!File.Exists(texturePath)) {
                            throw new FileNotFoundException($"Texture file not found: {texturePath}. Current directory: {Directory.GetCurrentDirectory()}");
                        }
                        using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(texturePath)) {
                            int width = img.Width;
                            int height = img.Height;
                            Raylib_cs.Color[,] pixels = new Raylib_cs.Color[height, width];
                            for (int y = 0; y < height; y++) {
                                for (int x = 0; x < width; x++) {
                                    var c = img[x, y];
                                    pixels[y, x] = new Raylib_cs.Color(c.R, c.G, c.B, c.A);
                                }
                            }
                            textures.Add((WallCell)cell, pixels);
                        }
                    }
                }
            }
        }



        public abstract void Loop();
    }
}