#version 410 core

out vec4 outputColor;
  
in vec2 texCoord;

uniform sampler2D _texture;

void main()
{
    outputColor = texture(_texture, texCoord);
} 