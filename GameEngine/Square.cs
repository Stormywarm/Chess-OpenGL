using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class Square : RenderObject
    {
        public Square(Shader shader, Vector3 position)
        {
            this.shader = shader;

            this.position = position;
            UpdatePosition();
        }
    }
}
