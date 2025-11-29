namespace LibCast {
    public class RoomDefault : Room {
        public override string name => "rmDefault";
        //public override List<List<char>> roomRaw => MazeGeneration.GenerateMaze(100, 100);
         public override List<List<char>> roomRaw => new List<List<char>>(){
            new List<char>{'#','#','#','#','#','#','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ','#','#'},
            new List<char>{'#',' ',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ','#','#','#','#','#'},
            new List<char>{'#',' ',' ',' ',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ','#','#',' ',' ','#','#','#','#'},
            new List<char>{'#',' ','#','#',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#','#','#','#','#','#','#'}};



        public RoomDefault() : base() {
            //room[2][2] = new EmptyCell(2, 2, ' ');
            entities.Add(new Player(2, 2));
            for(int i = 0; i < 100; i++) {
            entities.Add(new TestEntity(2, 3));
            entities.Add(new TestEntity(2, 4));

            entities.Add(new TestEntity(2.3, 3));
            entities.Add(new TestEntity(2, 3.5));
            }

            foreach (Entity entity in entities) {
                if (entity is Player) {
                    player = (Player)entity;
                }
            }

            LoadTextures();

        }

        public override void Loop() {
            foreach (Entity entity in entities) {
                entity.Loop();
            }
        }
    }
}