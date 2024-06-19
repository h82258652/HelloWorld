using ComputeSharp;
using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct Pokemon : ID2D1PixelShader
{
    private readonly float _progress;
    private readonly bool _reverse;
    private readonly float2 _resolution;

    public Pokemon(float progress, float2 resolution, bool reverse)
    {
        _progress = progress;
        _resolution = resolution;
        _reverse = reverse;
    }

    public float4 Execute()
    {
        float4 color = D2D.GetInput(0);
        if (_reverse)
        {
            if (_progress <= 0)
            {
                return new float4(0, 0, 0, 0);
            }
            else if (_progress >= 1)
            {
                return color;
            }
        }
        else
        {
            if (_progress <= 0)
            {
                return color;
            }
            else if (_progress >= 1)
            {
                return new float4(0, 0, 0, 0);
            }
        }

        float4 uv = D2D.GetInputCoordinate(0);
        float2 xy = Hlsl.Floor(uv.XY * _resolution);
        float rand = Random2(xy);
        if (_reverse)
        {
            if (_progress < rand)
            {
                return new float4(0, 0, 0, 0);
            }
        }
        else
        {
            if (_progress > rand)
            {
                return new float4(0, 0, 0, 0);
            }
        }

        return color;
    }

    private float Random2(float2 st)
    {
        return Hlsl.Frac(Hlsl.Sin(Hlsl.Dot(st.XY, new float2(12.9898f, 78.233f))) * 43758.5453f);
    }
}