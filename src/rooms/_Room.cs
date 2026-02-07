
namespace LibCast {
    public abstract class Room {
        public virtual List<List<char>> roomAsArray { get; set; }
        public Dictionary<(int worldX, int worldY), RoomCell> room = new Dictionary<(int, int), RoomCell>();
        public virtual string? skyboxTexture {get; set;}
        public abstract string name { get; set; }
        public Dictionary<(int chunkX, int chunkY), List<Entity>> entities = new Dictionary<(int, int), List<Entity>>();
        public List<Entity> entitiesInRange = new List<Entity>();
        public Dictionary<string, TextureBitmap> textureBitmaps = new Dictionary<string, TextureBitmap>();
        public Player? player = null;
        public virtual int chunkSize {get; set;} = 8;
        public virtual int renderDistance {get; set;} = Raycaster.maxRaySurfaces*2/8;


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
                default:
                    throw new Exception("Unaccounted for room cell character.");
            }
        }

    
        public TextureBitmap GetTexture(string textureString) {
            return textureBitmaps[textureString];
        }
        

        private void LoadTextureFromPath(string texturePath) {
            if (texturePath == null || textureBitmaps.ContainsKey(texturePath)) {
                return;
            }
            
            if (!File.Exists(texturePath)) {
                texturePath = "src/assets/textures/error.png";
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

            textureBitmaps.Add(texturePath, new TextureBitmap(pixels));
        }

        public void LoadTextures() {
            textureBitmaps = new Dictionary<string, TextureBitmap>();

            string[] textureFiles = 
            Directory.GetFiles("src/assets/textures", "*.png", SearchOption.AllDirectories);

            for(int i = 0; i < textureFiles.Count(); i++) {
                textureFiles[i] = textureFiles[i].Replace("\\", "/");
                textureFiles[i] = textureFiles[i].Replace("\\", "/");
            }

            foreach(string texture in textureFiles) {
                LoadTextureFromPath(texture);
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