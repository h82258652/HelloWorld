using ComputeSharp;
using ComputeSharp.D2D1;
using System;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct Sergio : ID2D1PixelShader
{
    private readonly float2 _position;
    private readonly float _progress;
    private readonly float2 _resolution;

    public Sergio(float progress, float2 position, float2 resolution)
    {
        _progress = progress;
        _position = position;
        _resolution = resolution;
    }

    public float4 Execute()
    {
        float2 uv = D2D.GetScenePosition().XY / Hlsl.Min(_resolution.X, _resolution.Y);

        uv -= 0.5f; // move to center

        // cast to Polar coordinate
        float r = Hlsl.Length(uv);
        float angle = Hlsl.Atan2(uv.Y, uv.X); // angle is in radians

        // Cast pressed position to [0,1]
        float2 pressedPoint = _position / Hlsl.Min(_resolution.X, _resolution.Y);

        pressedPoint = pressedPoint - 0.5f;

        // calculate the distance from the position that user clicked
        float distance = Hlsl.Distance(uv, pressedPoint);

        float easingProgress = CircularOut(_progress);

        float t = (distance * 10f - easingProgress) * Hlsl.Min(_resolution.X, _resolution.Y) / 8f;

        if (t > 0 && t < (float)Math.PI * 2.0f)
        {
            r += Hlsl.Sin(t) * 0.01f * (1 - easingProgress);
        }

        // cast back to Cartesian coordinate
        float x = r * Hlsl.Cos(angle);
        float y = r * Hlsl.Sin(angle);

        // move to original
        x += 0.5f;
        y += 0.5f;

        float4 color = D2D.SampleInputAtPosition(0, new float2(x, y) * Hlsl.Min(_resolution.X, _resolution.Y));
        return color;
    }

    private float CircularOut(float t)
    {
        return Hlsl.Sqrt((2.0f - t) * t);
    }
}