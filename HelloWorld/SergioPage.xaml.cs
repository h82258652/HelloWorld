using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#nullable enable

namespace HelloWorld;

public sealed partial class SergioPage : Page
{
    private static readonly Random _rand = new();
    private readonly List<InnerSergio> _sergios = new();

    public SergioPage()
    {
        InitializeComponent();
        ContentGrid.AddHandler(PointerPressedEvent, new PointerEventHandler(OnContentGridPointerPressed), true);
    }

    private static double GenerateDuration()
    {
        // [1, 3)
        return _rand.NextDouble() * 2d + 1d;
    }

    private async void OnContentGridPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        Vector2 position = e.GetCurrentPoint(element).Position.ToVector2();
        double duration = GenerateDuration();
        Vector2 resolution = new((float)element.ActualWidth, (float)element.ActualHeight);
        InnerSergio sergio = new(DateTime.Now, duration, position, resolution);
        _sergios.Add(sergio);

        await Task.Delay(TimeSpan.FromSeconds(duration));

        _sergios.Remove(sergio);
    }

    private void OnJustinSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _sergios.Clear();
    }

    private ICanvasImage? OnProcessImage(JustinControl sender, IGraphicsEffectSource effectSource)
    {
        if (_sergios.Count <= 0)
        {
            return null;
        }

        DateTime now = DateTime.Now;
        IGraphicsEffectSource lastEffect = effectSource;
        for (int i = 0; i < _sergios.Count; i++)
        {
            var sergio = _sergios[i];
            sergio.UpdateEffectProperties(lastEffect, now);
            lastEffect = sergio.Effect;
        }

        return lastEffect as ICanvasImage;
    }

    private void OnVSRudyButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    internal sealed class InnerSergio
    {
        private readonly double _duration;
        private readonly Vector2 _position;
        private readonly Vector2 _resolution;
        private readonly DateTime _startTime;

        public InnerSergio(DateTime startTime, double duration, Vector2 position, Vector2 resolution)
        {
            _startTime = startTime;
            _duration = duration;
            _position = position;
            _resolution = resolution;
            Effect = new PixelShaderEffect<Sergio>();
        }

        public PixelShaderEffect<Sergio> Effect { get; }

        public void UpdateEffectProperties(IGraphicsEffectSource previousEffect, DateTime now)
        {
            Effect.Sources[0] = previousEffect;

            float progress = (float)((now - _startTime).TotalSeconds / _duration);
            Effect.ConstantBuffer = new Sergio(progress, _position, _resolution);
        }
    }
}