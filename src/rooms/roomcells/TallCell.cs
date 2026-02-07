namespace LibCast {
    public class TallCell : RoomCell {
        public TallCell(double x, double y) : base(x, y) {
            texture.SetAllWalls("src/assets/textures/tree_tall.png");
        }
    }
}