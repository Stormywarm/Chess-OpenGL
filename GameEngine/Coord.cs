using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class Coord
    {
        public int file;
        public int rank;

        public Coord(int x, int y)
        {
            file = x;
            rank = y;
        }

        public Coord(Vector2 v)
        {
            file = (int)v.X;
            rank = (int)v.Y;
        }

        public bool IsValidSquare()
        {
            return file >= 0 && file < Board.WIDTH && rank >= 0 && rank < Board.HEIGHT;
        }

        public int ToIndex()
        {
            return file + rank * Board.WIDTH;
        }

        public ulong ToBitBoard()
        {
            int index = file + rank * Board.WIDTH;
            return (ulong)1 << index;
        }

        public static implicit operator string(Coord c)
        {
            char file;
            try
            {
                file = (char)(c.file + 97);
            }
            catch
            {
                file = ' ';
            }
            return $"{file}{c.rank + 1}";
        }

        public static Coord operator + (Coord a, Coord b) => new Coord(a.file + b.file, a.rank + b.rank);
        public static Coord operator - (Coord a, Coord b) => new Coord(a.file - b.file, a.rank - b.rank);
    }
}
