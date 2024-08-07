using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloWorld;

public sealed partial class JustinPage : Page
{
    private readonly PixelShaderEffect<Justin> _justin = new();

    public JustinPage()
    {
        InitializeComponent();
    }

    private void OnGoJustinButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private ICanvasImage OnProcessImage(JustinControl sender, IGraphicsEffectSource effectSource)
    {
        _justin.Sources[0] = effectSource;
        return _justin;
    }
}