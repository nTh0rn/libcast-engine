namespace LibCast {
    public class BrickCell : WallCell {
        public BrickCell(double x, double y) : base(x, y) {
            texture.SetOutTextures("tree_tall.png");
        }
    }

    public class DarkBrickWall : WallCell {
        public DarkBrickWall(double x, double y) : base(x, y) {
            texture.SetOutTextures("dark_cobblestone.png");
        }
    }

    public class DoorCell : RoomCell {
        public DoorCell(double x, double y) : base(x, y) {
            texture = new TextureMap("half_transparent.png",
            "direction_test/bottom.png",
            "direction_test/wo.png",
            "direction_test/eo.png",
            "direction_test/no.png",
            "direction_test/so.png",
            "direction_test/wi.png",
            "direction_test/ei.png",
            "direction_test/ni.png",
            "half_transparent.png");
        }
    }
}