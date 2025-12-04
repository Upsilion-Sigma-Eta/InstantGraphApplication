namespace IGA_GUI_Module.Model.Data;

/// <summary>
/// 차트의 단일 데이터 포인트를 나타내는 클래스
/// </summary>
public class ChartDataPoint
{
    /// <summary>
    /// X 좌표 값
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y 좌표 값
    /// </summary>
    public double Y { get; set; }

    public ChartDataPoint(double __x, double __y)
    {
        X = __x;
        Y = __y;
    }
}
