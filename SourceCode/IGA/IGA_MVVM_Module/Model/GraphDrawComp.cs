using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace IGA_GUI_Module.Model;

/// <summary>
/// Wrapper class for LiveCharts2 chart configuration and data management
/// </summary>
public class GraphDrawComp
{
    /// <summary>
    /// Chart title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// X-axis configuration
    /// </summary>
    public AxisConfig XAxisConfig { get; set; } = new();

    /// <summary>
    /// Y-axis configuration
    /// </summary>
    public AxisConfig YAxisConfig { get; set; } = new();

    /// <summary>
    /// Collection of chart series data
    /// </summary>
    public ObservableCollection<ChartSeriesData> SeriesDataCollection { get; set; } = new();

    /// <summary>
    /// Whether to show legend
    /// </summary>
    public bool ShowLegend { get; set; } = true;

    /// <summary>
    /// Add a new series to the chart
    /// </summary>
    public void AddSeries(ChartSeriesData __seriesData)
    {
        SeriesDataCollection.Add(__seriesData);
    }

    /// <summary>
    /// Remove a series from the chart
    /// </summary>
    public void RemoveSeries(ChartSeriesData __seriesData)
    {
        SeriesDataCollection.Remove(__seriesData);
    }

    /// <summary>
    /// Clear all series
    /// </summary>
    public void ClearAllSeries()
    {
        SeriesDataCollection.Clear();
    }

    /// <summary>
    /// Convert internal series data to LiveCharts2 ISeries collection
    /// </summary>
    public ObservableCollection<ISeries> GetLiveChartsSeries()
    {
        var series = new ObservableCollection<ISeries>();

        foreach (var seriesData in SeriesDataCollection)
        {
            ISeries chartSeries = seriesData.SeriesType switch
            {
                ChartSeriesType.Line => CreateLineSeries(seriesData),
                ChartSeriesType.Scatter => CreateScatterSeries(seriesData),
                _ => throw new NotSupportedException($"Series type {seriesData.SeriesType} is not supported")
            };

            series.Add(chartSeries);
        }

        return series;
    }

    /// <summary>
    /// Create LiveCharts2 LineSeries from ChartSeriesData
    /// </summary>
    private LineSeries<ChartDataPoint> CreateLineSeries(ChartSeriesData __seriesData)
    {
        var lineSeries = new LineSeries<ChartDataPoint>
        {
            Name = __seriesData.Name,
            Values = __seriesData.DataPoints,
            Mapping = (__point, _) => new(__point.X, __point.Y),
            LineSmoothness = 0, // 0 = straight lines, 1 = smooth curves
            GeometrySize = __seriesData.ShowDataPoints ? __seriesData.PointSize : 0,
            Fill = __seriesData.Fill ? CreateSolidColorPaint(__seriesData.Color, 50) : null,
            Stroke = CreateSolidColorPaint(__seriesData.Color, 255),
            GeometryStroke = CreateSolidColorPaint(__seriesData.Color, 255),
        };

        if (lineSeries.Stroke != null)
        {
            lineSeries.Stroke.StrokeThickness = (float)__seriesData.StrokeThickness;
        }

        return lineSeries;
    }

    /// <summary>
    /// Create LiveCharts2 ScatterSeries from ChartSeriesData
    /// </summary>
    private ScatterSeries<ChartDataPoint> CreateScatterSeries(ChartSeriesData __seriesData)
    {
        var scatterSeries = new ScatterSeries<ChartDataPoint>
        {
            Name = __seriesData.Name,
            Values = __seriesData.DataPoints,
            Mapping = (__point, _) => new(__point.X, __point.Y),
            GeometrySize = __seriesData.PointSize,
            Fill = CreateSolidColorPaint(__seriesData.Color, 200),
            Stroke = CreateSolidColorPaint(__seriesData.Color, 255),
        };

        if (scatterSeries.Stroke != null)
        {
            scatterSeries.Stroke.StrokeThickness = 1;
        }

        return scatterSeries;
    }

    /// <summary>
    /// Create LiveCharts2 X-axis from AxisConfig
    /// </summary>
    public Axis CreateXAxis()
    {
        return new Axis
        {
            Name = XAxisConfig.Title,
            MinLimit = XAxisConfig.MinLimit,
            MaxLimit = XAxisConfig.MaxLimit,
            MinStep = XAxisConfig.Step ?? 1,
            Labeler = __value => __value.ToString(XAxisConfig.LabelFormat),
            ShowSeparatorLines = XAxisConfig.ShowSeparatorLines,
            IsVisible = XAxisConfig.ShowLabels
        };
    }

    /// <summary>
    /// Create LiveCharts2 Y-axis from AxisConfig
    /// </summary>
    public Axis CreateYAxis()
    {
        return new Axis
        {
            Name = YAxisConfig.Title,
            MinLimit = YAxisConfig.MinLimit,
            MaxLimit = YAxisConfig.MaxLimit,
            MinStep = YAxisConfig.Step ?? 1,
            Labeler = __value => __value.ToString(YAxisConfig.LabelFormat),
            ShowSeparatorLines = YAxisConfig.ShowSeparatorLines,
            IsVisible = YAxisConfig.ShowLabels
        };
    }

    /// <summary>
    /// Helper method to create a solid color paint for LiveCharts2
    /// </summary>
    private SolidColorPaint? CreateSolidColorPaint(string? __hexColor, byte __alpha)
    {
        if (string.IsNullOrEmpty(__hexColor))
            return null;

        try
        {
            var color = SKColor.Parse(__hexColor);
            return new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, __alpha));
        }
        catch
        {
            // Return default color if parsing fails
            return new SolidColorPaint(new SKColor(33, 150, 243, __alpha)); // Default blue
        }
    }

    /// <summary>
    /// Get X-axis collection for LiveCharts2 binding
    /// </summary>
    public ObservableCollection<Axis> GetXAxes()
    {
        return new ObservableCollection<Axis> { CreateXAxis() };
    }

    /// <summary>
    /// Get Y-axis collection for LiveCharts2 binding
    /// </summary>
    public ObservableCollection<Axis> GetYAxes()
    {
        return new ObservableCollection<Axis> { CreateYAxis() };
    }
}
