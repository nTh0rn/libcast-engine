namespace LibCast {
    public class BrickCell : WallCell {
        public override string? texture { get; set; } = "src/assets/textures/wall_brickhigh.png";
        public BrickCell(double x, double y, char character, Color color) : base(x, y, character, color) {
            floorTexture = "src/assets/textures/minecraft.png";
            ceilingTexture = "src/assets/textures/wall_brick.png";
        }
    }
}