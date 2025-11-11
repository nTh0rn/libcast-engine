namespace LibCast {
    public class RoomDefault : Room {
        public override string name => "rmDefault";
        public override List<List<char>> roomRaw => MazeGeneration.GenerateMaze(30, 15);
        // public override List<List<char>> roomRaw => new List<List<char>>(){
        //     new List<char>{'#','#','#','#','#','#','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ','#','#',' ',' ','#'},
        //     new List<char>{'#',' ','#','#',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#',' ',' ',' ',' ',' ','#'},
        //     new List<char>{'#','#','#','#','#','#','#'}};



        public RoomDefault() : base() {
            LoadTextures();
            //room[2][2] = new EmptyCell(2, 2, ' ');
            entities.Add(new Player(2, 2));
            foreach (Entity entity in entities) {
                if (entity is Player) {
                    player = (Player)entity;
                }
            }
        }

        public override void Loop() {
            foreach (Entity entity in entities) {
                entity.Loop();
            }
        }
    }
}