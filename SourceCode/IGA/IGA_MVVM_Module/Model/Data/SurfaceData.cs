using System.Windows.Media.Media3D;

namespace IGA_GUI_Module.Model.Data;

/// <summary>
/// 3D Surface Plot용 그리드 데이터 모델
/// X-Y 평면의 그리드에서 각 점의 Z 높이 값을 저장
/// </summary>
public class SurfaceData
{
    /// <summary>
    /// Z 값 그리드 배열 [xIndex, yIndex]
    /// </summary>
    public double[,] Values { get; set; }

    /// <summary>
    /// X축 최소값
    /// </summary>
    public double XMin { get; set; }

    /// <summary>
    /// X축 최대값
    /// </summary>
    public double XMax { get; set; }

    /// <summary>
    /// Y축 최소값
    /// </summary>
    public double YMin { get; set; }

    /// <summary>
    /// Y축 최대값
    /// </summary>
    public double YMax { get; set; }

    /// <summary>
    /// Z축 최소값 (자동 계산됨)
    /// </summary>
    public double ZMin { get; private set; }

    /// <summary>
    /// Z축 최대값 (자동 계산됨)
    /// </summary>
    public double ZMax { get; private set; }

    /// <summary>
    /// X 방향 그리드 크기
    /// </summary>
    public int XCount => Values?.GetLength(0) ?? 0;

    /// <summary>
    /// Y 방향 그리드 크기
    /// </summary>
    public int YCount => Values?.GetLength(1) ?? 0;

    /// <summary>
    /// 기본 생성자
    /// </summary>
    public SurfaceData()
    {
        Values = new double[0, 0];
    }

    /// <summary>
    /// 그리드 크기와 범위로 초기화하는 생성자
    /// </summary>
    public SurfaceData(int __xCount, int __yCount, double __xMin, double __xMax, double __yMin, double __yMax)
    {
        Values = new double[__xCount, __yCount];
        XMin = __xMin;
        XMax = __xMax;
        YMin = __yMin;
        YMax = __yMax;
    }

    /// <summary>
    /// 기존 2D 배열에서 생성하는 생성자
    /// </summary>
    public SurfaceData(double[,] __values, double __xMin, double __xMax, double __yMin, double __yMax)
    {
        Values = __values;
        XMin = __xMin;
        XMax = __xMax;
        YMin = __yMin;
        YMax = __yMax;
        CalculateZRange();
    }

    /// <summary>
    /// Z 범위 계산
    /// </summary>
    public void CalculateZRange()
    {
        if (Values == null || XCount == 0 || YCount == 0)
        {
            ZMin = 0;
            ZMax = 0;
            return;
        }

        ZMin = double.MaxValue;
        ZMax = double.MinValue;

        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                double v = Values[i, j];
                if (!double.IsNaN(v) && !double.IsInfinity(v))
                {
                    if (v < ZMin) ZMin = v;
                    if (v > ZMax) ZMax = v;
                }
            }
        }

        if (ZMin == double.MaxValue) ZMin = 0;
        if (ZMax == double.MinValue) ZMax = 0;
    }

    /// <summary>
    /// 인덱스에 해당하는 실제 X 좌표 반환
    /// </summary>
    public double GetX(int __xIndex)
    {
        if (XCount <= 1) return XMin;
        return XMin + (XMax - XMin) * __xIndex / (XCount - 1);
    }

    /// <summary>
    /// 인덱스에 해당하는 실제 Y 좌표 반환
    /// </summary>
    public double GetY(int __yIndex)
    {
        if (YCount <= 1) return YMin;
        return YMin + (YMax - YMin) * __yIndex / (YCount - 1);
    }

    /// <summary>
    /// 특정 위치의 Point3D 반환
    /// </summary>
    public Point3D GetPoint(int __xIndex, int __yIndex)
    {
        return new Point3D(GetX(__xIndex), GetY(__yIndex), Values[__xIndex, __yIndex]);
    }

    /// <summary>
    /// 산점 데이터로부터 Surface 생성 (그리드 보간)
    /// </summary>
    public static SurfaceData FromScatterPoints(IEnumerable<ChartDataPoint3D> __points, int __gridResolution = 50)
    {
        var pointList = __points.ToList();
        if (pointList.Count == 0)
            return new SurfaceData();

        double xMin = pointList.Min(__p => __p.X);
        double xMax = pointList.Max(__p => __p.X);
        double yMin = pointList.Min(__p => __p.Y);
        double yMax = pointList.Max(__p => __p.Y);

        // 범위가 0인 경우 처리
        if (Math.Abs(xMax - xMin) < 1e-10) { xMin -= 1; xMax += 1; }
        if (Math.Abs(yMax - yMin) < 1e-10) { yMin -= 1; yMax += 1; }

        var surface = new SurfaceData(__gridResolution, __gridResolution, xMin, xMax, yMin, yMax);

        // 역거리 가중 보간 (IDW)
        for (int i = 0; i < __gridResolution; i++)
        {
            for (int j = 0; j < __gridResolution; j++)
            {
                double x = surface.GetX(i);
                double y = surface.GetY(j);

                double sumWeights = 0;
                double sumValues = 0;

                foreach (var pt in pointList)
                {
                    double dx = x - pt.X;
                    double dy = y - pt.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist < 1e-10)
                    {
                        // 정확히 그 점 위에 있는 경우
                        sumWeights = 1;
                        sumValues = pt.Z;
                        break;
                    }

                    double weight = 1.0 / (dist * dist);
                    sumWeights += weight;
                    sumValues += weight * pt.Z;
                }

                surface.Values[i, j] = sumWeights > 0 ? sumValues / sumWeights : 0;
            }
        }

        surface.CalculateZRange();
        return surface;
    }
}
