﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class Shader : IDisposable
    {
        readonly int handle;

        private Dictionary<string, int> UniformLocations = new Dictionary<string, int>();

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource); 
            
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out _);

            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out _);

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out _);

            GL.DetachShader(handle, fragmentShader);
            GL.DetachShader(handle, vertexShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int numUniforms);
            for (int i = 0; i < numUniforms; i++)
            {
                string key = GL.GetActiveUniform(handle, i, out _, out _);
                int id = GL.GetUniformLocation(handle, key);

                UniformLocations.Add(key, id);
            }
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public void SetMatrix4(string key, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(UniformLocations[key], true, ref data);
        }

        public void SetVector4(string key, Vector4 data)
        {
            GL.UseProgram(handle);
            GL.Uniform4(UniformLocations[key], ref data);
        }

        public int GetAttribLocation(string key)
        {
            return GL.GetAttribLocation(handle, key);
        }

        public void Dispose()
        {
            GL.DeleteProgram(handle);
        }
    }
}
