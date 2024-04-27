using ComputeSharp;
using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[AutoConstructor]
public readonly partial struct Doge : ID2D1PixelShader
{
    private readonly float2 _blockSize;
    private readonly float2 _resolution;

    public float4 Execute()
    {
        float2 uv = D2D.GetInputCoordinate(0).XY;

        float2 blocks = _resolution / _blockSize;

        float2 mosaicUV = Hlsl.Floor(uv * blocks) / blocks;
        float4 mosaicColor = D2D.SampleInput(0, mosaicUV);

        return mosaicColor;
    }
}