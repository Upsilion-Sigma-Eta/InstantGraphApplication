using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using IGA_GUI_Module.Model.Data;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// 3D 차트 요소 생성 팩토리
/// HelixToolkit.Wpf를 사용하여 3D 시각화 요소 생성
/// </summary>
public static class Chart3DFactory
{
    /// <summary>
    /// 기본 색상 팔레트 (3D 차트용)
    /// </summary>
    public static readonly Color[] DefaultColorPalette = new Color[]
    {
        Colors.DodgerBlue,
        Colors.OrangeRed,
        Colors.LimeGreen,
        Colors.Purple,
        Colors.Gold,
        Colors.Crimson,
        Colors.Teal,
        Colors.HotPink
    };

    /// <summary>
    /// 색상 팔레트에서 색상 가져오기
    /// </summary>
    public static Color GetColor(int __index)
    {
        return DefaultColorPalette[__index % DefaultColorPalette.Length];
    }

    #region 3D Scatter Plot

    /// <summary>
    /// 3D 산점도 생성
    /// </summary>
    public static Model3DGroup CreateScatter3D(
        IEnumerable<ChartDataPoint3D> __points,
        double __pointSize = 0.5,
        bool __colorByZ = true)
    {
        var group = new Model3DGroup();
        var pointList = __points.ToList();

        if (pointList.Count == 0)
            return group;

        double zMin = pointList.Min(__p => __p.Z);
        double zMax = pointList.Max(__p => __p.Z);
        double zRange = zMax - zMin;
        if (zRange < 1e-10) zRange = 1;

        int index = 0;
        foreach (var point in pointList)
        {
            Color color;
            if (__colorByZ)
            {
                double t = (point.Z - zMin) / zRange;
                color = GetColorFromGradient(t);
            }
            else
            {
                color = GetColor(index);
            }

            var sphere = CreateSphere(point.ToPoint3D(), __pointSize, color);
            group.Children.Add(sphere);
            index++;
        }

        return group;
    }

    /// <summary>
    /// 구체 모델 생성
    /// </summary>
    private static GeometryModel3D CreateSphere(Point3D __center, double __radius, Color __color)
    {
        var builder = new MeshBuilder(false, false);
        builder.AddSphere(__center, __radius, 16, 8);

        var material = new DiffuseMaterial(new SolidColorBrush(__color));

        return new GeometryModel3D
        {
            Geometry = builder.ToMesh(),
            Material = material,
            BackMaterial = material
        };
    }

    #endregion

    #region 3D Surface Plot

    /// <summary>
    /// 3D 표면 그래프 생성
    /// </summary>
    public static Model3DGroup CreateSurface3D(SurfaceData __surface, bool __showWireframe = false)
    {
        var group = new Model3DGroup();

        if (__surface.XCount < 2 || __surface.YCount < 2)
            return group;

        // 메쉬 생성
        var mesh = CreateSurfaceMesh(__surface);
        var material = CreateGradientMaterial(__surface);

        var surfaceModel = new GeometryModel3D
        {
            Geometry = mesh,
            Material = material,
            BackMaterial = material
        };

        group.Children.Add(surfaceModel);

        // 와이어프레임 추가
        if (__showWireframe)
        {
            var wireframe = CreateWireframe(__surface);
            group.Children.Add(wireframe);
        }

        return group;
    }

    /// <summary>
    /// 표면 메쉬 생성
    /// </summary>
    private static MeshGeometry3D CreateSurfaceMesh(SurfaceData __surface)
    {
        var mesh = new MeshGeometry3D();
        var positions = new Point3DCollection();
        var indices = new Int32Collection();
        var texCoords = new PointCollection();

        double zMin = __surface.ZMin;
        double zMax = __surface.ZMax;
        double zRange = zMax - zMin;
        if (zRange < 1e-10) zRange = 1;

        // 정점 생성
        for (int i = 0; i < __surface.XCount; i++)
        {
            for (int j = 0; j < __surface.YCount; j++)
            {
                var pt = __surface.GetPoint(i, j);
                positions.Add(pt);

                // 텍스처 좌표 (Z값 기반 색상용)
                double t = (pt.Z - zMin) / zRange;
                texCoords.Add(new Point(t, 0));
            }
        }

        // 삼각형 인덱스 생성
        for (int i = 0; i < __surface.XCount - 1; i++)
        {
            for (int j = 0; j < __surface.YCount - 1; j++)
            {
                int idx = i * __surface.YCount + j;

                // 첫 번째 삼각형
                indices.Add(idx);
                indices.Add(idx + __surface.YCount);
                indices.Add(idx + 1);

                // 두 번째 삼각형
                indices.Add(idx + 1);
                indices.Add(idx + __surface.YCount);
                indices.Add(idx + __surface.YCount + 1);
            }
        }

        mesh.Positions = positions;
        mesh.TriangleIndices = indices;
        mesh.TextureCoordinates = texCoords;

        return mesh;
    }

    /// <summary>
    /// 그라데이션 Material 생성
    /// </summary>
    private static Material CreateGradientMaterial(SurfaceData __surface)
    {
        // 파랑 → 초록 → 노랑 → 빨강 그라데이션
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5)
        };

        brush.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
        brush.GradientStops.Add(new GradientStop(Colors.Cyan, 0.25));
        brush.GradientStops.Add(new GradientStop(Colors.LimeGreen, 0.5));
        brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.75));
        brush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));

        return new DiffuseMaterial(brush);
    }

    /// <summary>
    /// 와이어프레임 생성
    /// </summary>
    private static Model3D CreateWireframe(SurfaceData __surface)
    {
        var builder = new MeshBuilder(false, false);

        double lineRadius = Math.Min(__surface.XMax - __surface.XMin, __surface.YMax - __surface.YMin) * 0.002;
        if (lineRadius < 0.01) lineRadius = 0.01;

        // X 방향 선
        for (int i = 0; i < __surface.XCount; i++)
        {
            for (int j = 0; j < __surface.YCount - 1; j++)
            {
                var p1 = __surface.GetPoint(i, j);
                var p2 = __surface.GetPoint(i, j + 1);
                builder.AddPipe(p1, p2, lineRadius * 0.5, lineRadius * 0.5, 4);
            }
        }

        // Y 방향 선
        for (int j = 0; j < __surface.YCount; j++)
        {
            for (int i = 0; i < __surface.XCount - 1; i++)
            {
                var p1 = __surface.GetPoint(i, j);
                var p2 = __surface.GetPoint(i + 1, j);
                builder.AddPipe(p1, p2, lineRadius * 0.5, lineRadius * 0.5, 4);
            }
        }

        var material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)));

        return new GeometryModel3D
        {
            Geometry = builder.ToMesh(),
            Material = material
        };
    }

    #endregion

    #region 3D Axes

    /// <summary>
    /// 3D 축 생성
    /// </summary>
    public static Model3DGroup CreateAxes3D(
        double __xMin, double __xMax,
        double __yMin, double __yMax,
        double __zMin, double __zMax,
        string __xLabel = "X",
        string __yLabel = "Y",
        string __zLabel = "Z")
    {
        var group = new Model3DGroup();

        double axisRadius = Math.Max(
            Math.Max(__xMax - __xMin, __yMax - __yMin),
            __zMax - __zMin) * 0.01;

        if (axisRadius < 0.01) axisRadius = 0.01;

        // X축 (빨강)
        group.Children.Add(CreateAxisLine(
            new Point3D(__xMin, __yMin, __zMin),
            new Point3D(__xMax, __yMin, __zMin),
            Colors.Red, axisRadius));

        // Y축 (초록)
        group.Children.Add(CreateAxisLine(
            new Point3D(__xMin, __yMin, __zMin),
            new Point3D(__xMin, __yMax, __zMin),
            Colors.Green, axisRadius));

        // Z축 (파랑)
        group.Children.Add(CreateAxisLine(
            new Point3D(__xMin, __yMin, __zMin),
            new Point3D(__xMin, __yMin, __zMax),
            Colors.Blue, axisRadius));

        return group;
    }

    /// <summary>
    /// 축 선 생성
    /// </summary>
    private static GeometryModel3D CreateAxisLine(Point3D __from, Point3D __to, Color __color, double __radius)
    {
        var builder = new MeshBuilder(false, false);
        builder.AddPipe(__from, __to, __radius, __radius, 8);

        // 화살표 헤드
        var direction = __to - __from;
        direction.Normalize();
        var arrowBase = __to - direction * __radius * 5;
        builder.AddCone(arrowBase, direction, __radius * 3, __radius * 3, 0, true, true, 12);

        var material = new DiffuseMaterial(new SolidColorBrush(__color));

        return new GeometryModel3D
        {
            Geometry = builder.ToMesh(),
            Material = material,
            BackMaterial = material
        };
    }

    /// <summary>
    /// 그리드 바닥면 생성
    /// </summary>
    public static Model3D CreateGridFloor(
        double __xMin, double __xMax,
        double __yMin, double __yMax,
        double __z,
        int __divisions = 10)
    {
        var builder = new MeshBuilder(false, false);

        double lineRadius = Math.Max(__xMax - __xMin, __yMax - __yMin) * 0.002;
        if (lineRadius < 0.005) lineRadius = 0.005;

        double xStep = (__xMax - __xMin) / __divisions;
        double yStep = (__yMax - __yMin) / __divisions;

        // X 방향 그리드 선
        for (int i = 0; i <= __divisions; i++)
        {
            double x = __xMin + i * xStep;
            builder.AddPipe(
                new Point3D(x, __yMin, __z),
                new Point3D(x, __yMax, __z),
                lineRadius, lineRadius, 4);
        }

        // Y 방향 그리드 선
        for (int i = 0; i <= __divisions; i++)
        {
            double y = __yMin + i * yStep;
            builder.AddPipe(
                new Point3D(__xMin, y, __z),
                new Point3D(__xMax, y, __z),
                lineRadius, lineRadius, 4);
        }

        var material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(64, 128, 128, 128)));

        return new GeometryModel3D
        {
            Geometry = builder.ToMesh(),
            Material = material
        };
    }

    #endregion

    #region Color Utilities

    /// <summary>
    /// 그라데이션에서 색상 가져오기 (0~1 범위)
    /// </summary>
    public static Color GetColorFromGradient(double __t)
    {
        __t = Math.Clamp(__t, 0, 1);

        // 파랑 → 초록 → 노랑 → 빨강
        if (__t < 0.25)
        {
            return InterpolateColor(Colors.Blue, Colors.Cyan, __t * 4);
        }
        else if (__t < 0.5)
        {
            return InterpolateColor(Colors.Cyan, Colors.LimeGreen, (__t - 0.25) * 4);
        }
        else if (__t < 0.75)
        {
            return InterpolateColor(Colors.LimeGreen, Colors.Yellow, (__t - 0.5) * 4);
        }
        else
        {
            return InterpolateColor(Colors.Yellow, Colors.Red, (__t - 0.75) * 4);
        }
    }

    /// <summary>
    /// 두 색상 보간
    /// </summary>
    private static Color InterpolateColor(Color __c1, Color __c2, double __t)
    {
        __t = Math.Clamp(__t, 0, 1);
        return Color.FromArgb(
            255,
            (byte)(__c1.R + (__c2.R - __c1.R) * __t),
            (byte)(__c1.G + (__c2.G - __c1.G) * __t),
            (byte)(__c1.B + (__c2.B - __c1.B) * __t));
    }

    #endregion

    #region Bounding Box

    /// <summary>
    /// 데이터 경계 박스 계산
    /// </summary>
    public static Rect3D CalculateBounds(IEnumerable<ChartDataPoint3D> __points)
    {
        var pointList = __points.ToList();
        if (pointList.Count == 0)
            return new Rect3D(0, 0, 0, 1, 1, 1);

        double xMin = pointList.Min(__p => __p.X);
        double xMax = pointList.Max(__p => __p.X);
        double yMin = pointList.Min(__p => __p.Y);
        double yMax = pointList.Max(__p => __p.Y);
        double zMin = pointList.Min(__p => __p.Z);
        double zMax = pointList.Max(__p => __p.Z);

        return new Rect3D(xMin, yMin, zMin, xMax - xMin, yMax - yMin, zMax - zMin);
    }

    /// <summary>
    /// SurfaceData 경계 박스 계산
    /// </summary>
    public static Rect3D CalculateBounds(SurfaceData __surface)
    {
        return new Rect3D(
            __surface.XMin, __surface.YMin, __surface.ZMin,
            __surface.XMax - __surface.XMin,
            __surface.YMax - __surface.YMin,
            __surface.ZMax - __surface.ZMin);
    }

    #endregion
}
