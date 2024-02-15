using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class VertexData
    {
        public float[] vertices { get; set; }
        public uint[] indices { get; set; }

        public VertexData(float[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
        }
    }
}
