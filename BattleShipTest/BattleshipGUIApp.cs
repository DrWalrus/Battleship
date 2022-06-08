using BattleShipCore;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
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

                bool isRunPressed = DrawButton("Run Match", rectRunMatch, true);

                if(isRunPressed && p1Index >= 0 && p2Index >= 0)
                {
                    winnerText = "";

                    IBattleshipAI p1 = InstantiateAI(p1Index);
                    IBattleshipAI p2 = InstantiateAI(p2Index);

                    p1Grid.

                    MatchResult result = game.RunMatch(p1, p2);
                    result.
                }

                if(turns.Count > 0)
                {
                    Turn turn = turns.Dequeue();

                }

                
                DrawDropdown(bots, rectSelectP1, ref p1Index, ref isP1ListOpen);

                DrawDropdown(bots, rectSelectP2, ref p2Index, ref isP2ListOpen);

                

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

        private Color[,] InitialiseGrid()
        {
            Color[,] grid = new Color[BattleShipGame.GRID_SIZE, BattleShipGame.GRID_SIZE];
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
        }

        private IEnumerable<Type> GetListOfBotClasses()
        {
            var botTypes = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                           where t.GetInterfaces().Contains(typeof(IBattleshipAI)) &&
                           t.Namespace == "BattleShipPublicSDK.Bots"
                           select t;

            return botTypes;
        }


        private bool DrawButton(string text, Rectangle rectangle, bool outline)
        {
            bool mouseOver = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rectangle);
            Color main = mouseOver ? altColor : mainColor;
            Color alt =  !mouseOver ? altColor : mainColor;

            Raylib.DrawRectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height, alt);
            
            if(outline)
                Raylib.DrawRectangleLines((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height, main);
            
            Raylib.DrawText(text, (int)rectangle.x + 15, (int)rectangle.y + 5, 20, main);

            return mouseOver && Raylib.IsMouseButtonPressed((MouseButton)MouseButton.MOUSE_BUTTON_LEFT);
        }

        private void DrawDropdown(string[] items, Rectangle rectangle, ref int selected, ref bool isOpen)
        {
            if(items == null)
                items = new string[0];

            bool mouseOver = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rectangle);
            Color main = mouseOver ? altColor : mainColor;
            Color alt = !mouseOver ? altColor : mainColor;

            String text = "...";
            if(selected >= 0 && items != null && selected < items.Length)
                text = items[selected];

            bool isPressed = DrawButton(text, rectangle, true);

            if(isPressed) isOpen = !isOpen;

            Rectangle openRect = new Rectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height + (items.Length * 50));

            if (isOpen)
            {
                Raylib.DrawRectangleLines((int)openRect.x, (int)openRect.y, (int)openRect.width, (int)openRect.height + (items.Length * 50), mainColor);

                for (int i = 0; i < items.Length; i++)
                {
                    float yOffset = (rectangle.y + (50 * (i + 1)));
                    bool isItemPressed = DrawButton(items[i], new Rectangle(rectangle.x + 1, yOffset, rectangle.width - 2, rectangle.height), false);
                    if (isItemPressed)
                    {
                        selected = i;
                        isOpen = false;
                    }

                }

                
            }
            
        }
    }
}
