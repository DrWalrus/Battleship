// See https://aka.ms/new-console-template for more information
using BattleShipCore;
using BattleShipPublicSDK;

/**
 * Console app for Battleship framework.
 * Meant to be a demonstration of the framework's capabilities, not a production app
 * This app will probably be made obsolete by a proper GUI app and 
 * a proper console app capable of doing batches, tournamets, file output etc
 * 
 * This app allows the user to select 2 AI bots to play a single match of battleship
 * and then displays the result to the user, turn by turn.
 * 
 * Grid legend:
 * ~ = water
 * 0 = intact ship cell
 * X = hit ship cell
 * m = miss
 * 
 * To run: open solution in Visual Studio
 * Run/Debug
 * Bots should be placed in the Bots folder, with the namespace "BattleShipPublicSDK.Bots"
 * Bots must implement the IBattleshipAI interface.
 * 
 */
namespace BattleShipPublicSDK
{
    public class BattleshipConsoleApp
    {
        public static void Main(string[] args)
        {
            BattleshipConsoleApp consoleApp = new BattleshipConsoleApp();
            consoleApp.Run();
        }

        private static readonly long UpdateSpeed = 100;

        private long LastUpdate { get; set; }
        

        private void Run()
        {
            Console.Clear();
                       
            IEnumerable <Type> botTypes = GetListOfBotClasses();

            if(botTypes == null || botTypes.Count() <= 0)
            {
                Console.WriteLine("Not AIs found. Exiting program");
                return;
            }

            IBattleshipAI player1 = GetAIFromInput("Select AI for player 1: ", botTypes);
            IBattleshipAI player2 = GetAIFromInput("Select AI for player 2: ", botTypes);

            BattleShipGame game = new BattleShipGame();
            MatchResult result = game.RunMatch(player1, player2);

            string[,] player1Grid = CreateGrid(result.Player1Ships);
            string[,] player2Grid = CreateGrid(result.Player2Ships);

            LastUpdate = GetCurrentMilliSeconds();

            while (result.Turns.Count > 0)
            {
                if (!Update())
                    continue;

                Turn turn = result.Turns.Dequeue();
                if(turn.PlayerIndex == 0)
                {
                    player2Grid = UpdateGrid(turn.ShotResult, player2Grid);
                }
                else
                {
                    player1Grid = UpdateGrid(turn.ShotResult, player1Grid);
                }

                Draw(turn, player1Grid, player2Grid);
            }

            Console.WriteLine("Match complete! Winner: " + result.Winner.ToString());
            Console.WriteLine("Press Y to run another match or Q to quit: ");
            string input = Console.ReadLine();

            if (input != null && "Y" == input.ToUpper())
                Run();
        }

        private string[,] UpdateGrid(ShotResult shotResult, string[,] grid)
        {
            
            if(shotResult.ShipInfo.IsHit)
            {
                grid[shotResult.Coordinate.X, shotResult.Coordinate.Y] = "X";
            }
            else
            {
                grid[shotResult.Coordinate.X, shotResult.Coordinate.Y] = "m";
            }


            return grid;
        }

        private string[,] CreateGrid(Dictionary<Coordinate, ShipInfo> shipLocations)
        {
            string[,] grid = new string[BattleShipGame.GRID_SIZE,BattleShipGame.GRID_SIZE];

            for (int i = 0; i < BattleShipGame.GRID_SIZE; i++)
            {
                for (int j = 0; j < BattleShipGame.GRID_SIZE; j++)
                {
                    Coordinate coordinate = new Coordinate(i, j);
                    if (shipLocations.ContainsKey(coordinate))
                    {
                        grid[i, j] = "0";
                    }
                    else
                    {
                        grid[i, j] = "~";
                    }
                }
            }

            return grid;
        }

        private bool Update()
        {
            if (GetCurrentMilliSeconds() - LastUpdate <= UpdateSpeed)
                return false;

            LastUpdate = GetCurrentMilliSeconds();

            return true;
        }

        private void Draw(Turn turn, string[,] player1Grid, string[,] player2Grid)
        {
            Console.Clear();

            Console.WriteLine("Match in progress...");
            Console.WriteLine("Turn #: " + turn.TurnNumber);
            Console.WriteLine("Player #: " + turn.PlayerIndex);

            DrawGrids(player1Grid, player2Grid);

            Console.WriteLine("Legend:");
            Console.WriteLine("~ : water");
            Console.WriteLine("m : miss");
            Console.WriteLine("0 : intact ship");
            Console.WriteLine("X : hit ship");
        }

        private void DrawGrids(string[,] player1Grid, string[,] player2Grid)
        {
            string border = "----------";
            string spacer = "          ";

            Console.WriteLine(" Player 1 " + spacer + "   Player 2");
            Console.WriteLine(" " + border + " " + spacer + " " + border);

            for (int i = 0; i < BattleShipGame.GRID_SIZE; i++)
            {
                string player1Section = "";
                string player2Section = "";
                for (int j = 0; j < BattleShipGame.GRID_SIZE; j++)
                {
                    player1Section += player1Grid[i, j];
                    player2Section += player2Grid[i, j];
                }

                Console.WriteLine("|" + player1Section + "|" + spacer + "|" + player2Section + "|");
            }

            Console.WriteLine(" " + border + " " + spacer + " " + border);
        }

        private long GetCurrentMilliSeconds()
        {
            return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
        }

        private IEnumerable<Type> GetListOfBotClasses()
        {
            var botTypes = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                        where t.GetInterfaces().Contains(typeof(IBattleshipAI)) &&
                        t.Namespace == "BattleShipPublicSDK.Bots"
                        select t;

            return botTypes;
        }

        private IBattleshipAI GetAIFromInput(string message, IEnumerable<Type> botTypes)
        {
            Console.WriteLine(message);

            for (int i = 0; i < botTypes.Count(); i++)
            {
                Console.WriteLine((i + 1).ToString() + ". " + botTypes.ElementAt(i).Name);
            }

            string input = Console.ReadLine();

            if (input == null)
                input = "1";

            int selectedAI = int.Parse(input) - 1;

            object ai = Activator.CreateInstance(botTypes.ElementAt(selectedAI));
            if (ai is IBattleshipAI)
                return ai as IBattleshipAI;

            return null;
        }
    }
    
}
