#version 410 core
layout (location = 0) in vec3 vPos;

out vec4 vertexColor;

uniform vec4 colour;

uniform mat4 projection;

void main()
{
    vertexColor = colour; 

    gl_Position = vec4(vPos, 1.0) * projection; 
}