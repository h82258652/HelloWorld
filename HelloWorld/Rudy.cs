using ComputeSharp;
using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct Rudy : ID2D1PixelShader
{
    private const float shake_block_size = 30.5f;
    private const float shake_color_rate = 0.01f;
    private const float shake_power = 0.03f;
    private const float shake_rate = 0.2f;
    private const float shake_speed = 1.0f;
    private readonly float _time;

    public Rudy(float time)
    {
        _time = time;
    }

    public float4 Execute()
    {
        float enable_shift = Hlsl.BoolToFloat(random(Hlsl.Trunc(_time * shake_speed)) < shake_rate);

        float4 SCREEN_UV = D2D.GetInputCoordinate(0);
        float2 fixed_uv = SCREEN_UV.XY;// SCREEN_UV;
        fixed_uv.X += (
            random(
                (Hlsl.Trunc(SCREEN_UV.Y * shake_block_size) / shake_block_size)
            + _time
            ) - 0.5f
        ) * shake_power * enable_shift;

        float4 pixel_color = D2D.SampleInput(0, fixed_uv);
        pixel_color.R = Hlsl.Lerp(
            pixel_color.R
        , D2D.SampleInput(0, fixed_uv + new float2(shake_color_rate, 0.0f)).R
        , enable_shift
        );
        pixel_color.B = Hlsl.Lerp(
            pixel_color.B
        , D2D.SampleInput(0, fixed_uv + new float2(-shake_color_rate, 0.0f)).B
        , enable_shift
        );

        return pixel_color;
    }

    private float random(float seed)
    {
        return Hlsl.Frac(543.2543f * Hlsl.Sin(Hlsl.Dot(new float2(seed, seed), new float2(3525.46f, -54.3415f))));
    }
}