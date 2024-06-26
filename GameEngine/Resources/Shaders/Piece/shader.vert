#version 410 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 projection;

void main()
{
    texCoord = aTexCoord;

    gl_Position = vec4(vPos, 1.0) * projection; 
}