using Raylib_cs;

namespace LibCast {
    public class JennEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override string? texture {get; set; } = "src/assets/textures/jenn_scaled.png";

        public JennEntity(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            return true;
        }
    }
}