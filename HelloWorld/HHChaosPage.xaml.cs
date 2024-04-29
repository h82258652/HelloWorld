﻿using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Display;
using Windows.Graphics.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloWorld;

public sealed partial class HHChaosPage : Page
{
    private readonly PixelShaderEffect<HHChaos> _hhchaos = new();

    public HHChaosPage()
    {
        InitializeComponent();
    }

    private void OnMeowButtonClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private ICanvasImage OnProcessImage(IGraphicsEffectSource effectSource)
    {
        float dpiScale = DisplayInformation.GetForCurrentView().ResolutionScale.ToFloat();

        _hhchaos.Sources[0] = effectSource;
        _hhchaos.ConstantBuffer = new HHChaos((float)AmountSlider.Value, new float2(300, 300), dpiScale);
        return _hhchaos;
    }
}