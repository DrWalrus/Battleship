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

        Color mainColor = Color.BEIGE;

        Color altColor = Color.DARKGRAY;

        Rectangle rectRunMatch = new Rectangle(300, 50, 135, 25);

        Rectangle rectP1Closed = new Rectangle(50, 50, 135, 25);

        Rectangle rectP2Closed = new Rectangle(600, 50, 135, 25);

        BoundingBox box = new BoundingBox();

        IEnumerable<Type> botTypes;

        private void Run()
        {
            botTypes = GetListOfBotClasses();
            string[] bots = (from b in botTypes
                             select b.Name).ToArray();
            Raylib.InitWindow(800, 480, "Battleship");
            while (!Raylib.WindowShouldClose())
            {
                

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DARKGRAY);

                bool isRunPressed = DrawButton("Run Match", rectRunMatch, true);
                               

                DrawDropdown(bots, rectP1Closed, ref p1Index, ref isP1ListOpen);

                DrawDropdown(bots, rectP1Closed, ref p2Index, ref isP2ListOpen);

                

                Raylib.EndDrawing();

                Raylib.DrawText("Winner", 315, 55, 20, mainColor);


            }

            Raylib.CloseWindow();
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
            Color alt =  !mouseOver ? mainColor : altColor;

            Raylib.DrawRectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height, main);
            
            if(outline)
                Raylib.DrawRectangleLines((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height, alt);
            
            Raylib.DrawText(text, 315, 55, 20, alt);

            return mouseOver && Raylib.IsMouseButtonPressed((MouseButton)MouseButton.MOUSE_BUTTON_LEFT);
        }

        private void DrawDropdown(string[] items, Rectangle rectangle, ref int selected, ref bool isOpen)
        {
            if(items == null)
                items = new string[0];

            bool mouseOver = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rectangle);
            Color main = mouseOver ? altColor : mainColor;
            Color alt = !mouseOver ? mainColor : altColor;


            String text = "...";
            if(selected >= 0 || items != null && selected < items.Length)
                text = items[selected];

            bool isPressed = DrawButton(text, rectangle, true);

            Raylib.DrawText(text, 315, 55, 20, alt);

            if(!isPressed) isOpen = !isOpen;

            Rectangle openRect = new Rectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height + (items.Length * 50));

            if (isOpen)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    float yOffset = (rectangle.y + (50 * i));
                    bool isItemPressed = DrawButton(items[i], new Rectangle(rectangle.x, yOffset, rectangle.height, rectangle.width), false);
                    if (isItemPressed)
                        selected = i;

                }

                Raylib.DrawRectangleLines((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height + (items.Length * 50), altColor);
            }
            
        }
    }
}
