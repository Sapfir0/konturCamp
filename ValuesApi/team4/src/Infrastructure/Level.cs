using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using thegame.Infrastructure.Common;
using thegame.Models;

namespace thegame.Infrastructure
{
    public class Level
    {
        public readonly string File;
        public readonly CellDto[] Map;

        public readonly int Width;
        public readonly int Height;

        public Level(CellDto[] map, int width, int height, string file = null)
        {
            Map = map;
            Width = width;
            Height = height;
            File = file;
        }

        public Level Next()
        {
            var newLevel = All().SkipWhile(lvl => lvl != File).FirstOrDefault(lvl => lvl != File);
            return newLevel == null ? null : FromFile(newLevel);
        }

        public bool IsFinished() => Map.All(cell => cell.Type != "box");

        public static string[] All() => Assembly.GetExecutingAssembly().GetManifestResourceNames();

        public static Level First() => FromFile(All().First());
        
        public static Level Load(int level) => FromFile(All().First(name => name.Contains(level.ToString())));

        public CellDto GetCell(Vec vector, params string[] type) => 
            Map.FirstOrDefault(x => x.Pos.Equals(vector) && (type == null || type.Contains(x.Type)));

        public static Level FromFile(string name) =>
            FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(name), name);

        static Level FromStream(Stream stream, string file) => FromSource(stream.ReadAllLines().ToArray(), file);

        static Level FromSource(string[] lines, string file)
        {
            List<CellDto> map = new List<CellDto>();
            int index = 0, width = 0, x = 0, y = 0;

            foreach (var line in lines)
            {
                foreach (var ch in line)
                {
                    switch (ch)
                    {
                        case 'w':
                            map.Add(new CellDto(index.ToString(), new Vec(x++, y), "wall", "", 1));
                            break;

                        case '*':
                            map.Add(new CellDto(index.ToString(), new Vec(x++, y), "target", "", 0));
                            break;

                        case 'P':
                            map.Add(new CellDto(index.ToString(), new Vec(x++, y), "player", "", 1));
                            break;

                        case 'B':
                            map.Add(new CellDto(index.ToString(), new Vec(x++, y), "box", "", 1));
                            break;

                        case '.':
                            map.Add(new CellDto(index.ToString(), new Vec(x++, y), "", "", 1));
                            break;
                    }

                    index++;
                }

                y++;
                x = 0;
                width = Math.Max(width, line.Length);
            }

            return new Level(map.ToArray(), width, lines.Length, file);
        }

        public Vec GetPlayerPosition() => 
            Map.First(x => x.Type == "player").Pos.Clone();
    }
}