namespace LibCast {
    public class RoomDefault : Room {
        public override string name => "rmDefault";
        //public override List<List<char>> roomRaw => MazeGeneration.GenerateMaze(100, 100);
         public override List<List<char>> roomRaw => new List<List<char>>(){
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
    }
}