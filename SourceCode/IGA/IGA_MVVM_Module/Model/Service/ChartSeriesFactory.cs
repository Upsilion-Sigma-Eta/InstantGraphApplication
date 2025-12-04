using System.Collections.ObjectModel;
using IGA_GUI_Module.Model.Data;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// 차트 시리즈 생성 팩토리
/// 차트 유형에 따라 적절한 LiveCharts2 시리즈 생성
/// </summary>
public static class ChartSeriesFactory
{
    /// <summary>
    /// 기본 차트 색상 팔레트
    /// </summary>
    public static readonly SKColor[] DefaultColorPalette = new SKColor[]
    {
        SKColors.DodgerBlue,
        SKColors.OrangeRed,
        SKColors.LimeGreen,
        SKColors.Purple,
        SKColors.Gold,
        SKColors.Crimson,
        SKColors.Teal,
        SKColors.HotPink
    };

    /// <summary>
    /// 색상 팔레트에서 색상 가져오기
    /// </summary>
    public static SKColor GetColor(int __index)
    {
        return DefaultColorPalette[__index % DefaultColorPalette.Length];
    }

    /// <summary>
    /// 차트 유형에 따른 시리즈 생성
    /// </summary>
    public static ISeries CreateSeries(
        ChartType __chartType,
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return __chartType switch
        {
            ChartType.Line => CreateLineSeries(__name, __dataPoints, __color),
            ChartType.Column => CreateColumnSeries(__name, __dataPoints, __color),
            ChartType.Bar => CreateBarSeries(__name, __dataPoints, __color),
            ChartType.Area => CreateAreaSeries(__name, __dataPoints, __color),
            ChartType.Scatter => CreateScatterSeries(__name, __dataPoints, __color),
            ChartType.StepLine => CreateStepLineSeries(__name, __dataPoints, __color),
            _ => CreateLineSeries(__name, __dataPoints, __color)
        };
    }

    /// <summary>
    /// 꺾은선 시리즈 생성
    /// </summary>
    public static LineSeries<ChartDataPoint> CreateLineSeries(
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return new LineSeries<ChartDataPoint>
        {
            Name = __name,
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            LineSmoothness = 0,
            GeometrySize = 6,
            Stroke = new SolidColorPaint(__color, 2),
            GeometryStroke = new SolidColorPaint(__color, 2),
            GeometryFill = new SolidColorPaint(__color),
            Fill = null
        };
    }

    /// <summary>
    /// 세로 막대 시리즈 생성
    /// </summary>
    public static ColumnSeries<ChartDataPoint> CreateColumnSeries(
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return new ColumnSeries<ChartDataPoint>
        {
            Name = __name,
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            Stroke = new SolidColorPaint(__color, 1),
            Fill = new SolidColorPaint(__color.WithAlpha(200)),
            MaxBarWidth = 30
        };
    }

    /// <summary>
    /// 가로 막대 시리즈 생성
    /// </summary>
    public static RowSeries<ChartDataPoint> CreateBarSeries(
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return new RowSeries<ChartDataPoint>
        {
            Name = __name,
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            Stroke = new SolidColorPaint(__color, 1),
            Fill = new SolidColorPaint(__color.WithAlpha(200)),
            MaxBarWidth = 20
        };
    }

    /// <summary>
    /// 영역 시리즈 생성
    /// </summary>
    public static LineSeries<ChartDataPoint> CreateAreaSeries(
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return new LineSeries<ChartDataPoint>
        {
            Name = __name,
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            LineSmoothness = 0.3,
            GeometrySize = 4,
            Stroke = new SolidColorPaint(__color, 2),
            GeometryStroke = new SolidColorPaint(__color, 2),
            GeometryFill = new SolidColorPaint(__color),
            Fill = new SolidColorPaint(__color.WithAlpha(80))
        };
    }

    /// <summary>
    /// 산점도 시리즈 생성
    /// </summary>
    public static ScatterSeries<ChartDataPoint> CreateScatterSeries(
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return new ScatterSeries<ChartDataPoint>
        {
            Name = __name,
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            GeometrySize = 10,
            Stroke = new SolidColorPaint(__color, 2),
            Fill = new SolidColorPaint(__color.WithAlpha(200))
        };
    }

    /// <summary>
    /// 계단형 시리즈 생성
    /// </summary>
    public static StepLineSeries<ChartDataPoint> CreateStepLineSeries(
        string __name,
        ObservableCollection<ChartDataPoint> __dataPoints,
        SKColor __color)
    {
        return new StepLineSeries<ChartDataPoint>
        {
            Name = __name,
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            GeometrySize = 6,
            Stroke = new SolidColorPaint(__color, 2),
            GeometryStroke = new SolidColorPaint(__color, 2),
            GeometryFill = new SolidColorPaint(__color),
            Fill = null
        };
    }

    /// <summary>
    /// 파이 시리즈 생성
    /// </summary>
    public static PieSeries<double> CreatePieSeries(
        string __label,
        double __value,
        SKColor __color)
    {
        return new PieSeries<double>
        {
            Name = __label,
            Values = new[] { Math.Abs(__value) },
            Fill = new SolidColorPaint(__color),
            Stroke = new SolidColorPaint(SKColors.White, 2),
            Pushout = 0,
            DataLabelsSize = 12,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            DataLabelsFormatter = _ => $"{__label}: {__value:0.##}"
        };
    }

    /// <summary>
    /// 회귀선 시리즈 생성
    /// </summary>
    public static LineSeries<ChartDataPoint> CreateRegressionLineSeries(
        ObservableCollection<ChartDataPoint> __dataPoints)
    {
        return new LineSeries<ChartDataPoint>
        {
            Name = "회귀선",
            Values = __dataPoints,
            Mapping = (__point, _) => new((__point as ChartDataPoint)!.X, (__point as ChartDataPoint)!.Y),
            LineSmoothness = 0,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(SKColors.Red, 2) { StrokeCap = SKStrokeCap.Round },
            Fill = null
        };
    }

    /// <summary>
    /// 마커 시리즈 생성 (최대/최소값 표시용)
    /// </summary>
    public static ScatterSeries<ChartDataPoint> CreateMarkerSeries(
        ChartDataPoint __point,
        string __name,
        SKColor __color,
        bool __isTop = true)
    {
        return new ScatterSeries<ChartDataPoint>
        {
            Name = __name,
            Values = new[] { __point },
            Mapping = (__p, _) => new((__p as ChartDataPoint)!.X, (__p as ChartDataPoint)!.Y),
            GeometrySize = 15,
            Stroke = new SolidColorPaint(__color, 3),
            Fill = new SolidColorPaint(__color.WithAlpha(180)),
            DataLabelsSize = 12,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsPosition = __isTop
                ? LiveChartsCore.Measure.DataLabelsPosition.Top
                : LiveChartsCore.Measure.DataLabelsPosition.Bottom,
            DataLabelsFormatter = _ => $"({__point.X:0.##}, {__point.Y:0.##})"
        };
    }
}
