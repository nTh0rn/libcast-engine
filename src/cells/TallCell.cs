namespace LibCast {
    public class TallCell : WallCell {
        public override string? texture { get; set; } = "src/assets/textures/shai.png";
        public TallCell(double x, double y, char character, Color color) : base(x, y, character, color) {
            floorTexture = "src/assets/textures/minecraft.png";
            ceilingTexture = "src/assets/textures/wall_brick.png";
        }
    }
}