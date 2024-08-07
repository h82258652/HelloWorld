using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloWorld;

public sealed partial class HuhPage : Page
{
    private readonly EdgeDetectionEffect _edgeDetectionEffect;
    private readonly CompositeEffect _final;
    private readonly PixelShaderEffect<Huh> _huh;

    public HuhPage()
    {
        _huh = new PixelShaderEffect<Huh>();
        _edgeDetectionEffect = new EdgeDetectionEffect()
        {
            Amount = 0.2f
        };
        _final = new CompositeEffect();
        _final.Sources.Add(_huh);
        _final.Sources.Add(new ChromaKeyEffect
        {
            Source = new InvertEffect
            {
                Source = new GrayscaleEffect
                {
                    Source = _edgeDetectionEffect
                }
            },
            Color = Colors.White,
            Feather = true
        });

        InitializeComponent();
    }

    private void OnHuhButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private ICanvasImage OnProcessImage(JustinControl sender, IGraphicsEffectSource effectSource)
    {
        _huh.Sources[0] = effectSource;
        _edgeDetectionEffect.Source = effectSource;
        return _final;
    }
}