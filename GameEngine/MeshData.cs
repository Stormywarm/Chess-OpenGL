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
        public float[] vertices { get; set; }
        public uint[] indices { get; set; }

        public MeshData(float[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
        }
    }
}
