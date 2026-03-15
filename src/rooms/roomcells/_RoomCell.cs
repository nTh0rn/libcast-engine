namespace LibCast {
    public abstract class RoomCell {
        public double x;
        public double y;
        public TextureMap texture;
        public bool stopRay = false;

        public RoomCell(double x, double y) {
            this.x = x;
            this.y = y;
            texture = new TextureMap();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public void SetPosition(int x, int y) {
            this.x = (double)x;
            this.y = (double)y;
        }


    }

    public class WallCell : SolidCell {
        public WallCell(double x, double y) : base(x, y) {
        }
    }


    public class SolidCell : RoomCell {
        public SolidCell(double x, double y) : base(x, y) {}
    }

    public class EmptyCell : RoomCell {
        public EmptyCell(double x, double y) : base(x, y) {
            texture.bottom = "src/assets/textures/grass.png";
        }
    }
}