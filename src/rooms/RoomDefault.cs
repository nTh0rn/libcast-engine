namespace LibCast {
    public class RoomDefault : Room {
        public override string name => "rmDefault";
        //public override List<List<char>> roomRaw => MazeGeneration.GenerateMaze(100, 100);
         public override List<List<char>> roomAsArray => new List<List<char>>(){
            new List<char>{'#','#','#','#','#','#','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ','*','*',' ','#','#'},
            new List<char>{'#',' ','*',' ','#'},
            new List<char>{' ',' ','a',' ',' ','#','#','#','#','#'},
            new List<char>{'#',' ',' ',' ',' ','a',' ',' ',' ','#'},
            new List<char>{'#',' ','!','#',' ','a','#','#','#','#'},
            new List<char>{'#',' ','#','#',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#','#','#','#','#','#','#'}};
        public override string? skyboxTexture => "src/assets/textures/sky.png";

        public RoomDefault() : base() {
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
            if(UI.gameState != GameState.PLAY) {
                return;
            }
            foreach (Entity entity in entitiesInRange) {
                entity.Loop();
            }
            //DrawTopDown(0,0);
        }
    }
}