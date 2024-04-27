using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;

#nullable enable

namespace HelloWorld;

[TemplatePart(Name = RootGridTemplateName, Type = typeof(Grid))]
[ContentProperty(Name = nameof(Child))]
public class JustinControl : Control
{
    public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(
        nameof(Child),
        typeof(UIElement),
        typeof(JustinControl),
        new PropertyMetadata(null, OnChildChanged));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(JustinControl),
        new PropertyMetadata(false, OnIsActiveChanged));

    private const string RootGridTemplateName = "PART_RootGrid";

    private readonly CanvasControl _canvasControl;
    private readonly Border _childContainer;
    private readonly Visual _childContainerVisual;

    private CanvasBitmap? _bitmap;
    private Direct3D11CaptureFramePool? _captureFramePool;
    private GraphicsCaptureSession? _captureSession;
    private CanvasDevice? _device;

    public JustinControl()
    {
        DefaultStyleKey = typeof(JustinControl);

        _childContainer = new Border();
        _childContainerVisual = ElementCompositionPreview.GetElementVisual(_childContainer);

        _canvasControl = new CanvasControl
        {
            IsHitTestVisible = false
        };
        _canvasControl.Draw += OnDraw;
        _canvasControl.CreateResources += OnCreateResources;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    public event EventHandler? DeviceReady;

    public event JustinControlProcessImageEventHandler? ProcessImage;

    public UIElement? Child
    {
        get => (UIElement?)GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    public CanvasDevice? Device => _device;

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Grid rootGrid = (Grid)GetTemplateChild(RootGridTemplateName);
        rootGrid.Children.Add(_childContainer);
        rootGrid.Children.Add(_canvasControl);
    }

    private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        JustinControl self = (JustinControl)d;
        if (e.OldValue is UIElement)
        {
            self._childContainer.Child = null;
        }

        if (e.NewValue is UIElement newChild)
        {
            self._childContainer.Child = newChild;
        }
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        JustinControl self = (JustinControl)d;
        bool oldValue = (bool)e.OldValue;
        if (oldValue)
        {
            self.Uninitialize();
        }

        bool newValue = (bool)e.NewValue;
        if (newValue)
        {
            self.Initialize();
        }
    }

    private SizeInt32 GetIntSize()
    {
        return new SizeInt32
        {
            Width = (int)ActualWidth,
            Height = (int)ActualHeight
        };
    }

    private void Initialize()
    {
        if (!IsActive || !IsLoaded)
        {
            return;
        }

        if (_captureFramePool is not null && _captureSession is not null)
        {
            return;
        }

        if (_device is null)
        {
            return;
        }

        SizeInt32 size = GetIntSize();
        if (IsInvalidSize(size))
        {
            return;
        }

        _childContainerVisual.Opacity = 0;
        GraphicsCaptureItem captureItem = GraphicsCaptureItem.CreateFromVisual(_childContainerVisual);
        _captureFramePool = Direct3D11CaptureFramePool.Create(_device, DirectXPixelFormat.B8G8R8A8UIntNormalized, 1, size);
        _captureFramePool.FrameArrived += OnFrameArrived;
        _captureSession = _captureFramePool.CreateCaptureSession(captureItem);
        _captureSession.StartCapture();
    }

    private bool IsInvalidSize(SizeInt32 size)
    {
        return size.Width <= 0 || size.Height <= 0;
    }

    private void OnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        _device = sender.Device;

        DeviceReady?.Invoke(this, EventArgs.Empty);

        Initialize();
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        args.DrawingSession.Clear(Colors.Transparent);

        if (_bitmap is null)
        {
            return;
        }

        ICanvasImage? processedImage = ProcessImage?.Invoke(_bitmap);
        if (processedImage is not null)
        {
            args.DrawingSession.DrawImage(processedImage);
        }
        else
        {
            args.DrawingSession.DrawImage(_bitmap);
        }
    }

    private void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
    {
        using Direct3D11CaptureFrame frame = sender.TryGetNextFrame();
        ProcessFrame(frame);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Initialize();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SizeInt32 size = GetIntSize();

        if (IsInvalidSize(size))
        {
            Uninitialize();
            return;
        }

        if (_captureFramePool is not null && _captureSession is not null)
        {
            if (_device is not null)
            {
                _captureFramePool.Recreate(_device, DirectXPixelFormat.B8G8R8A8UIntNormalized, 1, size);
            }
        }
        else
        {
            Initialize();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Uninitialize();
    }

    private void ProcessFrame(Direct3D11CaptureFrame frame)
    {
        _bitmap?.Dispose();
        _bitmap = CanvasBitmap.CreateFromDirect3D11Surface(_device, frame.Surface);

        _canvasControl.Invalidate();
    }

    private void Uninitialize()
    {
        _bitmap?.Dispose();
        _bitmap = null;
        _captureSession?.Dispose();
        _captureSession = null;
        _captureFramePool?.Dispose();
        _captureFramePool = null;
        _childContainerVisual.Opacity = 1;
        _canvasControl.Invalidate();
    }
}