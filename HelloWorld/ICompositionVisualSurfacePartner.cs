using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.UI.Composition;

namespace HelloWorld
{
    /// <summary>
    /// https://github.com/UnigramDev/Unigram/blob/develop/Telegram/Composition/Interop.cs
    /// </summary>
    [Guid("F26DA89E-683D-4C67-AEA7-BA29B2217A70")]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface ICompositionVisualSurfacePartner
    {
        Vector2 RealizationSize { get; set; }

        CompositionStretch Stretch { get; set; }

        void Freeze();
    }
}