#version 450 core
out vec4 OutputColour;

in vec2 TexCoords;

uniform sampler2D screenTexture;

void main()
{
    vec4 colour = texture(screenTexture, TexCoords);

    OutputColour = colour;
} 