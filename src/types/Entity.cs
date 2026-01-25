
namespace LibCast {
    public abstract class Entity {
        public virtual bool running { get; set; }
        public abstract bool Loop();
        public double x;
        public double y;
        public double z;
        public virtual double direction { get; set; }
        public virtual double radius { get; set;  }
        public virtual string? texture {get; set;}
        public virtual double traversalScale {get; set;} = 0.1;
        
        public bool CollisionPoint(double pointX, double pointY) {
            return Math.Sqrt(Math.Pow(x-pointX, 2)+Math.Pow(y-pointY, 2)) <= radius;
        }
        public bool CollisionEntity(Entity entity) {
            return Math.Sqrt(Math.Pow(x-entity.x, 2)+Math.Pow(y-entity.y, 2)) <= radius+entity.radius;
        }

        public bool IsValidMove(int dirx, int diry) {
            int ix = (int)(x+radius*dirx), iy = (int)(y+radius*diry);
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
            int dirx = dx > 0 ? 1 : -1;
            int diry = dy > 0 ? 1 : -1;
            double oldX = x, oldY = y;

            if(KeyDown(KeyboardKey.LeftShift)) {
                dx *= 1.5;
                dy *= 1.5;
            }
            
            double[,] attempts = new double[,]{{dx, dy}, {oldX + dx, oldY}, {oldX, oldY + dy}};
            for(int i = 0; i < attempts.GetLength(0); i++) {
                x += attempts[i,0];
                y += attempts[i,1];
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