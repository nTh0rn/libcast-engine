using Raylib_cs;

namespace LibCast {
    public class TreeEntityTwo : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override EntityTexture? texture {get; set;} = new EntityTexture("src/assets/textures/tree_tall.png");


        public TreeEntityTwo(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            return true;
        }
    }
}