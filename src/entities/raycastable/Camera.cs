
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LibCast {
    public class Camera : Raycastable {
        public override double pitch {get; set;} = 0;
        public override int pitchRange {get; set;} = 256;
        public override EntityTexture? texture {get; set;} = new EntityTexture("camera.png");

        public Camera(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override bool Loop() {
            PointAt(Game.room.player);
            Raycaster.Go(this, Game.room, 0, 0, 128, 72, -100);
            return true;
        }


    }
}