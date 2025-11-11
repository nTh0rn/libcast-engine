using System.Dynamic;

namespace LibCast {
    public abstract class Entity : Global {
        public virtual bool running { get; set; }
        public abstract bool Loop();
        public double x;
        public double y;
        public double z;
        public virtual double direction { get; set; }
        public virtual double radius { get; set;  }

        public bool CollisionPoint(double pointX, double pointY) {
            return Math.Sqrt(Math.Pow(x-pointX, 2)+Math.Pow(y-pointY, 2)) <= radius;
        }
        public bool CollisionEntity(Entity entity) {
            return Math.Sqrt(Math.Pow(x-entity.x, 2)+Math.Pow(y-entity.y, 2)) <= radius+entity.radius;
        }
    }
}