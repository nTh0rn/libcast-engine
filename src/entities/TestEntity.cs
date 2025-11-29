
namespace LibCast {
    public class TestEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override string? texture {get; set; } = "src/assets/textures/entity.png";

        public TestEntity(double x, double y) {
            this.x = x;
            this.y = y;
        }


        public override bool Loop() {
            return true;
        }


    }
}