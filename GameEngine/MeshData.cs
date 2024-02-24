using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class MeshData
    {
        public float[] Vertices { get; }
        public uint[] Indices   { get; }

        public MeshData(float[] vertices, uint[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
