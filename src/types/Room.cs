
namespace LibCast {

    public abstract class RoomCell {
        public char character;
        public double x;
        public double y;
        public string? floorTexture = null;
        public string? ceilingTexture = null;

        public RoomCell(double x, double y, char character) {
            this.x = x;
            this.y = y;
            this.character = character;
        }
    }

    public class WallCell : RoomCell {
        public Color color { get; set; }
        public virtual string? texture { get; set; }
        public WallCell(double x, double y, char character, Color color) : base(x, y, character) {
            this.color = color;
        }
    }

    public class PlayerCell : RoomCell {
        public PlayerCell(double x, double y, char character) : base(x, y, character) {}
    }

    public class EmptyCell : RoomCell {
        public EmptyCell(double x, double y, char character) : base(x, y, character) {
            floorTexture = "src/assets/textures/minecraft.png";
            ceilingTexture = "src/assets/textures/wall_brick.png";
        }
    }

    public class SpecialFloorCell : EmptyCell {
        public SpecialFloorCell(double x, double y, char character) : base(x, y, character) {
            floorTexture = "src/assets/textures/wall_old.png";
            ceilingTexture = "src/assets/textures/shai.png";
        }
    }

    public abstract class Room {
        public abstract List<List<char>> roomRaw { get; }
        public List<List<RoomCell>> room = new List<List<RoomCell>>(){};

        public abstract string name { get; }
        public List<Entity> entities = new List<Entity>();
        public Dictionary<string, Color[,]> textures = new Dictionary<string, Color[,]>();
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
                    return new BrickCell(x, y, character, Color.Black);
                case '!':
                    return new TallCell(x, y, character, Color.Black);
                case ' ':
                    return new EmptyCell(x, y, character);
                case '*':
                    return new SpecialFloorCell(x, y, character);
                case '@':
                    return new PlayerCell(x, y, character);
                default:
                    throw new Exception("Unaccounted for room cell character.");
            }
        }

        public int getWidth(int y) {
            return room[y].Count;
        }

        public int getHeight() {
            return room.Count;
        }

        

        private void LoadTextureFromPath(string? texturePath) {
            if (texturePath == null || textures.ContainsKey(texturePath)) {
                return;
            }
            
            if (!File.Exists(texturePath)) {
                throw new FileNotFoundException($"Texture file not found: {texturePath}. Current directory: {Directory.GetCurrentDirectory()}");
            }
            
            var img = Raylib.LoadImage(texturePath);
            int width = img.Width;
            int height = img.Height;
            
            Color[,] pixels = new Color[height, width];
            
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    pixels[y, x] = Raylib.GetImageColor(img, x, y);
                }
            }
            
            Raylib.UnloadImage(img);
            textures.Add(texturePath, pixels);
        }

        public void LoadTextures() {
            textures = new Dictionary<string, Color[,]>();
            
            // Load wall textures
            foreach (List<RoomCell> row in room) {
                foreach (RoomCell cell in row) {
                    if (cell is WallCell) {
                        LoadTextureFromPath(((WallCell)cell).texture);
                    }
                }
            }

            // Load entity textures
            foreach (Entity entity in entities) {
                LoadTextureFromPath(entity.texture);
            }
            
            // Load floor and ceiling textures from all cells
            foreach (List<RoomCell> row in room) {
                foreach (RoomCell cell in row) {
                    LoadTextureFromPath(cell.floorTexture);
                    LoadTextureFromPath(cell.ceilingTexture);
                }
            }
        }

        public void DrawTopDown(int x, int y) {
            for(int i = 0; i < room.Count; i++) {
                for(int j = 0; j < room[i].Count; j++) {
                    RoomCell cell = room[i][j];
                    Color cellColor = new Color(cell.character*10 % 255, cell.character*100 % 255, cell.character*1000 % 255);
                    Screen.Fill(cellColor);
                    Screen.DrawRect(x+j*3, y+i*3, 3, 3);
                }
            }

            if(player != null) {
                Screen.Fill(Color.Blue);
                //Screen.DrawPixel(x+(((int)player.x)*3)+1, y+(((int)player.y)*3)+1)
                Screen.DrawRect(x+(((int)player.x)*3)+1, y+(((int)player.y)*3)+1, 1, 1);
                //Screen.DrawPixel(x+(((int)player.x)*3)-1, y+(((int)player.y)*3)-1);
                Screen.DrawText(GetArrow(player.direction % 360), x+(((int)player.x)*3)+1, y+(((int)player.y)*3)+1);
            }
        }

        public string GetArrow(double dir) {
            return "->";
            if(dir < 30 || dir > 285) {
                return "→";
            }
        }



        public abstract void Loop();
    }
}