namespace LibCast {
    public class RoomProcedural : Room {
        public override string name {get; set;} = "rmProcedural";
        public override List<List<char>>? roomAsArray {get; set;} = new List<List<char>>();
        public override string? skyboxTexture {get; set;} = "sky.png";

        public RoomProcedural() : base(){
            AddEntity(new PlayerEntity(4,4));
            AddEntity(new ShaiEntity(2, 3));
            AddEntity(new BadBunnieEntity(3, 3, "no"));

            AddEntity(new Camera(0, 0));
            LoadTextures();
        }

        public override void Loop() {
            if(!UI.GameStateIsPlay()) return;
            GenerateTerrain();
            LoopEntities();
        }

        private void GenerateTerrain() {
            
            if (player == null) return;

            (int x, int y) center = (FloorWorldCoord(player.x), FloorWorldCoord(player.y));
            (int x, int y) centerChunk = CoordinateToChunk(center.x, center.y);

            for(int chunkX = centerChunk.x - renderDistance; chunkX <= centerChunk.x + renderDistance; chunkX++) {
                for(int chunkY = centerChunk.y - renderDistance; chunkY <= centerChunk.y + renderDistance; chunkY++) {
                    if(!room.ContainsKey((chunkX * chunkSize, chunkY * chunkSize))) {
                        GenerateChunk(chunkX, chunkY);
                    }
                }
            }
        }

        private void GenerateChunk(int chunkX, int chunkY) {
            for (int innerX = chunkX * chunkSize; innerX < chunkX * chunkSize + chunkSize; innerX++) {
                for (int innerY = chunkY * chunkSize; innerY < chunkY * chunkSize + chunkSize; innerY++) {
                    


                    if(room.ContainsKey((innerX, innerY))) {
                        throw new Exception("Overgenerating terrain!");
                    }

                    if(rng.Next(100) < 95) {
                        room.Add((innerX, innerY), new EmptyCell(innerX, innerY));
                    } else {
                        room.Add((innerX, innerY), new DoorCell(innerX, innerY));
                    }

                    int entityType = rng.Next(100);
                    
                    if(entityType < 5) {
                        AddEntity(new TreeEntity(innerX, innerY));
                    } else if(entityType < 10) {
                        AddEntity(new RockEntity(innerX, innerY));
                    }
                }
            }
        }
    }
}