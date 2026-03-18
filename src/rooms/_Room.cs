
namespace LibCast {
    using Newtonsoft.Json;
    using System.Text.Json.Serialization;
    using System.Reflection;

    public class RoomData
    {
        public string? name { get; set; }

        public List<CellData>? cells { get; set; }

        public List<EntityData>? entities { get; set; }

        public int[][]? map { get; set; }
    }

    public class CellData
    {
        public string? type { get; set; }

        public int id { get; set; }

        public List<Dictionary<string, string>>? textures { get; set; }
    }

    public class EntityData
    {
        public string? type { get; set; }

        public List<object>? param { get; set; }

        public int count { get; set; } = 1;
    }

    

    public abstract class Room {
        public virtual List<List<char>>? roomAsArray { get; set; }
        public Dictionary<(int worldX, int worldY), RoomCell> room = new Dictionary<(int, int), RoomCell>();
        public virtual string? skyboxTexture {get; set;}
        public abstract string name { get; set; }
        public Dictionary<(int chunkX, int chunkY), List<Entity>> entities = new Dictionary<(int, int), List<Entity>>();
        public List<Entity> entitiesInRange = new List<Entity>();
        public Dictionary<string, TextureBitmap> textureBitmaps = new Dictionary<string, TextureBitmap>();
        public PlayerEntity? player = null;
        public virtual int chunkSize {get; set;} = 8;
        public virtual int renderDistance {get; set;} = Raycaster.maxRaySurfaces*2/8;


        public Room() {
            if(roomAsArray != null) {
                var raw = roomAsArray;
                room = new Dictionary<(int, int), RoomCell>();
                for(int y = 0; y < raw.Count; y++) {
                    for(int x = 0; x < raw[y].Count; x++) {
                        room.Add((x, y), ParseRoomChar(raw[y][x], x, y));
                    }
                }
            }
            foreach (var entityList in entities) {
                foreach(Entity entity in entityList.Value) {
                    if(entity is PlayerEntity) {
                        player = (PlayerEntity)entity;
                    }
                }
            }
        }

        public void LoadRoomFromJson(string jsonPath) {
            if (!File.Exists(jsonPath)) {
                throw new Exception($"Room JSON file not found: {jsonPath}");
            }

            RoomData? roomData = JsonConvert.DeserializeObject<RoomData>(File.ReadAllText(jsonPath));

            if (roomData == null) {
                throw new Exception($"Failed to deserialize room data from {jsonPath}");
            }
            LoadTexturesFromJson(roomData.cells);
            BuildRoomFromJson(roomData.map, roomData.cells);
            SpawnEntitiesFromData(roomData.entities);
        }

        private void LoadTexturesFromJson(List<CellData>? cellDataList) {
            if (cellDataList == null) return;

            foreach (CellData cellData in cellDataList) {
                if (cellData.textures == null) continue;

                foreach (Dictionary<string, string> textureDict in cellData.textures) {
                    foreach (var kvp in textureDict) {
                        LoadTextureFromPath(kvp.Value);
                    }
                }
            }
        }

        private void BuildRoomFromJson(int[][]? map, List<CellData>? cellDataList) {
            if (map == null || cellDataList == null) return;

            room = new Dictionary<(int, int), RoomCell>();

            // Create a mapping of cell IDs to cell data for quick lookup
            Dictionary<int, CellData> cellDataMap = new Dictionary<int, CellData>();
            foreach (CellData cellData in cellDataList) {
                cellDataMap[cellData.id] = cellData;
            }

            // Build the room from the map
            for (int y = 0; y < map.Length; y++) {
                for (int x = 0; x < map[y].Length; x++) {
                    int cellId = map[y][x];

                    // Skip empty/no-cell markers (e.g., -1)
                    if (cellId < 0) continue;

                    if (cellDataMap.ContainsKey(cellId)) {
                        CellData cellData = cellDataMap[cellId];
                        if (cellData.type == null) continue;
                        RoomCell cell = CreateRoomCell(cellData.type, x, y);
                        ApplyTexturesToCell(cell, cellData.textures);
                        room.Add((x, y), cell);
                    }
                }
            }
        }

        private RoomCell CreateRoomCell(string cellTypeName, int x, int y) {
            // Try to find the cell type in the current namespace
            Type? cellType = Type.GetType($"LibCast.{cellTypeName}") ?? Type.GetType(cellTypeName);

            if (cellType == null) {
                throw new Exception($"Cell type '{cellTypeName}' not found. Make sure the class exists in LibCast namespace.");
            }

            // Create an instance with constructor parameters (double x, double y)
            object[] constructorParams = { (double)x, (double)y };
            object? cellObj = Activator.CreateInstance(cellType, constructorParams);
            RoomCell cell = (RoomCell)cellObj!;

            if (cell == null) {
                throw new Exception($"Failed to create instance of cell type '{cellTypeName}'");
            }

            return cell;
        }

        private void ApplyTexturesToCell(RoomCell cell, List<Dictionary<string, string>>? textures) {
            if (textures == null) return;

            TextureMap textureMap = cell.texture;

            foreach (Dictionary<string, string> textureDict in textures) {
                foreach (var kvp in textureDict) {
                    string faceName = kvp.Key;
                    string texturePath = kvp.Value;

                    // Dynamically set the texture field on the struct
                    FieldInfo? fieldInfo = textureMap.GetType().GetField(faceName);
                    if (fieldInfo != null) {
                        fieldInfo.SetValue(textureMap, texturePath);
                    }
                }
            }

            // Reassign the modified struct back to the cell
            cell.texture = textureMap;
        }

        private void SpawnEntitiesFromData(List<EntityData>? entityDataList) {
            if (entityDataList == null) return;

            foreach (EntityData entityData in entityDataList) {
                if (entityData.type == null) continue;
                int spawnCount = entityData.count;

                for (int i = 0; i < spawnCount; i++) {
                    Entity entity = CreateEntity(entityData.type, entityData.param);
                    if (entity != null) {
                        AddEntity(entity);
                    }
                }
            }
        }

        private Entity CreateEntity(string entityTypeName, List<object>? parameters) {
            // Try to find the entity type in the current namespace
            Type? entityType = Type.GetType($"LibCast.{entityTypeName}") ?? Type.GetType(entityTypeName);

            if (entityType == null) {
                throw new Exception($"Entity type '{entityTypeName}' not found. Make sure the class exists in LibCast namespace.");
            }

            // Convert parameters to appropriate types and find matching constructor
            object[] constructorParams = parameters?.ToArray() ?? Array.Empty<object>();
            
            // Try to find a constructor that matches the parameter count and types
            var constructors = entityType.GetConstructors();
            ConstructorInfo? matchingConstructor = null;

            // First try exact type matching
            foreach (var ctor in constructors) {
                var ctorParams = ctor.GetParameters();
                if (ctorParams.Length == constructorParams.Length) {
                    bool match = true;
                    for (int i = 0; i < ctorParams.Length; i++) {
                        if (!IsCompatibleType(constructorParams[i], ctorParams[i].ParameterType)) {
                            match = false;
                            break;
                        }
                    }
                    if (match) {
                        matchingConstructor = ctor;
                        break;
                    }
                }
            }

            if (matchingConstructor == null) {
                throw new Exception($"No suitable constructor found for entity type '{entityTypeName}' with {constructorParams.Length} parameters");
            }

            // Convert parameters to the correct types
            object[] convertedParams = new object[constructorParams.Length];
            var matchingParams = matchingConstructor.GetParameters();
            for (int i = 0; i < constructorParams.Length; i++) {
                convertedParams[i] = Convert.ChangeType(constructorParams[i], matchingParams[i].ParameterType);
            }

            object? entityObj = Activator.CreateInstance(entityType, convertedParams);
            Entity entity = (Entity)entityObj!;

            if (entity == null) {
                throw new Exception($"Failed to create instance of entity type '{entityTypeName}'");
            }

            return entity;
        }

        private bool IsCompatibleType(object value, Type targetType) {
            if (value == null) return !targetType.IsValueType;
            
            Type valueType = value.GetType();
            if (valueType == targetType) return true;
            
            // Check for numeric type compatibility
            if (targetType.IsAssignableFrom(valueType)) return true;
            
            // Try conversion
            try {
                Convert.ChangeType(value, targetType);
                return true;
            } catch {
                return false;
            }
        }

        public void LoadRoomFromStructure(string path, int x, int y) {
            
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
            if(!textureString.StartsWith("src/assets/textures/") && !textureString.StartsWith("/src/assets/textures")) {
                if(textureString.StartsWith('/')) {
                    textureString = "src/assets/textures"+textureString;
                } else {
                    textureString = "src/assets/textures/"+textureString;
                }
            }
            return textureBitmaps[textureString];
        }


        

        private void LoadTextureFromPath(string texturePath) {
            if(texturePath == null) return;
            if(!texturePath.StartsWith("src/assets/textures/") && !texturePath.StartsWith("/src/assets/textures")) {
                if(texturePath.StartsWith('/')) {
                    texturePath = "src/assets/textures" + texturePath;
                } else {
                    texturePath = "src/assets/textures/" + texturePath;
                }
            }
            if (textureBitmaps.ContainsKey(texturePath)) {
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
                // Normalize to relative path starting with "src/assets/textures/"
                if (textureFiles[i].Contains("src/assets/textures/")) {
                    textureFiles[i] = textureFiles[i].Substring(textureFiles[i].IndexOf("src/assets/textures/"));
                }
            }

            foreach(string texture in textureFiles) {
                LoadTextureFromPath(texture);
            }

        }

        public string GetArrow(double dir) {
            if(dir < 30 || dir > 285) {
                return "→";
            }
            return "->";
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
            if(player == null) return returnEntities;
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
            if(entity is PlayerEntity) {
                player = (PlayerEntity)entity;
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
                if(entity is PlayerEntity) continue;
                entity.Loop();
            }
        }



        public abstract void Loop();
    }
}