using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
public readonly partial struct Justin : ID2D1PixelShader
{
    public float4 Execute()
    {
        float4 color = D2D.GetInput(0);
        color.R = 1 - color.R;
        color.G = 1 - color.G;
        color.B = 1 - color.B;
        return color;
    }
}