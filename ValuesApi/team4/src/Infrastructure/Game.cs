using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using thegame.Infrastructure.Common;
using thegame.Models;

namespace thegame.Infrastructure
{
    public class Game
    {
        public readonly Guid Id = Guid.NewGuid();
        Level level;
        bool isFinished;
        int score;

        public Game() => level = Level.First();

        public Game(Guid guid) : this() => Id = guid;
        
        public Game(int lvl) => level = Level.Load(lvl);

        public void OnLevelLoaded() => isFinished = false;

        public bool CheckPosition(string type, Vec position) => 
            level.Map.Any(cell => cell.Type == type && cell.Pos.Equals(position));

        public void MovePlayer(Direction direction)
        {
            var playerPos = level.GetPlayerPosition();

            var validMove = CheckValidMove(playerPos, direction);
            if (!validMove) return;

            var newPosPlayer = playerPos.GetNextPosition(direction);
            if (CheckPosition("box", newPosPlayer) || CheckPosition("boxOnTarget", newPosPlayer))
            {
                if (!CheckValidMove(newPosPlayer, direction)) return;
                
                var newPosOfBox = newPosPlayer.GetNextPosition(direction);

                if(CheckPosition("box", newPosOfBox) || CheckPosition("boxOnTarget", newPosOfBox)) return;

                Move(newPosPlayer, newPosOfBox, "box", "boxOnTarget");
            }

            Move(playerPos, newPosPlayer, "player");
        }

        public bool CheckValidMove(Vec vector, Direction direction)
        {
            Vec vec = vector.Clone();
            switch (direction)
            {
                case Direction.Up:
                    vec.Y -= 1;
                    return vec.Y >= 0 && !CheckPosition("wall", vec);
                
                case Direction.Left:
                    vec.X -= 1;
                    return (vec.X - 1) >= 0 && !CheckPosition("wall", vec);
                
                case Direction.Right:
                    vec.X += 1;
                    return (vec.X + 1) < level.Width && !CheckPosition("wall", vec);
                
                case Direction.Down:
                    vec.Y += 1;
                    return (vec.Y + 1) < level.Height && !CheckPosition("wall", vec);
                
                default:
                    return false;
            }
        }

        public void Move(Vec pastVec, Vec newVec, params string[] type)
        {
            var currentCell = level.GetCell(pastVec, type);
            var futureCell = level.GetCell(newVec, null);
            if (futureCell != null && futureCell.Type == "target" && currentCell.Type == "box")
            {
                currentCell.Type = "boxOnTarget";
                score++;
            }
            else if (currentCell.Type == "boxOnTarget")
            {
                currentCell.Type = "box";
                score--;
            }
            
            currentCell.Pos = newVec.Clone();
        }

        public void CheckForLevelIsFinished()
        {
            if (level.IsFinished())
            {
                isFinished = true;
                
                var nextLevel = level.Next();
                if (nextLevel != null) level = nextLevel;
            }
        }

        public ObjectResult ToResponse() =>
            new ObjectResult(
                new GameDto(level.Map, true, false, level.Width, level.Height, Id, isFinished, score)
            );
    }
}