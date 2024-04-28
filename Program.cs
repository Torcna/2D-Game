
namespace imba_of_patch
{
    class Program
    {
        static void Main(string[] argv)
        {
            using(Game game = new Game(1000,583))
            {
                game.Run();
            }
        }

    }
}

