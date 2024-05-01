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
    public class Move
    {
        public Coord StartCoord { get; }
        public Coord DestCoord { get; }

        public Move(Coord startCoord, Coord destCoord)
        {
            StartCoord = startCoord;
            DestCoord = destCoord;
        }

        public static implicit operator string(Move move)
        {
            return $"{(string)move.StartCoord}-{(string)move.DestCoord}";
        }
    }
}
