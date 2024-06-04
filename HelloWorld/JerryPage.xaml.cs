using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace HelloWorld;

public sealed partial class JerryPage : Page
{
    private readonly PixelShaderEffect<Jerry> _jerry = new();

    private CanvasBitmap? _noise;
    private CanvasBitmap? _rampTex;

    public JerryPage()
    {
        InitializeComponent();
    }

    private async void OnDeviceReady(object sender, EventArgs e)
    {
        if (JustinControl.Device is { } device)
        {
            _noise = await CanvasBitmap.LoadAsync(device, new Uri("ms-appx:///Assets/dissolve_noise.png"), JustinControl.Dpi);
            _rampTex = await CanvasBitmap.LoadAsync(device, new Uri("ms-appx:///Assets/afmhot.png"), JustinControl.Dpi);
        }
    }

    private ICanvasImage? OnProcessImage(IGraphicsEffectSource effectSource)
    {
        if (_noise is null || _rampTex is null)
        {
            return null;
        }

        if (JustinControl.Dpi != _noise.Dpi || JustinControl.Dpi != _rampTex.Dpi)
        {
            return null;
        }

        _jerry.Sources[0] = effectSource;
        _jerry.Sources[1] = _noise;
        _jerry.Sources[2] = _rampTex;
        _jerry.ConstantBuffer = new Jerry((float)ThresholdSlider.Value);
        return _jerry;
    }

    private void OnTomButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}