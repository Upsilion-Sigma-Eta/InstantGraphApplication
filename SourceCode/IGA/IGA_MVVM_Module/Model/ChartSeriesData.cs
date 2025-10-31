using System.Collections.ObjectModel;

namespace IGA_GUI_Module.Model;

/// <summary>
/// Represents chart series type
/// </summary>
public enum ChartSeriesType
{
    Line,
    Scatter
}

/// <summary>
/// Represents a single data point in a chart
/// </summary>
public class ChartDataPoint
{
    public double X { get; set; }
    public double Y { get; set; }

    public ChartDataPoint(double  __x, double __y)
    {
        X = __x;
        Y = __y;
    }
}

/// <summary>
/// Configuration and data for a chart series
/// </summary>
public class ChartSeriesData
{
    /// <summary>
    /// Series name displayed in legend
    /// </summary>
    public string Name { get; set; } = "Series";

    /// <summary>
    /// Type of chart series (Line or Scatter)
    /// </summary>
    public ChartSeriesType SeriesType { get; set; } = ChartSeriesType.Line;

    /// <summary>
    /// Data points for this series
    /// </summary>
    public ObservableCollection<ChartDataPoint> DataPoints { get; set; } = new();

    /// <summary>
    /// Series color in hex format (e.g., "#FF5733")
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Line stroke thickness (for Line series)
    /// </summary>
    public double StrokeThickness { get; set; } = 2.0;

    /// <summary>
    /// Point/marker size (for Scatter series or Line series points)
    /// </summary>
    public double PointSize { get; set; } = 8.0;

    /// <summary>
    /// Whether to show data points on line chart
    /// </summary>
    public bool ShowDataPoints { get; set; } = true;

    /// <summary>
    /// Whether to fill area under line
    /// </summary>
    public bool Fill { get; set; } = false;

    /// <summary>
    /// Add a data point to the series
    /// </summary>
    public void AddDataPoint(double __x, double __y)
    {
        DataPoints.Add(new ChartDataPoint(__x, __y));
    }

    /// <summary>
    /// Add multiple data points from arrays
    /// </summary>
    public void AddDataPoints(double[] __xValues, double[] __yValues)
    {
        if (__xValues.Length != __yValues.Length)
            throw new ArgumentException("X and Y arrays must have the same length");

        for (int i = 0; i < __xValues.Length; i++)
        {
            DataPoints.Add(new ChartDataPoint(__xValues[i], __yValues[i]));
        }
    }

    /// <summary>
    /// Clear all data points
    /// </summary>
    public void ClearDataPoints()
    {
        DataPoints.Clear();
    }
}
