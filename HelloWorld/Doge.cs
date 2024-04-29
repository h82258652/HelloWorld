using ComputeSharp;
using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct Doge : ID2D1PixelShader
{
    private readonly float2 _blockSize;
    private readonly float2 _resolution;
    private readonly float _dpiScale;

    public Doge(float2 blockSize, float2 resolution, float dpiScale)
    {
        _blockSize = blockSize;
        _resolution = resolution;
        _dpiScale = dpiScale;
    }

    public float4 Execute()
    {
        float2 uv = D2D.GetScenePosition().XY / _resolution / _dpiScale;

        float2 blocks = _resolution / _blockSize;

        float2 mosaicUV = Hlsl.Floor(uv * blocks) / blocks;
        float4 mosaicColor = D2D.SampleInput(0, mosaicUV);

        return mosaicColor;
    }
}