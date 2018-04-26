﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Mesher.Core.Objects.Mesh;
using Mesher.Core.Objects.Scene;
using Mesher.GraphicsCore;
using Mesher.GraphicsCore.Data;
using Mesher.GraphicsCore.Data.OpenGL;
using Mesher.GraphicsCore.Light;
using Mesher.GraphicsCore.Material;
using Mesher.GraphicsCore.Primitives;
using Mesher.GraphicsCore.Texture;
using Mesher.Mathematics;

namespace Mesher.Core.Data
{
    public static class DataLoader
    {
        public static Scene LoadScene(String filePath, GlDataContext dataContext)
        {
            var path = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);

            var importer = new Assimp.AssimpImporter();

            var aiScene = importer.ImportFile(filePath, Assimp.PostProcessPreset.TargetRealTimeMaximumQuality);

            var scene = new Scene();
            
            if (aiScene.HasLights)
                for (var i = 0; i < aiScene.LightCount; i++)
                    scene.Lights.Add(InitLight(aiScene.Lights[i]));
                
            if (aiScene.HasMeshes)
                for (var i = 0; i < aiScene.MeshCount; i++)
                {
                    var aiMesh = aiScene.Meshes[i];
                    var mesh = new Mesh();
                    scene.Meshes.Add(mesh);

                    if (aiMesh.HasVertices)
                    {
                        var verts = aiMesh.Vertices.Select(t => GetVec3(t)).ToArray();
                        mesh.Vertexes = dataContext.CreateVertexBuffer(verts);
                    }

                    if (aiMesh.HasNormals)
                    {
                        var normals = aiMesh.Normals.Select(t => GetVec3(t)).ToArray();
                        mesh.Normals = dataContext.CreateVertexBuffer(normals);
                    }

                    if (aiMesh.HasTangentBasis)
                    {
                        var tangents = aiMesh.Tangents.Select(t => GetVec3(t)).ToArray();
                        var biTangents = aiMesh.BiTangents.Select(t => GetVec3(t)).ToArray();
                        mesh.Tangents = dataContext.CreateVertexBuffer(tangents);
                        mesh.BiTangents = dataContext.CreateVertexBuffer(biTangents);
                    }

                    if (aiMesh.HasTextureCoords(0))
                    {
                        var texcoord = aiMesh.GetTextureCoords(0).Select(t => new Vec2(t.X, 1 - t.Y)).ToArray();
                        mesh.TextureVertexes = dataContext.CreateVertexBuffer(texcoord);
                    }

                    mesh.HasVertexes = aiMesh.HasVertices;
                    mesh.HasNormals = aiMesh.HasNormals;
                    mesh.HasTextureVertexes = aiMesh.HasTextureCoords(0);
                    mesh.HasTangentBasis = aiMesh.HasTangentBasis;

                    var inds = aiMesh.GetIndices().Select(t => (Int32)t).ToArray();                    

                    mesh.IndexedRendering = true;
                    mesh.Indicies = dataContext.CreateIndexBuffer(inds);

                    mesh.HasMaterial = aiMesh.MaterialIndex != -1;

                    if(mesh.HasMaterial)
                        mesh.Material = GetMaterial(aiScene.Materials[aiMesh.MaterialIndex], path, dataContext);                    
                }

            return scene;
        }

        public static RScene LoadScene(String filePath, IDataContext dataContext)
        {
            var path = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);

            var importer = new Assimp.AssimpImporter();

            var aiScene = importer.ImportFile(filePath, Assimp.PostProcessPreset.TargetRealTimeMaximumQuality);

            var rScene = new RScene(dataContext);

            if (aiScene.HasLights)
                for (var i = 0; i < aiScene.LightCount; i++)
                    rScene.Lights.Add(InitLight(aiScene.Lights[i]));

            if (aiScene.HasMeshes)
                for (var i = 0; i < aiScene.MeshCount; i++)
                {
                    var aiMesh = aiScene.Meshes[i];
                    var mesh = rScene.AddTriangles();
                  
                    if (aiMesh.HasVertices)
                    {
                        var verts = aiMesh.Vertices.Select(t => GetVec3(t)).ToArray();
                        mesh.Positions.AddRange(verts);
                    }

                    if (aiMesh.HasNormals)
                    {
                        var normals = aiMesh.Normals.Select(t => GetVec3(t)).ToArray();
                        mesh.Normals.AddRange(normals);
                    }

                    if (aiMesh.HasTangentBasis)
                    {
                        var tangents = aiMesh.Tangents.Select(t => GetVec3(t)).ToArray();
                        var biTangents = aiMesh.BiTangents.Select(t => GetVec3(t)).ToArray();
                        mesh.Tangents.AddRange(tangents);
                        mesh.BiTangents.AddRange(biTangents);
                    }

                    if (aiMesh.HasTextureCoords(0))
                    {
                        var texcoord = aiMesh.GetTextureCoords(0).Select(t => new Vec2(t.X, 1 - t.Y)).ToArray();
                        mesh.TexCoords.AddRange(texcoord);
                    }

                    mesh.HasNormals = aiMesh.HasNormals;
                    mesh.HasTextureVertexes = aiMesh.HasTextureCoords(0);
                    mesh.HasTangentBasis = aiMesh.HasTangentBasis;

                    var inds = aiMesh.GetIndices().Select(t => (Int32)t).ToArray();

                    mesh.IndexedRendering = true;
                    mesh.Indexes.AddRange(inds);

                    mesh.HasMaterial = aiMesh.MaterialIndex != -1;

                    if (mesh.HasMaterial)
                        mesh.Material = GetMaterial(aiScene.Materials[aiMesh.MaterialIndex], path, dataContext);
                }

            return rScene;
        }

        private static Material GetMaterial(Assimp.Material aiMaterial, String filePath, IDataContext dataContext)
        {
            var material = new Material();

            if (aiMaterial.HasColorAmbient)
                material.ColorAmbient = GetColor(aiMaterial.ColorAmbient);

            if (aiMaterial.HasColorDiffuse)
                material.ColorDiffuse = GetColor(aiMaterial.ColorDiffuse);

            if (aiMaterial.HasColorSpecular)
                material.ColorSpecular = GetColor(aiMaterial.ColorSpecular);


            material.HasColorAmbient = aiMaterial.HasColorAmbient;
            material.HasColorDiffuse = aiMaterial.HasColorDiffuse;
            material.HasColorSpecular = aiMaterial.HasColorSpecular;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Ambient) > 0)
            {
                material.TextureAmbient = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Ambient, 0), filePath, dataContext);
                if (material.TextureAmbient != null)
                    material.HasTextureAmbient = true;
            }
            else material.HasTextureAmbient = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Diffuse) > 0)
            {
                material.TextureDiffuse = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Diffuse, 0), filePath, dataContext);
                if (material.TextureDiffuse != null)
                    material.HasTextureDiffuse = true;
            }
            else material.HasTextureDiffuse = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Emissive) > 0)
            {
                material.TextureEmissive = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Emissive, 0), filePath, dataContext);
                if (material.TextureEmissive != null)
                    material.HasTextureEmissive = true;
            }
            else material.HasTextureEmissive = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Specular) > 0)
            {
                material.TextureSpecular = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Specular, 0), filePath, dataContext);
                if (material.TextureSpecular != null)
                    material.HasTextureSpecular = true;
            }
            else material.HasTextureSpecular = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Height) > 0)
            {
                material.TextureNormal = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Height, 0), filePath, dataContext);
                if (material.TextureNormal != null)
                    material.HasTextureNormal = true;
            }
            else material.HasTextureNormal = false;

            return material;
        }

        private static Material GetMaterial(Assimp.Material aiMaterial, String filePath, GlDataContext dataContext)
        {      
            var material = new Material();                       

            if (aiMaterial.HasColorAmbient)
                material.ColorAmbient = GetColor(aiMaterial.ColorAmbient);

            if (aiMaterial.HasColorDiffuse)
                material.ColorDiffuse = GetColor(aiMaterial.ColorDiffuse);

            if (aiMaterial.HasColorSpecular)            
                material.ColorSpecular = GetColor(aiMaterial.ColorSpecular);
            

            material.HasColorAmbient = aiMaterial.HasColorAmbient;
            material.HasColorDiffuse = aiMaterial.HasColorDiffuse;
            material.HasColorSpecular = aiMaterial.HasColorSpecular;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Ambient) > 0)
            {
                material.TextureAmbient = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Ambient, 0), filePath, dataContext);
                if (material.TextureAmbient != null)
                    material.HasTextureAmbient = true;
            }
            else material.HasTextureAmbient = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Diffuse) > 0)
            {
                material.TextureDiffuse = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Diffuse, 0), filePath, dataContext);
                if (material.TextureDiffuse != null)
                    material.HasTextureDiffuse = true;
            }
            else material.HasTextureDiffuse = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Emissive) > 0)
            {
                material.TextureEmissive = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Emissive, 0), filePath, dataContext);
                if (material.TextureEmissive != null)
                    material.HasTextureEmissive = true;
            }
            else material.HasTextureEmissive = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Specular) > 0)
            {
                material.TextureSpecular = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Specular, 0), filePath, dataContext);
                if (material.TextureSpecular != null)
                    material.HasTextureSpecular = true;
            }
            else material.HasTextureSpecular = false;

            if (aiMaterial.GetTextureCount(Assimp.TextureType.Height) > 0)
            {
                material.TextureNormal = GetTexture(aiMaterial.GetTexture(Assimp.TextureType.Height, 0), filePath, dataContext);
                if (material.TextureNormal != null)
                    material.HasTextureNormal = true;
            }
            else material.HasTextureNormal = false;

            return material;
        }

        private static Texture GetTexture(Assimp.TextureSlot aiTexture, String filePath, IDataContext dataContext)
        {
            if (!File.Exists(filePath + aiTexture.FilePath))
                return null;
            return dataContext.CreateTexture((Bitmap)Image.FromFile(filePath + aiTexture.FilePath));
        }

        private static GlTexture GetTexture(Assimp.TextureSlot aiTexture, String filePath, GlDataContext dataContext)
        {
            if (!File.Exists(filePath + aiTexture.FilePath))
                return null;
            return dataContext.CreateTexture((Bitmap)Image.FromFile(filePath + aiTexture.FilePath));
        }

        private static Light InitLight(Assimp.Light aiLight)
        {
            var light = new Light
            {
                LightType = (LightType) aiLight.LightType,
                Name = aiLight.Name,
                AmbientColor = GetColor(aiLight.ColorAmbient),
                DiffuseColor = GetColor(aiLight.ColorDiffuse),
                SpecularColor = GetColor(aiLight.ColorSpecular)
            };

            if ((light.LightType & LightType.Directional) == LightType.Directional)
                light.Direction = GetVec3(aiLight.Direction);

            if ((light.LightType & LightType.Point) == LightType.Point)
            {
                light.Position = GetVec3(aiLight.Position);
                light.AttenuationConstant = aiLight.AttenuationConstant;
                light.AttenuationLinear = aiLight.AttenuationLinear;
                light.AttenuationQuadratic = aiLight.AttenuationQuadratic;
            }

            if (light.LightType == LightType.Spot)
            {
                light.InnerAngle = aiLight.AngleInnerCone;
                light.OuterAngle = aiLight.AngleOuterCone;
            }

            return light;
        }

        private static Color3 GetColor(Assimp.Color3D color)
        {
            return new Color3(color.R, color.G, color.B);
        }

        private static Color4 GetColor(Assimp.Color4D color)
        {
            return new Color4(color.R, color.G, color.B, color.A);
        }

        private static Vec3 GetVec3(Assimp.Vector3D vec)
        {
            return new Vec3(vec.X, vec.Y, vec.Z);
        }

        private static Vec2 GetVec2(Assimp.Vector2D vec)
        {
            return new Vec2(vec.X, vec.Y);
        }
    }
}
