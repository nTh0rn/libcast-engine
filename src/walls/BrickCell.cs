using Raylib_cs;

namespace LibCast {
    public class BrickCell : WallCell {
        public bool isTextured = true;
        public string? texture = "assets/textures/wall.png";
        public BrickCell(double x, double y, char character, Raylib_cs.Color color) : base(x, y, character, color) { }
    }
}