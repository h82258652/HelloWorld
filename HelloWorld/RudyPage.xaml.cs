using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace HelloWorld;

public sealed partial class RudyPage : Page
{
    public static readonly DependencyProperty HHChaosProperty = DependencyProperty.Register(
        nameof(HHChaos),
        typeof(double),
        typeof(RudyPage),
        new PropertyMetadata(0d));

    private readonly PixelShaderEffect<HHChaos> _hhchaos = new();
    private readonly PixelShaderEffect<Justin> _justin = new();
    private readonly PixelShaderEffect<Rudy> _rudy = new();

    public RudyPage()
    {
        InitializeComponent();
    }

    public double HHChaos
    {
        get => (double)GetValue(HHChaosProperty);
        set => SetValue(HHChaosProperty, value);
    }

    private ICanvasImage OnInnerProcessImage(IGraphicsEffectSource effectSource)
    {
        _justin.Sources[0] = effectSource;
        return _justin;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Storyboard storyboard = new();
        DoubleAnimation animation = new()
        {
            EnableDependentAnimation = true,
            From = -1.5,
            To = 1.5,
            Duration = TimeSpan.FromSeconds(3),
            RepeatBehavior = RepeatBehavior.Forever,
            AutoReverse = true
        };
        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, nameof(HHChaos));
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

    private ICanvasImage OnOuterProcessImage(IGraphicsEffectSource effectSource)
    {
        _hhchaos.Sources[0] = effectSource;
        _hhchaos.ConstantBuffer = new HHChaos((float)HHChaos);
        return _hhchaos;
    }

    private ICanvasImage OnProcessImage(IGraphicsEffectSource effectSource)
    {
        _rudy.Sources[0] = effectSource;
        _rudy.ConstantBuffer = new Rudy(DateTime.Now.Millisecond);
        return _rudy;
    }

    private void OnRudyButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}