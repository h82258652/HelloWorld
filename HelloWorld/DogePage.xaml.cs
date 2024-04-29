using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Display;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloWorld;

public sealed partial class DogePage : Page
{
    private readonly PixelShaderEffect<Doge> _doge = new();

    public DogePage()
    {
        InitializeComponent();
    }

    private void OnDogeButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private ICanvasImage OnProcessImage(IGraphicsEffectSource effectSource)
    {
        float dpiScale = DisplayInformation.GetForCurrentView().ResolutionScale.ToFloat();

        _doge.Sources[0] = effectSource;
        _doge.ConstantBuffer = new Doge(new float2((float)BlockSizeSlider.Value, (float)BlockSizeSlider.Value), new float2(300, 300), dpiScale);
        return _doge;
    }
}