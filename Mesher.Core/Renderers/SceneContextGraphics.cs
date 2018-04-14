﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesher.GraphicsCore;
using Mesher.GraphicsCore.ShaderProgram;
using Mesher.Mathematics;

namespace Mesher.Core.Renderers
{
    public class SceneContextGraphics
    {
        private ISceneContext m_sceneContext;
        private ShaderProgram m_shaderProgram;

        public SceneContextGraphics(ISceneContext sceneContext)
        {
            m_sceneContext = sceneContext;
            m_shaderProgram = sceneContext.DataContext.CreateShaderProgram(Properties.Resources.ComponentsVertexShader, Properties.Resources.ComponentsFragmentShader);
        }

        public void DrawLine(Vec3 p0, Vec3 p1, Single lineWidth, Color color)
        {
            m_shaderProgram.Bind();

            InitView();
            m_shaderProgram.SetBuffer("position", new []{p0.X, p0.Y, p0.Z, p1.X, p1.Y, p1.Z}, 3);
            m_shaderProgram.SetValue("color", new Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
            m_shaderProgram.RenderLines(lineWidth, false);

            m_shaderProgram.Unbind();
        }

        public void DrawTriangle(Vec3 p0, Vec3 p1, Vec3 p2, Color color)
        {
            m_shaderProgram.Bind();

            InitView();
            m_shaderProgram.SetBuffer("position", new[] { p0.X, p0.Y, p0.Z, p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z }, 3);
            m_shaderProgram.SetValue("color", new Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
            m_shaderProgram.RenderTriangles(false);

            m_shaderProgram.Unbind();
        }

        public void DrawQuad(Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3, Color color)
        {
            DrawTriangle(p0, p1, p2, color);
            DrawTriangle(p1, p2, p3, color);
        }

        private void InitView()
        {
            m_shaderProgram.SetValue("viewPort", new Vec4(0, 0, m_sceneContext.Width, m_sceneContext.Height));
            m_shaderProgram.SetValue("clipDistance", (Single)Math.Max(m_sceneContext.Width, m_sceneContext.Height));
        }
    }
}