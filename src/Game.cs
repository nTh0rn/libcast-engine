namespace LibCast {
    public class Game {
        public static Room room; // Current room
        public static List<Room> rooms = new List<Room>() {new RoomProcedural(), new RoomDefault()}; // All available rooms.

        // Load a particular room
        public static void LoadRoom(string roomName) {
            foreach (Room eachRoom in rooms) {
                if (eachRoom.name.Equals(roomName)) {
                    room = eachRoom;
                }
            }
        }

        // Initialization, the first thing ran upon booting.
        public static void Init() {
            LoadRoom("rmProcedural");
            
        }


        public static void Loop() {
            Random rng = new Random();
            
            Screen.DrawBackground(0);
            room.Loop();
            Screen.DrawFPS();
            //Terminal.GetUserCommand();
            UI.Go();
            
        }
    }
}