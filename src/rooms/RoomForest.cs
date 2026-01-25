namespace LibCast {
    public class RoomForest : Room {
        public override string name => "rmForest";
        public override List<List<char>> roomAsArray => GenerateForest(100,100, 1000);
        public override string? skyboxTexture => "src/assets/textures/sky.png";

        public RoomForest() : base(){
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
            UpdateEntitiesInRange();
            if(UI.gameState != GameState.PLAY) {
                return;
            }
            foreach(Entity entity in entitiesInRange) {
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


            return cells;
        }
    }
}