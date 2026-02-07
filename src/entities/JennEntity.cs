using Raylib_cs;

namespace LibCast {
    public class JennEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override EntityTexture? texture {get; set;} = new EntityTexture("src/assets/textures/shai_front.png",
        "src/assets/textures/shai_left.png",
        "src/assets/textures/shai_right.png",
        "src/assets/textures/shai_back.png");


        public JennEntity(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            return true;
        }
    }
}