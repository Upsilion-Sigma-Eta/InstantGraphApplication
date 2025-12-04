using System.IO;
using System.Text;
using IGA_GUI_Module.Model.Data;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// CSV 파일 파싱 서비스
/// - CSV 파일을 읽어와서 CsvDataModel로 변환
/// </summary>
public static class CsvParser
{
    private static string _delimiter = ",";

    /// <summary>
    /// 구분자 설정
    /// </summary>
    public static void SetDelimiter(string __delimiter)
    {
        _delimiter = __delimiter;
    }

    /// <summary>
    /// 현재 구분자 반환
    /// </summary>
    public static string GetDelimiter()
    {
        return _delimiter;
    }

    /// <summary>
    /// CSV 파일을 읽어서 CsvDataModel로 반환
    /// </summary>
    /// <param name="__filePath">CSV 파일 경로</param>
    /// <returns>파싱된 CSV 데이터 모델</returns>
    /// <exception cref="FileNotFoundException">파일이 존재하지 않을 경우</exception>
    /// <exception cref="InvalidDataException">CSV 파일이 비어있거나 유효하지 않은 경우</exception>
    public static CsvDataModel LoadFromFile(string __filePath)
    {
        if (!File.Exists(__filePath))
        {
            throw new FileNotFoundException("CSV 파일을 찾을 수 없습니다.", __filePath);
        }

        var csvData = new CsvDataModel
        {
            FilePath = __filePath,
            FileName = Path.GetFileNameWithoutExtension(__filePath)
        };

        // UTF-8로 파일 읽기
        string[] lines = File.ReadAllLines(__filePath, Encoding.UTF8);

        if (lines.Length == 0)
        {
            throw new InvalidDataException("CSV 파일이 비어있습니다.");
        }

        // 첫 번째 행을 헤더로 파싱
        string[] headers = ParseLine(lines[0]);
        foreach (var header in headers)
        {
            csvData.Headers.Add(header.Trim());
        }

        // 두 번째 행부터 데이터로 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = ParseLine(lines[i]);

            // 열 수가 헤더보다 적으면 빈 문자열로 채움
            string[] row = new string[csvData.Headers.Count];
            for (int j = 0; j < row.Length; j++)
            {
                row[j] = j < values.Length ? values[j].Trim() : string.Empty;
            }

            csvData.Rows.Add(row);
        }

        if (csvData.Rows.Count == 0)
        {
            throw new InvalidDataException("헤더를 제외한 데이터가 없습니다.");
        }

        return csvData;
    }

    /// <summary>
    /// 파일이 UTF-8 인코딩인지 확인
    /// </summary>
    /// <param name="__filePath">파일 경로</param>
    /// <returns>UTF-8 인코딩 여부</returns>
    public static bool IsUtf8Encoding(string __filePath)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(__filePath);

            // BOM 체크
            if (bytes.Length >= 3 &&
                bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return true;
            }

            // UTF-8 유효성 검사
            return IsValidUtf8(bytes);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 바이트 배열이 유효한 UTF-8인지 확인
    /// </summary>
    private static bool IsValidUtf8(byte[] __bytes)
    {
        try
        {
            var encoding = new UTF8Encoding(false, true);
            encoding.GetString(__bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// CSV 한 줄을 파싱 (따옴표 처리 포함)
    /// </summary>
    internal static string[] ParseLine(string __line)
    {
        var result = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < __line.Length; i++)
        {
            char c = __line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // 다음 문자도 따옴표면 이스케이프된 따옴표
                    if (i + 1 < __line.Length && __line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++; // 다음 따옴표 건너뛰기
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    currentField.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c.ToString() == _delimiter)
                {
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
        }

        // 마지막 필드 추가
        result.Add(currentField.ToString());

        return result.ToArray();
    }
}
