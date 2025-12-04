namespace IGA_GUI_Module.Model.Domain;

/// <summary>
/// 통계 계산 결과
/// </summary>
public class StatisticsResult
{
    /// <summary>
    /// 데이터 개수
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 합계
    /// </summary>
    public double Sum { get; set; }

    /// <summary>
    /// 평균
    /// </summary>
    public double Mean { get; set; }

    /// <summary>
    /// 모분산
    /// </summary>
    public double Variance { get; set; }

    /// <summary>
    /// 모표준편차
    /// </summary>
    public double StandardDeviation { get; set; }

    /// <summary>
    /// 표본분산
    /// </summary>
    public double SampleVariance { get; set; }

    /// <summary>
    /// 표본표준편차
    /// </summary>
    public double SampleStandardDeviation { get; set; }

    /// <summary>
    /// 최댓값
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// 최댓값 인덱스
    /// </summary>
    public int MaxIndex { get; set; }

    /// <summary>
    /// 최솟값
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// 최솟값 인덱스
    /// </summary>
    public int MinIndex { get; set; }

    /// <summary>
    /// 범위 (최대 - 최소)
    /// </summary>
    public double Range { get; set; }

    public override string ToString()
    {
        return $"개수: {Count}, 합계: {Sum:F4}, 평균: {Mean:F4}, " +
               $"표준편차: {StandardDeviation:F4}, 최대: {Max:F4}, 최소: {Min:F4}";
    }
}
