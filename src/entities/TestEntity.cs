using Raylib_cs;

namespace LibCast {
    public class TestEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override EntityTexture? texture {get; set;} = new EntityTexture("src/assets/textures/wall_brickhigh.png");



        public TestEntity(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            return true;
        }
    }
}