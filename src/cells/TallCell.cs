namespace LibCast {
    public class TallCell : RoomCell {
        public TallCell(double x, double y, char character) : base(x, y, character) {
            texture.SetOutTextures("src/assets/textures/tallme.png");
            texture.SetInTextures("src/assets/textures/tallme.png");

            //floorTexture = "src/assets/textures/minecraft.png";
            //ceilingTexture = "src/assets/textures/wall_brick.png";
        }
    }
}