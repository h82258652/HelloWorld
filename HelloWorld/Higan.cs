using ComputeSharp;
using ComputeSharp.D2D1;
using System;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct Higan : ID2D1PixelShader
{
    private readonly float2 _center;
    private readonly float _progress;

    public Higan(float progress, float2 center)
    {
        _center = center;
        _progress = progress;
    }

    public float4 Execute()
    {
        float2 uv = D2D.GetInputCoordinate(0).XY;

        uv -= 0.5f;// move to center

        // cast to Polar coordinate
        float r = Hlsl.Length(uv);
        float angle = Hlsl.Atan2(uv.Y, uv.X);// angle is in radians

        // calculate the distance from the position that user clicked
        float distance = Hlsl.Distance(uv, _center - new float2(0.5f, 0.5f));

        // Hmm, actually I don't know why this...
        float t = distance * 300.0f - _progress * 130.0f;

        // this condition can make there is only one wave
        if (t > 0 && t < Math.PI * 2.0f)
        {
            // 0.1 to decrease the power of the wave
            // 1 - _progress that means the wave power will decrease as _progress goes by
            r += Hlsl.Sin(t) * 0.1f * (1 - _progress);
        }

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