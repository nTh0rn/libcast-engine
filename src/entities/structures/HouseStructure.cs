namespace LibCast {
    public class HouseStructure : Structure {
        int x;
        int y;
        int houseWidth = 5;
        int houseHeight = 5;

        
        public HouseStructure(int x, int y) : base() {
            this.x = x;
            this.y = y;
        }

        public HouseStructure(double x, double y) {
            this.x = FloorWorldCoord(x);
            this.y = FloorWorldCoord(y);
            Console.WriteLine("Building at " + x + " " + y);
        }

        public void Build() {
            Console.WriteLine("Building at " + x + " " + y);
            Game.room.AddEntity(new RockEntity(10, 10));
            WallCell wallTall = new WallCell(x, y);
            wallTall.texture.SetAllWalls("src/assets/textures/rock_wall_tall.png");

            WallCell cornerLeftNorth = new WallCell(x, y);
            cornerLeftNorth.texture.SetAllWalls("src/assets/textures/wood_wall.png");
            cornerLeftNorth.texture.northOut = "src/assets/textures/wood_wall_slope_left.png";
            cornerLeftNorth.texture.northIn = "src/assets/textures/wood_wall_slope_right.png";
            cornerLeftNorth.texture.southOut = "src/assets/textures/wood_wall_slope_right.png";
            cornerLeftNorth.texture.southIn = "src/assets/textures/wood_wall_slope_left.png";


            WallCell cornerRightNorth = new WallCell(x, y);
            cornerRightNorth.texture.SetAllWalls("src/assets/textures/wood_wall.png");
            cornerRightNorth.texture.northOut = "src/assets/textures/wood_wall_slope_right.png";
            cornerRightNorth.texture.northIn = "src/assets/textures/wood_wall_slope_left.png";
            cornerRightNorth.texture.southIn = "src/assets/textures/wood_wall_slope_right.png";
            cornerRightNorth.texture.southOut = "src/assets/textures/wood_wall_slope_left.png";

            WallCell cornerLeftSouth = new WallCell(x, y);
            cornerLeftSouth.texture.SetAllWalls("src/assets/textures/wood_wall.png");
            cornerLeftSouth.texture.southOut = "src/assets/textures/wood_wall_slope_right.png";
            cornerLeftSouth.texture.southIn = "src/assets/textures/wood_wall_slope_left.png";
            cornerLeftSouth.texture.northIn = "src/assets/textures/wood_wall_slope_right.png";
            cornerLeftSouth.texture.northOut = "src/assets/textures/wood_wall_slope_left.png";


            WallCell cornerRightSouth = new WallCell(x, y);
            cornerRightSouth.texture.SetAllWalls("src/assets/textures/wood_wall.png");
            cornerRightSouth.texture.southOut = "src/assets/textures/wood_wall_slope_left.png";
            cornerRightSouth.texture.southIn = "src/assets/textures/wood_wall_slope_right.png";
            cornerRightSouth.texture.northIn = "src/assets/textures/wood_wall_slope_left.png";
            cornerRightSouth.texture.northOut = "src/assets/textures/wood_wall_slope_right.png";

            WallCell wallEast = new WallCell(x, y);
            wallEast.texture.SetAllWalls("src/assets/textures/rock_wall.png");
            wallEast.texture.westIn = "src/assets/textures/rock_wall_tall.png";

            WallCell wallWest = new WallCell(x, y);
            wallWest.texture.SetAllWalls("src/assets/textures/rock_wall.png");
            wallWest.texture.eastIn = "src/assets/textures/rock_wall_tall.png";

            WallCell wallNorth = new WallCell(x, y);
            wallNorth.texture.SetAllWalls("src/assets/textures/rock_wall_tall.png");
            wallNorth.texture.eastOut = "src/assets/textures/stone_wood_wall_tall.png";
            wallNorth.texture.westOut = "src/assets/textures/stone_wood_wall_tall.png";

            WallCell wallSouth = new WallCell(x, y);
            wallSouth.texture.SetAllWalls("src/assets/textures/rock_wall_tall.png");
            wallSouth.texture.eastOut = "src/assets/textures/stone_wood_wall_tall.png";
            wallSouth.texture.westOut = "src/assets/textures/stone_wood_wall_tall.png";

            EmptyCell door = new EmptyCell(x, y);
            door.texture.SetAllWalls("src/assets/textures/rock_tall_door.png");
            door.texture.bottom = "src/assets/textures/dirt.png";
            door.texture.top = "src/assets/textures/rock_wall.png";

            EmptyCell inside = new EmptyCell(x, y);
            inside.texture.bottom = "src/assets/textures/dirt.png";
            inside.texture.top = "src/assets/textures/wood_wall.png";

            for(int w = 0; w < houseWidth; w++) {
                for(int h = 0; h < houseHeight; h++) {
                    if(w == 0 && h != 0 && h != houseHeight-1) {
                        wallWest.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)wallWest.Clone();
                    } else if (w == houseWidth-1 && h != 0 && h != houseHeight-1) {
                        wallEast.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)wallEast.Clone();
                    } else if (h == 0 && w != 0 && w != houseWidth-1 && w == (int)houseWidth/2) {
                        door.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (EmptyCell)door.Clone();
                    } else if (h == 0 && w != 0 && w != houseWidth-1) {
                        wallNorth.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)wallNorth.Clone();
                    } else if (h == houseHeight-1 && w != 0 && w != houseWidth-1) {
                        wallSouth.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)wallSouth.Clone();
                    } else if (w == 0 && h == 0) {
                        cornerLeftNorth.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)cornerLeftNorth.Clone();
                    } else if (w == 0 && h == houseHeight-1) {
                        cornerLeftSouth.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)cornerLeftSouth.Clone();
                    } else if (w == houseWidth-1 && h == 0) {
                        cornerRightNorth.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)cornerRightNorth.Clone();
                    } else if (w == houseWidth-1 && h == houseHeight-1) {
                        cornerRightSouth.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (WallCell)cornerRightSouth.Clone();
                    } else {
                        inside.SetPosition(x+w, y+h);
                        Game.room.room[(x+w, y+h)] = (EmptyCell)inside.Clone();
                    }
                }
            }
        }
    }
}