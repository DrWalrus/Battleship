using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BattleShipCore;
using Raylib_cs;

namespace BattleShipPublicSDK
{
    struct CommonState
    {
        public Color MainColor { get; set; }
        public Color AltColor { get; set; }
       public  bool MouseOver { get; set; }
    }

    internal class ComponentLib
    {
        public static Color MAINCOLOR = Color.WHITE;
        public static Color ALTCOLOR = Color.BLACK;
        public static Color DISABLEDCOLOR = Color.DARKGRAY;

        public static bool DrawButton(string text, Rectangle rectangle, bool outline)
        {
            var state = GetCommonState(rectangle, true);

            Raylib.DrawRectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height, state.AltColor);

            if (outline)
                Raylib.DrawRectangleLines((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height, state.MainColor);

            Raylib.DrawText(text, (int)rectangle.x + 15, (int)rectangle.y + 5, 20, state.MainColor);

            return state.MouseOver && Raylib.IsMouseButtonPressed((MouseButton)MouseButton.MOUSE_BUTTON_LEFT);
        }

        static public void DrawGrid(Color[,] grid, Vector2 location)
        {
            int cellSize = 10;

            for (int i = 0; i < BattleShipGame.GRID_SIZE; i++)
            {
                for (int j = 0; j < BattleShipGame.GRID_SIZE; j++)
                {
                    Raylib.DrawRectangle((int)location.X + (i + 1) * cellSize, (int)location.Y + (j + 1) * cellSize, cellSize, cellSize, grid[i, j]);
                }
            }
        }

        private static CommonState GetCommonState(Rectangle rectangle, bool enabled)
        {
            var state = new CommonState();
            state.MouseOver = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rectangle);
            if (enabled)
            {
                state.MainColor = state.MouseOver ? ALTCOLOR : MAINCOLOR;
                state.AltColor = !state.MouseOver ? ALTCOLOR : MAINCOLOR;
            }
            else
            {
                state.MainColor = DISABLEDCOLOR;
                state.AltColor = ALTCOLOR;
            }
            

            return state;
        }

        public static void DrawDropdown(string[] items, Rectangle rectangle, ref int selected, ref bool isOpen)
        {
            if (items == null)
                items = new string[0];

            var state = GetCommonState(rectangle, true);

            String text = "...";
            if (selected >= 0 && items != null && selected < items.Length)
                text = items[selected];

            bool isPressed = DrawButton(text, rectangle, true);

            if (isPressed) isOpen = !isOpen;

            Rectangle openRect = new Rectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height + (items.Length * 50));

            if (isOpen)
            {
                Raylib.DrawRectangleLines((int)openRect.x, (int)openRect.y, (int)openRect.width, (int)openRect.height + (items.Length * 50), state.MainColor);

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
