using BattleShipCore;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipPublicSDK
{
    
    internal class BattleshipGUIApp
    {
        public static void Main()
        {
            BattleshipGUIApp guiApp = new BattleshipGUIApp();
            guiApp.Run();
        }

        private static readonly long UpdateSpeed = 50;

        private long lastUpdate = 0;

        BattleShipGame game;

        bool isP1ListOpen = false;

        bool isP2ListOpen = false;

        int p1Index = -1;

        int p2Index = -1;

        Color mainColor = Color.RAYWHITE;

        Color altColor = Color.BLACK;

        Rectangle rectRunMatch = new Rectangle(300, 50, 135, 25);

        Rectangle rectSelectP1 = new Rectangle(50, 50, 225, 25);

        Rectangle rectSelectP2 = new Rectangle(500, 50, 225, 25);

        Vector2 p1GridLocation = new Vector2(50, 75);

        Vector2 p2GridLocation = new Vector2(500, 75);

        string statusText = "";

        IEnumerable<Type> botTypes;

        MatchResult result;

        Queue<Turn> turns = new Queue<Turn>();

        Color[,] p1Grid = new Color[BattleShipGame.GRID_SIZE, BattleShipGame.GRID_SIZE];
        Color[,] p2Grid = new Color[BattleShipGame.GRID_SIZE, BattleShipGame.GRID_SIZE];

        private void Run()
        {
            botTypes = GetListOfBotClasses();
            string[] bots = (from b in botTypes
                             select b.Name).ToArray();

            game = new BattleShipGame();
            lastUpdate = GetCurrentMilliSeconds();
            Raylib.InitWindow(800, 480, "Battleship");
            Raylib.SetTargetFPS(60);
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(altColor);

                bool isRunPressed = ComponentLib.DrawButton("Run Match", rectRunMatch, true, turns.Count <= 0);

                if(isRunPressed && p1Index >= 0 && p2Index >= 0)
                {
                    statusText = "Running match...";

                    IBattleshipAI p1 = InstantiateAI(p1Index);
                    IBattleshipAI p2 = InstantiateAI(p2Index);

                    result = game.RunMatch(p1, p2);
                    p1Grid = InitialiseGrid(result.Player1Ships);
                    p2Grid = InitialiseGrid(result.Player2Ships);

                    turns = result.Turns;
                }


                if (turns.Count > 0 && (GetCurrentMilliSeconds() - lastUpdate >= UpdateSpeed))
                {
                    lastUpdate = GetCurrentMilliSeconds();
                    Turn turn = turns.Dequeue();
                    if (turn.PlayerIndex == 0)
                    {
                        UpdateGrid(ref p2Grid, turn.ShotResult);
                    }
                    else
                    {
                        UpdateGrid(ref p1Grid, turn.ShotResult);
                    }

                    if(turns.Count == 0)
                    {
                        statusText = "Match complete! Winner: " + result.Winner.ToString();
                    }
                }
                              

                ComponentLib.DrawGrid(p1Grid, p1GridLocation);

                ComponentLib.DrawGrid(p2Grid, p2GridLocation);

                ComponentLib.DrawDropdown(bots, rectSelectP1, ref p1Index, ref isP1ListOpen, turns.Count <= 0);

                ComponentLib.DrawDropdown(bots, rectSelectP2, ref p2Index, ref isP2ListOpen, turns.Count <= 0);

                Raylib.EndDrawing();

                Raylib.DrawText(statusText, 20, 430, 20, mainColor);


            }

            Raylib.CloseWindow();
        }

        private IBattleshipAI InstantiateAI(int selectedAI)
        {
            object ai = Activator.CreateInstance(botTypes.ElementAt(selectedAI));

            if (ai is IBattleshipAI)
                return ai as IBattleshipAI;

            return null;
        }

        private Color[,] InitialiseGrid(Dictionary<Coordinate, ShipInfo> shipLocations)
        {
            Color[,] grid = new Color[BattleShipGame.GRID_SIZE, BattleShipGame.GRID_SIZE];
            for (int i = 0; i < BattleShipGame.GRID_SIZE; i++)
            {
                for (int j = 0; j < BattleShipGame.GRID_SIZE; j++)
                {
                    Coordinate coordinate = new Coordinate(i, j);
                    if (shipLocations.ContainsKey(coordinate))
                    {
                        grid[i, j] = Color.GRAY;
                    }
                    else
                    {
                        grid[i, j] = Color.BLUE;
                    }
                }
            }

            return grid;
        }

        private IEnumerable<Type> GetListOfBotClasses()
        {
            var botTypes = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                           where t.GetInterfaces().Contains(typeof(IBattleshipAI)) &&
                           t.Namespace == "BattleShipPublicSDK.Bots"
                           select t;

            return botTypes;
        }

        private void UpdateGrid(ref Color[,] grid, ShotResult shotResult)
        {
            if (shotResult.ShipInfo.IsHit)
            {
                grid[shotResult.Coordinate.X, shotResult.Coordinate.Y] = Color.RED;
            }
            else
            {
                grid[shotResult.Coordinate.X, shotResult.Coordinate.Y] = Color.DARKBLUE;
            }

        }

        private long GetCurrentMilliSeconds()
        {
            return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
        }


    }
}
