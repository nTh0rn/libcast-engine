using Raylib_cs;

namespace LibCast {
    public class TreeEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override string? texture {get; set; } = "src/assets/textures/tree.png";
        public override bool isTall {get; set;}= true;

        public TreeEntity(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            return true;
        }
    }
}