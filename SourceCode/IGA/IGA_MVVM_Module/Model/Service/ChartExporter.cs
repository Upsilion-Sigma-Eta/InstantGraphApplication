using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// 차트를 이미지 파일로 내보내는 서비스
/// WPF RenderTargetBitmap을 사용하여 UI 요소를 이미지로 변환
/// </summary>
public static class ChartExporter
{
    /// <summary>
    /// 지원하는 이미지 형식
    /// </summary>
    public enum ImageFormat
    {
        Png,
        Jpeg,
        Bmp
    }

    /// <summary>
    /// UI 요소를 이미지 파일로 저장
    /// </summary>
    /// <param name="__element">저장할 UI 요소</param>
    /// <param name="__filePath">저장 경로</param>
    /// <param name="__format">이미지 형식</param>
    /// <param name="__dpi">DPI (기본값: 96)</param>
    /// <returns>저장 성공 여부</returns>
    public static bool SaveToFile(FrameworkElement __element, string __filePath, ImageFormat __format = ImageFormat.Png, double __dpi = 96)
    {
        if (__element == null || string.IsNullOrEmpty(__filePath))
            return false;

        try
        {
            var bitmap = RenderToBitmap(__element, __dpi);
            if (bitmap == null)
                return false;

            BitmapEncoder encoder = __format switch
            {
                ImageFormat.Jpeg => new JpegBitmapEncoder { QualityLevel = 95 },
                ImageFormat.Bmp => new BmpBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using var fileStream = new FileStream(__filePath, FileMode.Create);
            encoder.Save(fileStream);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// UI 요소를 RenderTargetBitmap으로 렌더링
    /// </summary>
    /// <param name="__element">렌더링할 UI 요소</param>
    /// <param name="__dpi">DPI</param>
    /// <returns>렌더링된 비트맵</returns>
    private static RenderTargetBitmap? RenderToBitmap(FrameworkElement __element, double __dpi)
    {
        if (__element.ActualWidth <= 0 || __element.ActualHeight <= 0)
            return null;

        double scale = __dpi / 96.0;
        int width = (int)(__element.ActualWidth * scale);
        int height = (int)(__element.ActualHeight * scale);

        var renderBitmap = new RenderTargetBitmap(
            width,
            height,
            __dpi,
            __dpi,
            PixelFormats.Pbgra32);

        var drawingVisual = new DrawingVisual();
        using (var context = drawingVisual.RenderOpen())
        {
            var brush = new VisualBrush(__element)
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };

            context.DrawRectangle(
                brush,
                null,
                new Rect(0, 0, __element.ActualWidth * scale, __element.ActualHeight * scale));
        }

        renderBitmap.Render(drawingVisual);
        return renderBitmap;
    }

    /// <summary>
    /// 파일 확장자로 이미지 형식 결정
    /// </summary>
    /// <param name="__filePath">파일 경로</param>
    /// <returns>이미지 형식</returns>
    public static ImageFormat GetFormatFromExtension(string __filePath)
    {
        string extension = Path.GetExtension(__filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".bmp" => ImageFormat.Bmp,
            _ => ImageFormat.Png
        };
    }

    /// <summary>
    /// SaveFileDialog용 필터 문자열
    /// </summary>
    public static string FileFilter => "PNG 이미지 (*.png)|*.png|JPEG 이미지 (*.jpg)|*.jpg|BMP 이미지 (*.bmp)|*.bmp";
}
