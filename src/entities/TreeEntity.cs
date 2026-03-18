using Raylib_cs;

namespace LibCast {
    public class TreeEntity : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override EntityTexture? texture {get; set;} = new EntityTexture("tree_tall.png");


        public TreeEntity(double x, double y) {
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