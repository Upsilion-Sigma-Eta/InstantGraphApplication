namespace IGA_GUI_Module.Model;

/// <summary>
/// Axis configuration for chart axes (X or Y)
/// </summary>
public class AxisConfig
{
    /// <summary>
    /// Axis title/label
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Minimum value for the axis (null for auto)
    /// </summary>
    public double? MinLimit { get; set; }

    /// <summary>
    /// Maximum value for the axis (null for auto)
    /// </summary>
    public double? MaxLimit { get; set; }

    /// <summary>
    /// Format string for axis labels (e.g., "0.00", "N2")
    /// </summary>
    public string LabelFormat { get; set; } = "0.##";

    /// <summary>
    /// Whether to show axis labels
    /// </summary>
    public bool ShowLabels { get; set; } = true;

    /// <summary>
    /// Whether to show separator lines
    /// </summary>
    public bool ShowSeparatorLines { get; set; } = true;

    /// <summary>
    /// Step size for axis values (null for auto)
    /// </summary>
    public double? Step { get; set; }
}
