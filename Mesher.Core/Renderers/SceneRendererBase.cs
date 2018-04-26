﻿using System;
using Mesher.GraphicsCore;
using Mesher.GraphicsCore.Camera;
using Mesher.GraphicsCore.Data.OpenGL;
using Mesher.GraphicsCore.ShaderProgram;
using Scene = Mesher.Core.Objects.Scene.Scene;

namespace Mesher.Core.Renderers
{
    public abstract class SceneRendererBase
    {
        public GlShaderProgram ShaderProgram { get; }

        public SceneRendererBase(GlDataContext dataContext, String vertexShaderSource, String fragmentShaderSource)
        {
            ShaderProgram = dataContext.CreateShaderProgram(vertexShaderSource, fragmentShaderSource);
        }

        public abstract void Render(Scene scene, Camera camera);
    }
}
