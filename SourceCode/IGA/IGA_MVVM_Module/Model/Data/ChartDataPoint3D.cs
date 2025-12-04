using System.Windows.Media.Media3D;

namespace IGA_GUI_Module.Model.Data;

/// <summary>
/// 3D 차트의 단일 데이터 포인트를 나타내는 클래스
/// </summary>
public class ChartDataPoint3D
{
    /// <summary>
    /// X 좌표 값
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y 좌표 값
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Z 좌표 값
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    /// 데이터 포인트 레이블 (선택적)
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// 기본 생성자
    /// </summary>
    public ChartDataPoint3D()
    {
    }

    /// <summary>
    /// 좌표값으로 초기화하는 생성자
    /// </summary>
    public ChartDataPoint3D(double __x, double __y, double __z)
    {
        X = __x;
        Y = __y;
        Z = __z;
    }

    /// <summary>
    /// 좌표값과 레이블로 초기화하는 생성자
    /// </summary>
    public ChartDataPoint3D(double __x, double __y, double __z, string __label)
    {
        X = __x;
        Y = __y;
        Z = __z;
        Label = __label;
    }

    /// <summary>
    /// Point3D로 변환
    /// </summary>
    public Point3D ToPoint3D()
    {
        return new Point3D(X, Y, Z);
    }

    /// <summary>
    /// Point3D에서 생성
    /// </summary>
    public static ChartDataPoint3D FromPoint3D(Point3D __point)
    {
        return new ChartDataPoint3D(__point.X, __point.Y, __point.Z);
    }

    public override string ToString()
    {
        return $"({X:F2}, {Y:F2}, {Z:F2})";
    }
}
