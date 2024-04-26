using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloWorld;

public sealed partial class MainPage : Page
{
    private readonly List<int> _currentAmbie = new();
    private readonly IReadOnlyList<int> _targetHHChaos = new List<int>() { (int)VirtualKey.H, (int)VirtualKey.H, (int)VirtualKey.C, (int)VirtualKey.H, (int)VirtualKey.A, (int)VirtualKey.O, (int)VirtualKey.S };
    private readonly IReadOnlyList<int> _targetJustin = new List<int>() { (int)VirtualKey.J, (int)VirtualKey.U, (int)VirtualKey.S, (int)VirtualKey.T, (int)VirtualKey.I, (int)VirtualKey.N };

    public MainPage()
    {
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

        if (CheckAmbie(_currentAmbie, _targetJustin))
        {
            _currentAmbie.Clear();
            Frame.Navigate(typeof(JustinPage));
        }
        else if (CheckAmbie(_currentAmbie, _targetHHChaos))
        {
            _currentAmbie.Clear();
            Frame.Navigate(typeof(HHChaosPage));
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