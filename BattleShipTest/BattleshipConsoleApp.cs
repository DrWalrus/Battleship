// See https://aka.ms/new-console-template for more information
using BattleShipCore;
using BattleShipPublicSDK;

namespace BattleShipPublicSDK
{
    public class BattleshipConsoleApp
    {
        public static void Main(string[] args)
        {
            BattleshipConsoleApp consoleApp = new BattleshipConsoleApp();
            consoleApp.Run();
        }

        private static readonly long UpdateSpeed = 250;

        private long LastUpdate { get; set; }
        

        private void Run()
        {
            Console.WriteLine("Starting Battleship game...");
            BattleShipGame game = new BattleShipGame();
            MatchResult result = game.RunMatch(new RandomBattleShipAI(), new RandomBattleShipAI());

            LastUpdate = GetCurrentMilliSeconds();

            while (result.Turns.Count > 0)
            {
                if (!Update())
                    continue;

                Draw(result.Turns.Dequeue());
            }

            Console.WriteLine("Winner: " + result.Winner.ToString());

        }

        private bool Update()
        {
            if (GetCurrentMilliSeconds() - LastUpdate <= UpdateSpeed)
                return false;

            LastUpdate = GetCurrentMilliSeconds();

            return true;
        }

        private void Draw(Turn turn)
        {
            Console.Clear();

            Console.WriteLine("Battleship game");
            Console.WriteLine("Turn #: " + turn.TurnNumber);
            Console.WriteLine("Player #: " + turn.PlayerIndex);
        }

        private long GetCurrentMilliSeconds()
        {
            return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
        }
    }
    
}
