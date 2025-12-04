namespace IGA_GUI_Module.Model.Domain;

/// <summary>
/// 선형 회귀 분석 결과
/// </summary>
public class LinearRegressionResult
{
    /// <summary>
    /// 기울기
    /// </summary>
    public double Slope { get; set; }

    /// <summary>
    /// Y 절편
    /// </summary>
    public double Intercept { get; set; }

    /// <summary>
    /// 결정계수 (R²)
    /// </summary>
    public double RSquared { get; set; }

    /// <summary>
    /// 상관계수 (R)
    /// </summary>
    public double Correlation { get; set; }

    /// <summary>
    /// 데이터 개수
    /// </summary>
    public int DataCount { get; set; }

    /// <summary>
    /// 회귀식 문자열
    /// </summary>
    public string GetEquation()
    {
        string sign = Intercept >= 0 ? "+" : "-";
        return $"y = {Slope:F4}x {sign} {Math.Abs(Intercept):F4}";
    }

    public override string ToString()
    {
        return $"{GetEquation()}, R² = {RSquared:F4}";
    }
}
