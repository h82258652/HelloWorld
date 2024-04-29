using Windows.Graphics.Display;

namespace HelloWorld;

public static class ResolutionScaleExtensions
{
    public static float ToFloat(this ResolutionScale scale)
    {
        switch (scale)
        {
            case ResolutionScale.Scale100Percent:
                return 1.0f;

            case ResolutionScale.Scale120Percent:
                return 1.2f;

            case ResolutionScale.Scale125Percent:
                return 1.25f;

            case ResolutionScale.Scale140Percent:
                return 1.4f;

            case ResolutionScale.Scale150Percent:
                return 1.5f;

            case ResolutionScale.Scale160Percent:
                return 1.6f;

            case ResolutionScale.Scale175Percent:
                return 1.75f;

            case ResolutionScale.Scale180Percent:
                return 1.8f;

            case ResolutionScale.Scale200Percent:
                return 2.0f;

            case ResolutionScale.Scale225Percent:
                return 2.25f;

            case ResolutionScale.Scale250Percent:
                return 2.50f;

            default:
                return 1.0f;
        }
    }
}