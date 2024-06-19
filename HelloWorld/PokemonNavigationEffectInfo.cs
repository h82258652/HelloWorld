using ComputeSharp.D2D1.Uwp;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Graphics.Effects;

namespace HelloWorld;

public class PokemonNavigationEffectInfo : JustinFrameNavigationEffectInfo
{
    private readonly int _height;
    private readonly PixelShaderEffect<Pokemon> _newPokemon = new();
    private readonly PixelShaderEffect<Pokemon> _oldPokemon = new();
    private readonly int _width;

    public PokemonNavigationEffectInfo(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public override TimeSpan Duration => TimeSpan.FromSeconds(0.8);

    public override ICanvasImage ProcessNewPageEffect(IGraphicsEffectSource effectSource, double progress)
    {
        _newPokemon.Sources[0] = effectSource;
        _newPokemon.ConstantBuffer = new Pokemon((float)progress, new float2(_width, _height), true);
        return _newPokemon;
    }

    public override ICanvasImage ProcessOldPageEffect(IGraphicsEffectSource effectSource, double progress)
    {
        _oldPokemon.Sources[0] = effectSource;
        _oldPokemon.ConstantBuffer = new Pokemon((float)progress, new float2(_width, _height), false);
        return _oldPokemon;
    }
}