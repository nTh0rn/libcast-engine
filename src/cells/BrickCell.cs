namespace LibCast {
    public class BrickCell : WallCell {
        public BrickCell(double x, double y, char character) : base(x, y, character) {
            texture.SetOutTextures("src/assets/textures/tree_tall.png");
        }
    }

    public class DarkBrickWall : WallCell {
        public DarkBrickWall(double x, double y, char character) : base(x, y, character) {
            texture.SetOutTextures("src/assets/textures/tree_tall.png");
            //floorTexture = "src/assets/textures/sky.png";
            //ceilingTexture = "src/assets/textures/dark_cobblestone.png";
        }
    }

    public class DoorCell : RoomCell {
        public DoorCell(double x, double y, char character) : base(x, y, character) {
            //texture.SetOutTextures("src/assets/textures/door.png");
            //texture.SetBottomTexture("src/assets/textures/dark_cobblestone.png");

            texture = new TextureMap("src/assets/textures/half_transparent.png",
            "src/assets/textures/direction_test/bottom.png",
            "src/assets/textures/direction_test/wo.png",
            "src/assets/textures/direction_test/eo.png",
            "src/assets/textures/direction_test/no.png",
            "src/assets/textures/direction_test/so.png",
            "src/assets/textures/direction_test/wi.png",
            "src/assets/textures/direction_test/ei.png",
            "src/assets/textures/direction_test/ni.png",
            "src/assets/textures/half_transparent.png");
            // texture.SetAllWalls("src/assets/textures/transparent.png");
            // texture.SetBottomTexture("src/assets/textures/dark_brick_wall.png");
            //floorTexture = "src/assets/textures/sky.png";
            //ceilingTexture = "src/assets/textures/dark_cobblestone.png";
        }
    }
}