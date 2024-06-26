﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloWorld;

public sealed partial class MainPage : Page
{
    private readonly Dictionary<Type, IReadOnlyList<int>> _allTargets;
    private readonly List<int> _currentAmbie = new();
    private readonly IReadOnlyList<int> _targetDoge = new List<int>() { (int)VirtualKey.D, (int)VirtualKey.O, (int)VirtualKey.G, (int)VirtualKey.E };
    private readonly IReadOnlyList<int> _targetHHChaos = new List<int>() { (int)VirtualKey.H, (int)VirtualKey.H, (int)VirtualKey.C, (int)VirtualKey.H, (int)VirtualKey.A, (int)VirtualKey.O, (int)VirtualKey.S };
    private readonly IReadOnlyList<int> _targetHigan = new List<int>() { (int)VirtualKey.H, (int)VirtualKey.I, (int)VirtualKey.G, (int)VirtualKey.A, (int)VirtualKey.N };
    private readonly IReadOnlyList<int> _targetHuh = new List<int>() { (int)VirtualKey.H, (int)VirtualKey.U, (int)VirtualKey.H };
    private readonly IReadOnlyList<int> _targetJerry = new List<int>() { (int)VirtualKey.J, (int)VirtualKey.E, (int)VirtualKey.R, (int)VirtualKey.R, (int)VirtualKey.Y };
    private readonly IReadOnlyList<int> _targetJustin = new List<int>() { (int)VirtualKey.J, (int)VirtualKey.U, (int)VirtualKey.S, (int)VirtualKey.T, (int)VirtualKey.I, (int)VirtualKey.N };
    private readonly IReadOnlyList<int> _targetRudy = new List<int>() { (int)VirtualKey.R, (int)VirtualKey.U, (int)VirtualKey.D, (int)VirtualKey.Y };
    private readonly IReadOnlyList<int> _targetSergio = new List<int>() { (int)VirtualKey.S, (int)VirtualKey.E, (int)VirtualKey.R, (int)VirtualKey.G, (int)VirtualKey.I, (int)VirtualKey.O };

    public MainPage()
    {
        _allTargets = new Dictionary<Type, IReadOnlyList<int>>()
        {
            { typeof(JustinPage), _targetJustin },
            { typeof(HHChaosPage), _targetHHChaos },
            { typeof(DogePage), _targetDoge },
            { typeof(JerryPage), _targetJerry },
            { typeof(HuhPage), _targetHuh },
            { typeof(RudyPage), _targetRudy },
            { typeof(HiganPage), _targetHigan },
            { typeof(SergioPage), _targetSergio },
        };

        InitializeComponent();
    }

    private static bool CheckAmbie(IReadOnlyList<int> daniel, IReadOnlyList<int> target)
    {
        return daniel.SequenceEqual(target);
    }

    private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
    {
        if (args.VirtualKey == VirtualKey.Escape)
        {
            _currentAmbie.Clear();
            return;
        }

        _currentAmbie.Add((int)args.VirtualKey);

        foreach (KeyValuePair<Type, IReadOnlyList<int>> kv in _allTargets)
        {
            IReadOnlyList<int> v = kv.Value;
            if (CheckAmbie(_currentAmbie, v))
            {
                _currentAmbie.Clear();
                Type k = kv.Key;
                ((JustinFrame)Frame).NavigateWithEffect(k, null, new PokemonNavigationEffectInfo((int)ActualWidth, (int)ActualHeight));
                return;
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CoreWindow.GetForCurrentThread().KeyDown += OnKeyDown;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CoreWindow.GetForCurrentThread().KeyDown -= OnKeyDown;
    }
}