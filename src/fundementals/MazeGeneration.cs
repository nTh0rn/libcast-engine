namespace LibCast {


    class MazeCell {
        public List<int> dirs = new List<int> { };
        public int x;
        public int y;

        public MazeCell(int x, int y) {
            this.x = x;
            this.y = y;
        }

        
    }

    public class MazeGeneration {

        private static List<List<RoomCell>> cells;

        public static List<List<char>> GenerateMaze(int width, int height) {
            cells = new List<List<RoomCell>>();
            Stack<MazeCell> stack = new Stack<MazeCell>();
            Random r = new Random();
            for (int i = 0; i < height; i++) {
                cells.Add(new List<RoomCell>());
                for (int j = 0; j < width; j++) {
                    cells[i].Add(new RoomCell(j, i, '#'));
                }
            }
            MazeCell firstCell = new MazeCell(1, 1);
            stack.Push(firstCell);
            while (stack.Count() > 0) {
                MazeCell curr = stack.Peek();
                curr = InitMazeCell(curr);
                if (curr.dirs.Count() == 0) {
                    stack.Pop();
                    continue;
                }
                int dir = curr.dirs[r.Next(0, curr.dirs.Count())];
                stack.Push(new MazeCell(curr.x + (dir % 2 == 0 ? 1-dir : 0), curr.y + (dir % 2 != 0 ? -2+dir : 0)));
            }

            List<List<char>> output = new List<List<char>>() { };
            for (int i = 0; i < cells.Count(); i++) {
                output.Add(new List<char>(){});
                for (int j = 0; j < cells[i].Count(); j++) {
                    output[i].Add(cells[i][j].character);
                }
            }
            return output;
        }

        private static MazeCell InitMazeCell(MazeCell cell) {
            cell.dirs = new List<int>{};
            int i = 1;
            foreach (int direction in new int[] { 0, 2, 3, 1 }) {
                try {
                    string t = "";
                    for (int j = -1; j <= 1; j++) {
                        t += (direction % 2 == 0 ? cells[cell.y + j][cell.x + i] : cells[cell.y + i][cell.x + j]).character.ToString();
                        t += (direction % 2 == 0 ? cells[cell.y + j][cell.x + i*2] : cells[cell.y + i*2][cell.x + j]).character.ToString();

                    }
                    //t += (direction % 2 == 0 ? cells[cell.y][cell.x + i*2] : cells[cell.y + i*2][cell.x]).character.ToString();
                    if (t == "######") {
                        cell.dirs.Add(direction);
                    }
                } catch {
                } finally {
                    i *= -1;
                }
            }
            cells[cell.y][cell.x].character = ' ';
            return cell;
        }
    }
}