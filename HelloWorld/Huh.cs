using ComputeSharp;
using ComputeSharp.D2D1;

namespace HelloWorld;

[D2DInputCount(1)]
[D2DInputSimple(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct Huh : ID2D1PixelShader
{
    public float4 Execute()
    {
        var color = D2D.GetInput(0);

        var hsl = RGB2HSL(color.RGB);
        var lightness = hsl.Z;
        lightness = Hlsl.Ceil(lightness * 10) / 10f;

        hsl.Z = lightness;

        var finalColor = HSL2RGB(hsl);
        return new float4(finalColor, color.A);
    }

    private float3 HSL2RGB(float3 hsl)
    {
        float3 rgb = new float3();

        if (hsl.Y == 0.0)
        {
            rgb = new float3(hsl.Z, hsl.Z, hsl.Z); // Luminance
        }
        else
        {
            float f2;

            if (hsl.Z < 0.5f)
                f2 = hsl.Z * (1.0f + hsl.Y);
            else
                f2 = hsl.Z + hsl.Y - hsl.Y * hsl.Z;

            float f1 = 2.0f * hsl.Z - f2;

            rgb.R = hue2rgb(f1, f2, hsl.X + (1.0f / 3.0f));
            rgb.G = hue2rgb(f1, f2, hsl.X);
            rgb.B = hue2rgb(f1, f2, hsl.X - (1.0f / 3.0f));
        }
        return rgb;
    }

    private float hue2rgb(float f1, float f2, float hue)
    {
        if (hue < 0.0f)
            hue += 1.0f;
        else if (hue > 1.0f)
            hue -= 1.0f;
        float res;
        if ((6.0f * hue) < 1.0f)
            res = f1 + (f2 - f1) * 6.0f * hue;
        else if ((2.0f * hue) < 1.0f)
            res = f2;
        else if ((3.0f * hue) < 2.0f)
            res = f1 + (f2 - f1) * ((2.0f / 3.0f) - hue) * 6.0f;
        else
            res = f1;
        return res;
    }

    private float3 RGB2HSL(float3 color)
    {
        float h = 0.0f;
        float s = 0.0f;
        float l;
        float r = color.R;
        float g = color.G;
        float b = color.B;
        float cMin = Hlsl.Min(r, Hlsl.Min(g, b));
        float cMax = Hlsl.Max(r, Hlsl.Max(g, b));

        l = (cMax + cMin) / 2.0f;
        if (cMax > cMin)
        {
            float cDelta = cMax - cMin;

            //s = l < .05 ? cDelta / ( cMax + cMin ) : cDelta / ( 2.0 - ( cMax + cMin ) ); Original
            s = l < .0 ? cDelta / (cMax + cMin) : cDelta / (2.0f - (cMax + cMin));

            if (r == cMax)
            {
                h = (g - b) / cDelta;
            }
            else if (g == cMax)
            {
                h = 2.0f + (b - r) / cDelta;
            }
            else
            {
                h = 4.0f + (r - g) / cDelta;
            }

            if (h < 0.0)
            {
                h += 6.0f;
            }
            h = h / 6.0f;
        }
        return new float3(h, s, l);
    }
}