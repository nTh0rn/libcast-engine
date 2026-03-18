using Raylib_cs;

namespace LibCast {
    public class BadBunnieEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public string name = "def";
        public override EntityTexture? texture {get; set;} = new EntityTexture("bb/bb_front.png",
        "bb/bb_left.png",
        "bb/bb_right.png",
        "bb/bb_back.png");


        public BadBunnieEntity(double x, double y, string name) {
            this.x = x;
            this.y = y;
            this.name = name;
        }

        public override bool Loop() {
            return true;
        }
    }
}