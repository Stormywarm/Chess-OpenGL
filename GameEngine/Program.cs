namespace GameEngine
{
    internal class Program
    {
        const int width = 800;
        const int height = 800;

        static void Main(string[] args)
        {
            using (Game game = new Game(width, height, "Game Engine"))
            {
                game.Run();
            }
        }
    }
}