#version 410 core

out vec4 outputColor;
  
in vec4 vertexColor;

void main()
{
    outputColor = vertexColor;
} 