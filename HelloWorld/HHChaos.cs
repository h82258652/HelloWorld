using ComputeSharp;
using ComputeSharp.D2D1;
using System;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[AutoConstructor]
public readonly partial struct HHChaos : ID2D1PixelShader
{
    private readonly float _amount;

    public float4 Execute()
    {
        float2 uv = D2D.GetInputCoordinate(0).XY;

        uv -= 0.5f;// move to center

        // cast to Polar coordinate
        float r = Hlsl.Length(uv);
        float angle = Hlsl.Atan2(uv.Y, uv.X);// angle is in radians

        float scaledR = r / Hlsl.Length(new float2(0.5f, 0.5f));// scale r to [0,1]
        float extraAngle = Hlsl.Lerp(0f, (float)Math.PI, scaledR);// scale to [0,PI]

        angle += extraAngle * _amount;// scale the extraAngle and add it to the original angle

        // cast back to Cartesian coordinate
        float x = r * Hlsl.Cos(angle);
        float y = r * Hlsl.Sin(angle);

        // move to original
        x += 0.5f;
        y += 0.5f;

        float4 color = D2D.SampleInput(0, new float2(x, y));
        return color;
    }
}