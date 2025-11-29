namespace LibCast {
    public class BrickCell : WallCell {
        public override string? texture { get; set; } = "src/assets/textures/wall.png";
        public BrickCell(double x, double y, char character, Raylib_cs.Color color) : base(x, y, character, color) { }
    }
}