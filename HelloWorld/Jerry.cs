using ComputeSharp;
using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(3)]
[D2DInputSimple(0)]
[D2DInputSimple(1)]
[D2DInputSimple(2)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct Jerry : ID2D1PixelShader
{
    private readonly float _threshold;

    public Jerry(float threshold)
    {
        _threshold = threshold;
    }

    public float4 Execute()
    {
        // Get noise color
        float4 noiseColor = D2D.GetInput(1);

        // If the noise color less than (threshold - 0.1f), we directly return
        // Clip(x) equals to `if(x<0) return float4(0,0,0,0);`
        // The r,g,b in the noise are the same, we can pick any of them
        Hlsl.Clip(noiseColor.R - (_threshold - 0.1f));

        // Calculate the ramp color offset
        // -0.1f that means the noiseColor is very close to the threshold
        float rampOffset = Hlsl.SmoothStep(_threshold - 0.1f, _threshold, noiseColor.R);

        // Because the white color is on the right of the ramp texture and the black color is on the left of the ramp texture
        // We need 1 - rampOffset
        float4 rampColor = D2D.SampleInput(2, new float2(1 - rampOffset, 0));

        float4 color = D2D.GetInput(0);
        color += rampColor;
        return color;
    }
}