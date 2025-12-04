using System.IO;
using ClosedXML.Excel;
using IGA_GUI_Module.Model.Data;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// Excel (.xlsx, .xls, .xlsm) 파일 처리 서비스
/// - Excel 파일을 읽어와서 CsvDataModel로 변환
/// </summary>
public static class ExcelHandler
{
    /// <summary>
    /// Excel 파일을 읽어서 CsvDataModel로 반환
    /// </summary>
    /// <param name="__filePath">Excel 파일 경로</param>
    /// <param name="__sheetIndex">읽을 시트 인덱스 (0부터 시작, 기본값 0)</param>
    /// <returns>파싱된 CSV 데이터 모델</returns>
    /// <exception cref="FileNotFoundException">파일이 존재하지 않을 경우</exception>
    /// <exception cref="InvalidDataException">Excel 파일이 비어있거나 유효하지 않은 경우</exception>
    public static CsvDataModel LoadFromFile(string __filePath, int __sheetIndex = 0)
    {
        if (!File.Exists(__filePath))
        {
            throw new FileNotFoundException("Excel 파일을 찾을 수 없습니다.", __filePath);
        }

        var csvData = new CsvDataModel
        {
            FilePath = __filePath,
            FileName = Path.GetFileNameWithoutExtension(__filePath)
        };

        using var workbook = new XLWorkbook(__filePath);

        if (workbook.Worksheets.Count == 0)
        {
            throw new InvalidDataException("Excel 파일에 시트가 없습니다.");
        }

        // 지정된 인덱스의 시트 가져오기 (인덱스가 범위를 벗어나면 첫 번째 시트 사용)
        var worksheet = __sheetIndex < workbook.Worksheets.Count
            ? workbook.Worksheets.ElementAt(__sheetIndex)
            : workbook.Worksheets.First();

        var usedRange = worksheet.RangeUsed();
        if (usedRange == null)
        {
            throw new InvalidDataException("Excel 시트에 데이터가 없습니다.");
        }

        int firstRow = usedRange.FirstRow().RowNumber();
        int lastRow = usedRange.LastRow().RowNumber();
        int firstColumn = usedRange.FirstColumn().ColumnNumber();
        int lastColumn = usedRange.LastColumn().ColumnNumber();

        // 첫 번째 행을 헤더로 파싱
        for (int col = firstColumn; col <= lastColumn; col++)
        {
            var cell = worksheet.Cell(firstRow, col);
            string headerValue = cell.GetValue<string>()?.Trim() ?? $"Column{col}";
            csvData.Headers.Add(string.IsNullOrEmpty(headerValue) ? $"Column{col}" : headerValue);
        }

        // 두 번째 행부터 데이터로 파싱
        for (int row = firstRow + 1; row <= lastRow; row++)
        {
            string[] rowData = new string[csvData.Headers.Count];
            bool hasData = false;

            for (int col = firstColumn; col <= lastColumn; col++)
            {
                int colIndex = col - firstColumn;
                var cell = worksheet.Cell(row, col);

                // 셀 값을 문자열로 가져오기 (숫자 포맷 유지)
                string cellValue;
                if (cell.DataType == XLDataType.Number)
                {
                    cellValue = cell.GetValue<double>().ToString();
                }
                else if (cell.DataType == XLDataType.DateTime)
                {
                    cellValue = cell.GetValue<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    cellValue = cell.GetValue<string>()?.Trim() ?? string.Empty;
                }

                rowData[colIndex] = cellValue;
                if (!string.IsNullOrEmpty(cellValue))
                    hasData = true;
            }

            // 빈 행은 건너뛰기
            if (hasData)
            {
                csvData.Rows.Add(rowData);
            }
        }

        if (csvData.Rows.Count == 0)
        {
            throw new InvalidDataException("헤더를 제외한 데이터가 없습니다.");
        }

        return csvData;
    }

    /// <summary>
    /// Excel 파일의 시트 이름 목록 가져오기
    /// </summary>
    /// <param name="__filePath">Excel 파일 경로</param>
    /// <returns>시트 이름 목록</returns>
    public static List<string> GetSheetNames(string __filePath)
    {
        if (!File.Exists(__filePath))
        {
            throw new FileNotFoundException("Excel 파일을 찾을 수 없습니다.", __filePath);
        }

        using var workbook = new XLWorkbook(__filePath);
        return workbook.Worksheets.Select(__ws => __ws.Name).ToList();
    }

    /// <summary>
    /// 파일이 Excel 파일(.xlsx, .xls, .xlsm)인지 확인
    /// </summary>
    /// <param name="__filePath">파일 경로</param>
    /// <returns>Excel 파일 여부</returns>
    public static bool IsExcelFile(string __filePath)
    {
        string extension = Path.GetExtension(__filePath).ToLowerInvariant();
        return extension == ".xlsx" || extension == ".xls" || extension == ".xlsm";
    }
}
