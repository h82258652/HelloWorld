using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

#nullable enable

namespace HelloWorld;

public sealed partial class HiganPage : Page
{
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress),
        typeof(double),
        typeof(HiganPage),
        new PropertyMetadata(0d));

    private readonly PixelShaderEffect<Higan> _higan = new();

    private float2? _center;
    private Storyboard? _storyboard;

    public HiganPage()
    {
        InitializeComponent();
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    private void OnHiganBorderPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        Storyboard? previousStoryboard = _storyboard;
        if (previousStoryboard is not null)
        {
            previousStoryboard.Completed -= OnStoryboardCompleted;
            previousStoryboard.Stop();
        }

        FrameworkElement element = (FrameworkElement)sender;
        Point position = e.GetCurrentPoint(element).Position;
        double x = position.X / element.ActualWidth;
        double y = position.Y / element.ActualHeight;
        _center = new float2((float)x, (float)y);

        Storyboard storyboard = new();
        DoubleAnimation animation = new()
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(1.5),
            EnableDependentAnimation = true,
            EasingFunction = new CircleEase
            {
                EasingMode = EasingMode.EaseOut
            }
        };
        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, nameof(Progress));
        storyboard.Children.Add(animation);
        storyboard.Completed += OnStoryboardCompleted;
        storyboard.Begin();

        _storyboard = storyboard;
    }

    private ICanvasImage? OnProcessImage(IGraphicsEffectSource effectSource)
    {
        if (!_center.HasValue)
        {
            return null;
        }

        _higan.Sources[0] = effectSource;
        _higan.ConstantBuffer = new Higan((float)Progress, _center.Value);
        return _higan;
    }

    private void OnStoryboardCompleted(object sender, object e)
    {
        _center = null;
    }

    private void OnVSButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}