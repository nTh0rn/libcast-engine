namespace LibCast {

    //A class and not a struct so it can be passed by reference vs duplicated
    public class TextureBitmap {
        public Color[,] texture;
        public int height;

        public TextureBitmap(Color[,] texture) {
            this.texture = texture;
            height = texture.GetLength(0);
        }
    }

    public struct TextureMap {
        public string? westOut, eastOut, northOut, southOut, westIn, eastIn, northIn, southIn, top, bottom = null;

        public TextureMap(string? top = null, string? bottom = null, string? westOut = null, string? eastOut = null, string? northOut = null, string? southOut = null, string? westIn = null, string? eastIn = null, string? northIn = null, string? southIn = null) {
            this.top = top;
            this.bottom = bottom;
            this.westOut = westOut;
            this.eastOut = eastOut;
            this.northOut = northOut;
            this.southOut = southOut;
            this.westIn = westIn;
            this.eastIn = eastIn;
            this.northIn = northIn;
            this.southIn = southIn;
        }

        public void SetOutTextures(string texture) {
            westOut = texture;
            eastOut = texture;
            northOut = texture;
            southOut = texture;
        }

        public void SetInTextures(string texture) {
            westIn = texture;
            eastIn = texture;
            northIn = texture;
            southIn = texture;
        }

        public void SetAllWalls(string texture) {
            SetInTextures(texture);
            SetOutTextures(texture);
        }

        public void SetBottomTexture(string texture) {
            bottom = texture;
        }

        public void SetTopTexture(string texture) {
            top = texture;
        }

    }
}