using Raylib_cs;

namespace LibCast {
    public class RockEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.20;
        public override EntityTexture? texture {get; set;} = new EntityTexture("src/assets/textures/rock.png");

        public RockEntity(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            if(Game.room.room[(FloorWorldCoord(x), FloorWorldCoord(y))] is SolidCell) {
                Game.room.entities[Game.room.CoordinateToChunk(x, y)].Remove(this);
            }
            return true;
        }
    }
}