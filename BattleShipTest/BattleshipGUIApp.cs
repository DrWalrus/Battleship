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
    public struct UIElement
    {
        Rectangle Bounds { get; set; }
        bool Visible { get; set; }

        Color Color { get; set; }

        Color AltColor { get; set; }

        bool Border { get; set; }
        
    }
    internal class BattleshipGUIApp
    {
        public static void Main()
        {
            
            BattleshipGUIApp guiApp = new BattleshipGUIApp();
            guiApp.Run();
        }

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

        string winnerText = "";

        IEnumerable<Type> botTypes;

        Queue<Turn> turns = new Queue<Turn>();

        Color[,] p1Grid = new Color[BattleShipGame.GRID_SIZE, BattleShipGame.GRID_SIZE];
        Color[,] p2Grid = new Color[BattleShipGame.GRID_SIZE, BattleShipGame.GRID_SIZE];

        private void Run()
        {
            botTypes = GetListOfBotClasses();
            string[] bots = (from b in botTypes
                             select b.Name).ToArray();

            game = new BattleShipGame();
            Raylib.InitWindow(800, 480, "Battleship");
            Raylib.SetTargetFPS(60);
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(altColor);

                bool isRunPressed = ComponentLib.DrawButton("Run Match", rectRunMatch, true);

                if(isRunPressed && p1Index >= 0 && p2Index >= 0)
                {
                    winnerText = "";

                    IBattleshipAI p1 = InstantiateAI(p1Index);
                    IBattleshipAI p2 = InstantiateAI(p2Index);

                    MatchResult result = game.RunMatch(p1, p2);
                }

                if(turns.Count > 0)
                {
                    Turn turn = turns.Dequeue();


                }

                ComponentLib.DrawGrid(p1Grid, p2GridLocation);

                ComponentLib.DrawGrid(p2Grid, p2GridLocation);

                ComponentLib.DrawDropdown(bots, rectSelectP1, ref p1Index, ref isP1ListOpen);

                ComponentLib.DrawDropdown(bots, rectSelectP2, ref p2Index, ref isP2ListOpen);

                

                Raylib.EndDrawing();

                Raylib.DrawText(winnerText, 20, 430, 20, mainColor);


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


        

        private void UpdateGrid(ref Color[,] grid)
        {

        }

        
    }
}
