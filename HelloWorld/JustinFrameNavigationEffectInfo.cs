using Microsoft.Graphics.Canvas;
using System;
using Windows.Graphics.Effects;

#nullable enable

namespace HelloWorld;

public abstract class JustinFrameNavigationEffectInfo
{
    public abstract TimeSpan Duration { get; }

    public bool IsNewPageOnTop { get; protected set; } = true;

    public abstract ICanvasImage? ProcessNewPageEffect(IGraphicsEffectSource effectSource, double progress);

    public abstract ICanvasImage? ProcessOldPageEffect(IGraphicsEffectSource effectSource, double progress);
}