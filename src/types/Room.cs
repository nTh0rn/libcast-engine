
namespace LibCast {


    public struct Texture {
        public Color[,] texture;
        public int height;

        public Texture(Color[,] texture) {
            this.texture = texture;
            height = texture.GetLength(0);
        }
    }

    public struct TextureMap {
        public string? westOut, eastOut, northOut, southOut, westIn, eastIn, northIn, southIn, top, bottom = null;

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
        public double x;
        public double y;
        public TextureMap texture;
        public bool stopRay = false;

        public RoomCell(double x, double y) {
            this.x = x;
            this.y = y;
            texture = new TextureMap();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public void SetPosition(int x, int y) {
            this.x = (double)x;
            this.y = (double)y;
        }


    }

    public class WallCell : SolidCell {
        public WallCell(double x, double y) : base(x, y) {
        }
    }

    public class PlayerCell : RoomCell {
        public PlayerCell(double x, double y) : base(x, y) {}
    }

    public class SolidCell : RoomCell {
        public SolidCell(double x, double y) : base(x, y) {}
    }

    public class EmptyCell : RoomCell {
        public EmptyCell(double x, double y) : base(x, y) {
            //floorTexture = "src/assets/textures/dark_cobblestone.png";
            //ceilingTexture = "src/assets/textures/dark_brick_wall.png";
            texture.bottom = "src/assets/textures/grass.png";
        }
    }

    public class SpecialFloorCell : EmptyCell {
        public SpecialFloorCell(double x, double y) : base(x, y) {
            texture = new TextureMap("src/assets/textures/wall_old.png", "src/assets/textures/wall_brick.png");
        }
    }

    public abstract class Room {
        public abstract List<List<char>> roomAsArray { get; }
        public Dictionary<(int worldX, int worldY), RoomCell> room = new Dictionary<(int, int), RoomCell>();
        public abstract string? skyboxTexture {get;}
        public abstract string name { get; }
        public Dictionary<(int chunkX, int chunkY), List<Entity>> entities = new Dictionary<(int, int), List<Entity>>();
        public List<Entity> entitiesInRange = new List<Entity>();
        public Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        public Player? player = null;
        public int chunkSize = 8;
        public int renderDistance = Raycaster.maxRaySurfaces*2/8;


        public Room() {
            
            var raw = roomAsArray;
            room = new Dictionary<(int, int), RoomCell>();
            for(int y = 0; y < raw.Count; y++) {
                for(int x = 0; x < raw[y].Count; x++) {
                room.Add((x, y), ParseRoomChar(raw[y][x], x, y));
                }
            }
            foreach (var entityList in entities) {
                foreach(Entity entity in entityList.Value) {
                    if(entity is Player) {
                        player = (Player)entity;
                    }
                }
            }
        }

        public RoomCell ParseRoomChar(char character, int x, int y) {
            switch (character) {
                case '#':
                    return new DarkBrickWall(x, y);
                case '!':
                    return new TallCell(x, y);
                case 'a':
                    return new DoorCell(x, y);
                case ' ':
                    return new EmptyCell(x, y);
                case '*':
                    return new SpecialFloorCell(x, y);
                case '@':
                    return new PlayerCell(x, y);
                default:
                    throw new Exception("Unaccounted for room cell character.");
            }
        }

        

        private void LoadTextureFromPath(string texturePath) {
            if (texturePath == null || textures.ContainsKey(texturePath)) {
                return;
            }
            
            if (!File.Exists(texturePath)) {
                texturePath = "src/assets/textures/error.png";
            }
            
            var img = Raylib.LoadImage(texturePath);
            int width = img.Width;
            int height = img.Height;

            // if(width % 256 != 0) {
            //     img = Raylib.LoadImage("src/assets/textures/error_wrong_size.png");
            //     width = 256;
            //     height = 256;
            // }

            // if(width % 256 != 0 || height % 256 != 0) {
            //     img = Raylib.LoadImage("src/assets/textures/error.png");
            //     width = 256;
            //     height = 256;
            //     return;
            // }
            
            Color[,] pixels = new Color[height, width];
            
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    pixels[y, x] = Raylib.GetImageColor(img, x, y);
                }
            }
            
            Raylib.UnloadImage(img);

            textures.Add(texturePath, new Texture(pixels));
        }

        public void LoadTextures() {
            textures = new Dictionary<string, Texture>();

            string[] textureFiles = 
            Directory.GetFiles("src/assets/textures", "*.png", SearchOption.AllDirectories);

            for(int i = 0; i < textureFiles.Count(); i++) {
                textureFiles[i] = textureFiles[i].Replace("\\", "/");
                textureFiles[i] = textureFiles[i].Replace("\\", "/");
            }

            foreach(string texture in textureFiles) {
                LoadTextureFromPath(texture);
            }

            // // Load wall textures
            // foreach (var cell in room) {
            //     LoadTextureFromPath(cell.Value.texture.top);
            //     LoadTextureFromPath(cell.Value.texture.bottom);
            //     LoadTextureFromPath(cell.Value.texture.westOut);
            //     LoadTextureFromPath(cell.Value.texture.eastOut);
            //     LoadTextureFromPath(cell.Value.texture.northOut);
            //     LoadTextureFromPath(cell.Value.texture.southOut);
            //     LoadTextureFromPath(cell.Value.texture.westIn);
            //     LoadTextureFromPath(cell.Value.texture.eastIn);
            //     LoadTextureFromPath(cell.Value.texture.northIn);
            //     LoadTextureFromPath(cell.Value.texture.southIn);
            // }

            // // Load entity textures
            // foreach (Entity entity in entities) {
            //     LoadTextureFromPath(entity.texture);
            // }

            // if(skyboxTexture != null) {
            //     LoadTextureFromPath(skyboxTexture);
            // }
        }

        public void DrawTopDown(int x, int y) {
            foreach (var cell in room) {
                //Color cellColor = new Color(cell.Value*10 % 255, cell.Value.character*100 % 255, cell.Value.character*1000 % 255);
                //Screen.Fill(cellColor);
                Screen.DrawRect(x+cell.Key.Item1*3, y+cell.Key.Item2*3, 3, 3);
            }

            if(player != null) {
                Screen.Fill(Color.Blue);
                Screen.DrawRect(x+(((int)player.x)*3)+1, y+(((int)player.y)*3)+1, 1, 1);
                Screen.DrawText(GetArrow(player.direction % 360), x+(((int)player.x)*3)+1, y+(((int)player.y)*3)+1);
            }
        }

        public string GetArrow(double dir) {
            return "->";
            if(dir < 30 || dir > 285) {
                return "→";
            }
        }

        public void UpdateEntitiesInRange() {
            entitiesInRange.Clear();
            foreach((int, int) coord in GetEntitiesInRange()) {
                foreach(Entity entity in entities[coord]) {
                    entitiesInRange.Add(entity);
                }
            }
        }

        public List<(int, int)> GetEntitiesInRange() {
            List<(int, int)> returnEntities = new List<(int, int)>();
            (int x, int y) playerChunk = CoordinateToChunk(player.x, player.y);

            for(int x = playerChunk.x-renderDistance; x < playerChunk.x+renderDistance; x++) {
                for(int y = playerChunk.y-renderDistance; y < playerChunk.y+renderDistance; y++) {
                    if(entities.ContainsKey((x, y))) {
                        returnEntities.Add((x, y));
                    }
                }
            }
            return returnEntities;
        }


        public (int x, int y) CoordinateToChunk(double x, double y) {
            return FloorWorldCoords(x / chunkSize, y / chunkSize);
        }

        public void AddEntity(Entity entity) {
            (int, int) chunk = CoordinateToChunk(entity.x, entity.y);
            if(!entities.ContainsKey(chunk)) {
                entities.Add(chunk, new List<Entity>(){entity});
            } else {
                entities[chunk].Add(entity);
            }
            if(entity is Player) {
                player = (Player)entity;
            }
        }

        public void LoopEntities() {
            UpdateEntitiesInRange();
            if(UI.gameState != GameState.PLAY) {
                return;
            }
            if(player != null) {
                player.Loop();
            }
            foreach(Entity entity in entitiesInRange) {
                if(entity is Player) continue;
                entity.Loop();
            }
        }



        public abstract void Loop();
    }
}