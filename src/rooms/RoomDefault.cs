namespace LibCast {
    public class RoomDefault : Room {
        public override string name {get; set;} = "rmDefault";
        //public override List<List<char>> roomRaw {get; set;} = MazeGeneration.GenerateMaze(100, 100);
         public override List<List<char>>? roomAsArray {get; set;} = new List<List<char>>(){
            new List<char>{'#','#','#','#','#','#','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#',' ',' ',' ',' ','#','#'},
            new List<char>{'#',' ',' ',' ','#'},
            new List<char>{' ',' ','a',' ',' ','#','#','#','#','#'},
            new List<char>{'#',' ',' ',' ',' ','a',' ',' ',' ','#'},
            new List<char>{'#',' ','!','#',' ','a','#','#','#','#'},
            new List<char>{'#',' ','#','#',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ','a','#'},
            new List<char>{'#',' ',' ',' ',' ',' ','#'},
            new List<char>{'#','#','#','#','#','#','#'}};
        public override string? skyboxTexture {get; set;} = "sky.png";

        public RoomDefault() : base() {
            AddEntity(new PlayerEntity(2,2));
            AddEntity(new ShaiEntity(2, 3));
            AddEntity(new Camera(3,2));
            LoadTextures();
        }

        public override void Loop() {
            if(!UI.GameStateIsPlay()) return;
            LoopEntities();
        }
    }
}