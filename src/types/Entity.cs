
namespace LibCast {
    public struct EntityTexture {
        public string front;
        public string left;
        public string right;
        public string back;

        public EntityTexture(string f, string l, string r, string b) {
            front = f;
            left = l;
            right = r;
            back = b;
        }

        public EntityTexture(string f) {
            front = f;
            left = f;
            right = f;
            back = f;
        }
        
    }
    public abstract class Entity {
        public virtual bool running { get; set; }
        public abstract bool Loop();
        public double x;
        public double y;
        public double z;
        public virtual double direction { get; set; }
        public virtual double radius { get; set;  }
        public virtual EntityTexture? texture {get; set;}
        public virtual double traversalScale {get; set;} = 0.1;
        public int priority = 0;
        
        public bool CollisionPoint(double pointX, double pointY) {
            return Math.Sqrt(Math.Pow(x-pointX, 2)+Math.Pow(y-pointY, 2)) <= radius;
        }
        public bool CollisionEntity(Entity entity) {
            return Math.Sqrt(Math.Pow(x-entity.x, 2)+Math.Pow(y-entity.y, 2)) <= radius+entity.radius;
        }

        public void PointAt(Entity entity) {

            double dx = entity.x - this.x;
            double dy = entity.y - this.y;

            // atan2 handles all quadrants and negative coordinates correctly
            double angleRad = Math.Atan2(-dy, dx);

            double angleDeg = angleRad * (180.0 / Math.PI);

            // Normalize to [0, 360)
            if (angleDeg < 0)
                angleDeg += 360.0;

            direction = angleDeg;
        }

        public bool IsValidMove(int dirx, int diry) {
            int ix = FloorWorldCoord(x+radius*dirx), iy = FloorWorldCoord(y+radius*diry);
            if(!Game.room.room.ContainsKey((ix, iy))) return false;
            if(Game.room.room[(ix, iy)] is SolidCell) return false;
            
            foreach(Entity entity in Game.room.entitiesInRange) {
                if(entity != this && entity.CollisionEntity(this)) return false;
            }
            return true;
        }

        public void moveDirection(double dir) {
            double dx = Math.Cos((dir) * (Math.PI / 180.0)) * traversalScale * Screen.deltaTime;
            double dy = -Math.Sin((dir) * (Math.PI / 180.0)) * traversalScale * Screen.deltaTime;
            double oldX = x, oldY = y;

            if(KeyDown(KeyboardKey.LeftShift)) {
                dx *= 1.5;
                dy *= 1.5;
            }

            // Try diagonal, then horizontal, then vertical
            double[,] attempts = new double[,]{{dx, dy}, {dx, 0}, {0, dy}};
            for(int i = 0; i < attempts.GetLength(0); i++) {
                double tryX = oldX + attempts[i,0];
                double tryY = oldY + attempts[i,1];
                int dirx = attempts[i,0] > 0 ? 1 : -1;
                int diry = attempts[i,1] > 0 ? 1 : -1;
                x = tryX;
                y = tryY;
                if(IsValidMove(dirx, diry)) {
                    (int chunkX, int chunkY) newChunk = Game.room.CoordinateToChunk(x, y);
                    (int chunkX, int chunkY) oldChunk = Game.room.CoordinateToChunk(oldX, oldY);
                    if(newChunk != oldChunk) {
                        Game.room.AddEntity(this);
                        Game.room.entities[oldChunk].Remove(this);
                    }
                    return;
                }
            }
            // Revert to original
            x = oldX; y = oldY;
        }
    }
}