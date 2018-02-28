﻿namespace Mesher.GraphicsCore.ShaderProgram
{
    public enum ShaderVariable
    {
        ProjectionMatrix,
        ModelMatrix,
        ViewMatrix,
        ModelViewMatrix,
        MeshVertexes,
        MeshTextureVertexes,
        MeshNormals,
        MeshTangents,
        MeshBiTangents,
        LightPosition,
        MaterialTextureDiffuse,
        MaterialHasTextureDiffuse,
        LightAmbientColor,
        LightDiffuseColor,
        LightSpecularColor,
        LightDirection,
        LightInnerAngle,
        LightOuterAngle,
        LightAttenuationConstant,
        LightAttenuationLinear,
        LightAttenuationQuadratic,
        LightLightType,
        LightLightsCount,
        MaterialHasColorAmbient,
        MaterialColorAmbient,
        MaterialHasColorDiffuse,
        MaterialColorDiffuse,
        MaterialHasColorSpecular,
        MaterialColorSpecular,
        MaterialHasShininess,
        MaterialShininess,
        MaterialHasTextureAmbient,
        MaterialTextureAmbient,
        MaterialHasTextureSpecular,
        MaterialTextureSpecular,
        MaterialHasTextureEmissive,
        MaterialTextureEmissive,
        MaterialHasTextureNormal,
        MaterialTextureNormal,
        MeshHasVertexes,
        MeshHasTextureVertexes,
        MeshHasNormals,
        MeshHasTangentBasis
    }
}
