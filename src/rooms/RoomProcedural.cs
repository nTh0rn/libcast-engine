namespace LibCast {
    public class RoomProcedural : Room {
        public override string name => "rmProcedural";
        public override List<List<char>> roomAsArray => new List<List<char>>();
        public override string? skyboxTexture => "src/assets/textures/sky.png";

        public RoomProcedural() : base(){
            //room[2][2] = new EmptyCell(2, 2, ' ');
            AddEntity(new Player(2,2));
            AddEntity(new JennEntity(2, 3));

            // for(int i = 0; i < getHeight(); i++) {
            //     for(int j = 0; j < getWidth(i); j++) {
            //         entities.Add(new TestEntity(j, i));
            //     }
            // }
            
            LoadTextures();
        }

        public override void Loop() {
            GenerateTerrain();
            UpdateEntitiesInRange();
            if(UI.gameState != GameState.PLAY) {
                return;
            }
            foreach(Entity entity in entitiesInRange) {
                entity.Loop();
            }
            //DrawTopDown(0,0);
        }

        private void GenerateTerrain() {

            if (player == null) return;

            (int x, int y) center = ((int)player.x, (int)player.y);
            // (int x, int y) centerChunk = (
            //     (center.x - ((center.x % chunkSize + chunkSize) % chunkSize)) / chunkSize,
            //     (center.y - ((center.y % chunkSize + chunkSize) % chunkSize)) / chunkSize
            // );

            // for (int cx = centerChunk.x - renderDistance; cx <= centerChunk.x + renderDistance; cx++) {
            //     for (int cy = centerChunk.y - renderDistance; cy <= centerChunk.y + renderDistance; cy++) {
            //         GenerateChunk(cx * chunkSize, cy * chunkSize);
            //     }
            // }

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
                    if(innerX == (int)player.x && innerY == (int)player.y) {
                        continue;
                    }
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
        public static int Mod(int value, int modulus) {
            return ((value % modulus) + modulus) % modulus;
        }
    }
}