using Raylib_cs;

namespace LibCast {
    public class Player : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.25;

        private double traversalScale = 10;

        public Player(int x, int y) {
            this.x = x;
            this.y = y;
        }


        public override bool Loop() {
            if (KeyDown(KeyboardKey.Comma)) {
                direction += KeyDown(KeyboardKey.LeftShift) ? 5 : 3;
            }
            if (KeyDown(KeyboardKey.Period)) {
                direction -= KeyDown(KeyboardKey.LeftShift) ? 5 : 3;
            }
            if (KeyDown(KeyboardKey.A)) {
                moveDirection(direction + 90);
            }
            if (KeyDown(KeyboardKey.D)) {
                moveDirection(direction - 90);
            }
            if (KeyDown(KeyboardKey.W)) {
                moveDirection(direction);
            }
            if (KeyDown(KeyboardKey.S)) {
                moveDirection(direction - 180);
            }

            Raycaster.Go(this);
            return true;
        }
        
        private void moveDirection(double dir) {
            double dx = Math.Cos((dir) * (Math.PI / 180.0)) / traversalScale;
            double dy = -Math.Sin((dir) * (Math.PI / 180.0)) / traversalScale;
            if (Game.room.room[(int)(y)][(int)(x + dx)] is EmptyCell) {
                x += dx;
            }
            if(Game.room.room[(int)(y+dy)][(int)(x)] is EmptyCell) {
                y += dy;
            }
        }


    }
}