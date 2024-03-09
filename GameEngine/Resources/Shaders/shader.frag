#version 450 core

out vec4 outputColor;
  
in vec4 vertexColor;

void main()
{
    outputColor = vertexColor;
} 