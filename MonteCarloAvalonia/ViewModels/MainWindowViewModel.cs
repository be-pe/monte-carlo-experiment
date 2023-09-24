using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI.Fody.Helpers;

namespace MonteCarloAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    public string SampleText { get; set; }
    [Reactive]
    public string PiText { get; set; }
    public int Size => 500;
    public int SampleSize { get; set; } = 10_000;
    private int AmountInCircle { get; set; }
    
    [Reactive] 
    public ObservableCollection<PointData> Points { get; set; }

    public MainWindowViewModel()
    {
        SampleText = "Not simulated yet";
        PiText = string.Empty;
        Points = new();
    }

    [RelayCommand]
    private async Task Simulate()
    {
        AmountInCircle = 0;
        var points = new ConcurrentBag<PointData>();
        await Parallel.ForEachAsync(Enumerable.Range(0, SampleSize), (_,_) =>
        {
            var x = Random.Shared.NextDouble();
            var y = Random.Shared.NextDouble();

            var point = new PointData
            {
                X = x * Size,
                Y = y * Size
            };
            
            if (x*x+y*y <= 1)
            {
                AmountInCircle += 1;
                point.Fill = Brushes.Red;
            }

            points.Add(point);

            return ValueTask.CompletedTask;
        });

        Points = new ObservableCollection<PointData>(points);

        SampleText = $"{AmountInCircle} points in the circle out of {SampleSize}.";
        PiText = $"PI \u2248 {AmountInCircle * 1d / SampleSize * 4}";
    }
}

public class PointData
{
    public double X { get; init; }
    public double Y { get; init; }
    public IBrush Fill { get; set; } = Brushes.Blue;
}