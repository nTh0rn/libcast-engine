
namespace LibCast {

    public struct TextureMap {
        public string? westOut = null;
        public string? eastOut = null;
        public string? northOut = null;
        public string? southOut = null;
        public string? westIn = null;
        public string? eastIn = null;
        public string? northIn = null;
        public string? southIn = null;
        public string? top = null;
        public string? bottom = null;

        public TextureMap(string? top = null, string? bottom = null, string? westOut = null, string? eastOut = null, string? northOut = null, string? southOut = null, string? westIn = null, string? eastIn = null, string? northIn = null, string? southIn = null) {
            this.top = top;
            this.bottom = bottom;
            this.westOut = westOut;
            this.eastOut = eastOut;
            this.northOut = northOut;
            this.southOut = southOut;
            this.westIn = westIn;
            this.eastIn = eastIn;
            this.northIn = northIn;
            this.southIn = southIn;
        }

        public void SetOutTextures(string texture) {
            westOut = texture;
            eastOut = texture;
            northOut = texture;
            southOut = texture;
        }

        public void SetInTextures(string texture) {
            westIn = texture;
            eastIn = texture;
            northIn = texture;
            southIn = texture;
        }

        public void SetAllWalls(string texture) {
            SetInTextures(texture);
            SetOutTextures(texture);
        }

        public void SetBottomTexture(string texture) {
            bottom = texture;
        }

        public void SetTopTexture(string texture) {
            top = texture;
        }

    }

    public abstract class RoomCell {
        public char character;
        public double x;
        public double y;
        public TextureMap texture;
        public bool stopRay = false;

        public RoomCell(double x, double y, char character) {
            this.x = x;
            this.y = y;
            this.character = character;
            texture = new TextureMap();
        }
    }

    public class WallCell : SolidCell {
        public WallCell(double x, double y, char character) : base(x, y, character) {
        }
    }

    public class PlayerCell : RoomCell {
        public PlayerCell(double x, double y, char character) : base(x, y, character) {}
    }

    public class SolidCell : RoomCell {
        public SolidCell(double x, double y, char character) : base(x, y, character) {}
    }

    public class EmptyCell : RoomCell {
        public EmptyCell(double x, double y, char character) : base(x, y, character) {
            //floorTexture = "src/assets/textures/dark_cobblestone.png";
            //ceilingTexture = "src/assets/textures/dark_brick_wall.png";
            texture.bottom = "src/assets/textures/grass.png";
        }
    }

    public class SpecialFloorCell : EmptyCell {
        public SpecialFloorCell(double x, double y, char character) : base(x, y, character) {
            texture = new TextureMap("src/assets/textures/wall_old.png", "src/assets/textures/wall_brick.png");
        }
    }

    public abstract class Room {
        public abstract List<List<char>> roomAsArray { get; }
        public Dictionary<(int, int), RoomCell> room = new Dictionary<(int, int), RoomCell>();
        public abstract string? skyboxTexture {get;}
        public abstract string name { get; }
        public List<Entity> entities = new List<Entity>();
        public Dictionary<string, Color[,]> textures = new Dictionary<string, Color[,]>();
        public Player? player = null;


        public Room() {
            
            var raw = roomAsArray;
            room = new Dictionary<(int, int), RoomCell>();
            for(int y = 0; y < raw.Count; y++) {
                for(int x = 0; x < raw[y].Count; x++) {
                room.Add((x, y), ParseRoomChar(raw[y][x], x, y));
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
                    return new DarkBrickWall(x, y, character);
                case '!':
                    return new TallCell(x, y, character);
                case 'a':
                    return new DoorCell(x, y, character);
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
            foreach (var cell in room) {
                LoadTextureFromPath(cell.Value.texture.top);
                LoadTextureFromPath(cell.Value.texture.bottom);
                LoadTextureFromPath(cell.Value.texture.westOut);
                LoadTextureFromPath(cell.Value.texture.eastOut);
                LoadTextureFromPath(cell.Value.texture.northOut);
                LoadTextureFromPath(cell.Value.texture.southOut);
                LoadTextureFromPath(cell.Value.texture.westIn);
                LoadTextureFromPath(cell.Value.texture.eastIn);
                LoadTextureFromPath(cell.Value.texture.northIn);
                LoadTextureFromPath(cell.Value.texture.southIn);
            }

            // Load entity textures
            foreach (Entity entity in entities) {
                LoadTextureFromPath(entity.texture);
            }

            if(skyboxTexture != null) {
                LoadTextureFromPath(skyboxTexture);
            }
        }

        public void DrawTopDown(int x, int y) {
            foreach (var cell in room) {
                Color cellColor = new Color(cell.Value.character*10 % 255, cell.Value.character*100 % 255, cell.Value.character*1000 % 255);
                Screen.Fill(cellColor);
                Screen.DrawRect(x+cell.Key.Item1*3, y+cell.Key.Item2*3, 3, 3);
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