using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

#nullable enable

namespace HelloWorld;

[TemplatePart(Name = ContentHostTemplateName, Type = typeof(Border))]
[TemplatePart(Name = PreviousHostTemplateName, Type = typeof(Border))]
[TemplatePart(Name = ContentEffectHostTemplateName, Type = typeof(JustinControl))]
[TemplatePart(Name = PreviousEffectHostTemplateName, Type = typeof(JustinControl))]
public class JustinFrame : Frame
{
    private const string ContentEffectHostTemplateName = "PART_ContentEffectHost";
    private const string ContentHostTemplateName = "PART_ContentHost";
    private const string PreviousEffectHostTemplateName = "PART_PreviousEffectHost";
    private const string PreviousHostTemplateName = "PART_PreviousHost";
    private readonly Compositor _compositor = Window.Current.Compositor;
    private JustinControl? _contentEffectHost;
    private Border? _contentHost;
    private Visual? _contentHostVisual;
    private JustinFrameNavigationEffectInfo? _currentEffectInfo;
    private JustinControl? _previousEffectHost;
    private Border? _previousHost;
    private DateTimeOffset? _startTime;

    public JustinFrame()
    {
        DefaultStyleKey = typeof(JustinFrame);
    }

    public async void NavigateWithEffect(Type sourcePageType, object? parameter, JustinFrameNavigationEffectInfo effectInfo)
    {
        if (_contentHost is null
            || _previousHost is null
            || _contentEffectHost is null
            || _previousEffectHost is null
            || effectInfo.Duration <= TimeSpan.Zero)
        {
            Cleanup();
            Navigate(sourcePageType, parameter, new SuppressNavigationTransitionInfo());
            return;
        }

        _currentEffectInfo = effectInfo;

        if (_currentEffectInfo.IsNewPageOnTop)
        {
            Canvas.SetZIndex(_previousEffectHost, -1);
        }
        else
        {
            Canvas.SetZIndex(_previousEffectHost, 1);
        }

        CompositionVisualSurface surface = _compositor.CreateVisualSurface();
        surface.SourceVisual = _contentHostVisual;
        surface.SourceSize = new Vector2((float)_contentHost.ActualWidth, (float)_contentHost.ActualHeight);

        ICompositionVisualSurfacePartner surfacePartner = (ICompositionVisualSurfacePartner)(object)surface;
        surfacePartner.Stretch = CompositionStretch.Fill;
        surfacePartner.RealizationSize = new Vector2((float)_contentHost.ActualWidth, (float)_contentHost.ActualHeight) * (float)_contentHost.XamlRoot.RasterizationScale;
        surfacePartner.Freeze();

        CompositionSurfaceBrush brush = _compositor.CreateSurfaceBrush(surface);

        SpriteVisual visual = _compositor.CreateSpriteVisual();
        visual.Brush = brush;
        visual.RelativeSizeAdjustment = Vector2.One;

        ElementCompositionPreview.SetElementChildVisual(_previousHost, visual);

        TaskCompletionSource<object?> tcs = new();
        EventHandler<RenderedEventArgs>? handler = null;
        handler = (s, e) =>
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendered -= handler;
            tcs.SetResult(null);
        };
        Windows.UI.Xaml.Media.CompositionTarget.Rendered += handler;

        await tcs.Task;

        _startTime = DateTimeOffset.Now;

        Navigate(sourcePageType, parameter, new SuppressNavigationTransitionInfo());

        await Task.Delay(effectInfo.Duration);

        if (_currentEffectInfo == effectInfo)
        {
            Cleanup();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _contentHost = (Border)GetTemplateChild(ContentHostTemplateName);
        _previousHost = (Border)GetTemplateChild(PreviousHostTemplateName);

        _contentHostVisual = ElementCompositionPreview.GetElementVisual(_contentHost);

        _contentEffectHost = (JustinControl)GetTemplateChild(ContentEffectHostTemplateName);
        _previousEffectHost = (JustinControl)GetTemplateChild(PreviousEffectHostTemplateName);

        _contentEffectHost.ProcessImage += OnContentEffectHostProcessImage;
        _previousEffectHost.ProcessImage += OnPreviousEffectHostProcessImage;
    }

    private void Cleanup()
    {
        ElementCompositionPreview.SetElementChildVisual(_previousHost, null);

        _startTime = null;

        _currentEffectInfo = null;
    }

    private ICanvasImage? OnContentEffectHostProcessImage(IGraphicsEffectSource effectSource)
    {
        if (_currentEffectInfo is not null && _startTime.HasValue)
        {
            TimeSpan duration = _currentEffectInfo.Duration;
            DateTimeOffset now = DateTimeOffset.Now;
            double progress = Math.Clamp((now - _startTime.Value) / duration, 0, 1);
            return _currentEffectInfo.ProcessNewPageEffect(effectSource, progress);
        }

        return null;
    }

    private ICanvasImage? OnPreviousEffectHostProcessImage(IGraphicsEffectSource effectSource)
    {
        if (_currentEffectInfo is not null && _startTime.HasValue)
        {
            TimeSpan duration = _currentEffectInfo.Duration;
            DateTimeOffset now = DateTimeOffset.Now;
            double progress = Math.Clamp((now - _startTime.Value) / duration, 0, 1);
            return _currentEffectInfo.ProcessOldPageEffect(effectSource, progress);
        }

        return null;
    }
}