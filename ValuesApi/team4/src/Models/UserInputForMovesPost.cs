using thegame.Infrastructure.Common;

namespace thegame.Models
{
    public class UserInputForMovesPost
    {
        public UserInputForMovesPost(char keyPressed, Vec clickedPos)
        {
            KeyPressed = keyPressed;
            ClickedPos = clickedPos;
        }

        public readonly char KeyPressed;
        public readonly Vec ClickedPos;

        public Direction GetDirection()
        {
            switch ((int) KeyPressed)
            {
                case 37: // ArrowLeft
                case 65: // A
                    return Direction.Left;

                case 39: // ArrowRight
                case 68: // D
                    return Direction.Right;

                case 38: // ArrowUp
                case 87: // W
                    return Direction.Up;

                case 40: // ArrowDown
                case 83: // S
                    return Direction.Down;

                default:
                    return Direction.None;
            }
        }
    }
}