#version 450 core
layout (location = 1) in vec3 vPos;
layout (location = 2) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 projection;

void main()
{
    texCoord = aTexCoord;

    gl_Position = vec4(vPos, 1.0) * projection; 
}