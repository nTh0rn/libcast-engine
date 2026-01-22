namespace LibCast {
    public class RoomForest : Room {
        public override string name => "rmForest";
        public override List<List<char>> roomAsArray => GenerateForest(100,100, 500);
        public override string? skyboxTexture => "src/assets/textures/sky.png";

        public RoomForest() : base(){
            //room[2][2] = new EmptyCell(2, 2, ' ');
            entities.Add(new Player(2, 2));
            for(int i = 0; i < 1; i++) {
                entities.Add(new JennEntity(2, 3));
            }

            

            // for(int i = 0; i < getHeight(); i++) {
            //     for(int j = 0; j < getWidth(i); j++) {
            //         entities.Add(new TestEntity(j, i));
            //     }
            // }

            foreach (Entity entity in entities) {
                if (entity is Player) {
                    player = (Player)entity;
                }
            }

            LoadTextures();


        }

        public override void Loop() {
            if(UI.gameState != GameState.PLAY) {
                return;
            }
            foreach (Entity entity in entities) {
                entity.Loop();
            }
            //DrawTopDown(0,0);
        }

        private List<List<char>> GenerateForest(int width, int height, int treeCount=100) {
            List<List<char>> cells = new List<List<char>>();
            Stack<MazeCell> stack = new Stack<MazeCell>();
            Random r = new Random();
            for (int i = 0; i < height; i++) {
                cells.Add(new List<char>());
                for (int j = 0; j < width; j++) {
                    if(i == 0 || i == height-1 || j == 0 || j == width-1) {
                        cells[i].Add('#');
                    } else {
                        cells[i].Add(' ');
                    }
                }
            }

            for(int i = 0; i < treeCount; i++) {
                entities.Add(new TreeEntity(r.Next(width), r.Next(height)));
            }

            return cells;
        }
    }
}