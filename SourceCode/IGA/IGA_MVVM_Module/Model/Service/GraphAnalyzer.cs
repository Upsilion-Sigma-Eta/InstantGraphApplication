using IGA_GUI_Module.Model.Domain;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// 그래프 분석 서비스
/// 최댓값/최솟값 찾기, 선형 회귀 분석, 통계 연산 기능 제공
/// </summary>
public static class GraphAnalyzer
{
    #region 극값 찾기

    /// <summary>
    /// 데이터에서 최댓값과 해당 인덱스 찾기
    /// </summary>
    public static (double maxValue, int index) FindMax(double[] __values)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        int maxIndex = 0;
        double maxValue = __values[0];

        for (int i = 1; i < __values.Length; i++)
        {
            if (__values[i] > maxValue)
            {
                maxValue = __values[i];
                maxIndex = i;
            }
        }

        return (maxValue, maxIndex);
    }

    /// <summary>
    /// 데이터에서 최솟값과 해당 인덱스 찾기
    /// </summary>
    public static (double minValue, int index) FindMin(double[] __values)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        int minIndex = 0;
        double minValue = __values[0];

        for (int i = 1; i < __values.Length; i++)
        {
            if (__values[i] < minValue)
            {
                minValue = __values[i];
                minIndex = i;
            }
        }

        return (minValue, minIndex);
    }

    #endregion

    #region 통계 연산

    /// <summary>
    /// 평균값 계산
    /// </summary>
    public static double CalculateMean(double[] __values)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        double sum = 0;
        for (int i = 0; i < __values.Length; i++)
        {
            sum += __values[i];
        }
        return sum / __values.Length;
    }

    /// <summary>
    /// 분산 계산 (모분산)
    /// </summary>
    public static double CalculateVariance(double[] __values)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        double mean = CalculateMean(__values);
        double sumOfSquares = 0;

        for (int i = 0; i < __values.Length; i++)
        {
            double diff = __values[i] - mean;
            sumOfSquares += diff * diff;
        }

        return sumOfSquares / __values.Length;
    }

    /// <summary>
    /// 표준편차 계산 (모표준편차)
    /// </summary>
    public static double CalculateStandardDeviation(double[] __values)
    {
        return Math.Sqrt(CalculateVariance(__values));
    }

    /// <summary>
    /// 표본 분산 계산
    /// </summary>
    public static double CalculateSampleVariance(double[] __values)
    {
        if (__values == null || __values.Length < 2)
            throw new ArgumentException("표본 분산 계산에는 최소 2개의 데이터가 필요합니다.", nameof(__values));

        double mean = CalculateMean(__values);
        double sumOfSquares = 0;

        for (int i = 0; i < __values.Length; i++)
        {
            double diff = __values[i] - mean;
            sumOfSquares += diff * diff;
        }

        return sumOfSquares / (__values.Length - 1);
    }

    /// <summary>
    /// 표본 표준편차 계산
    /// </summary>
    public static double CalculateSampleStandardDeviation(double[] __values)
    {
        return Math.Sqrt(CalculateSampleVariance(__values));
    }

    /// <summary>
    /// 합계 계산
    /// </summary>
    public static double CalculateSum(double[] __values)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        double sum = 0;
        for (int i = 0; i < __values.Length; i++)
        {
            sum += __values[i];
        }
        return sum;
    }

    /// <summary>
    /// 모든 통계값 계산 결과
    /// </summary>
    public static StatisticsResult CalculateAllStatistics(double[] __values)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        var (maxValue, maxIndex) = FindMax(__values);
        var (minValue, minIndex) = FindMin(__values);
        double mean = CalculateMean(__values);
        double variance = CalculateVariance(__values);
        double stdDev = Math.Sqrt(variance);
        double sum = CalculateSum(__values);

        double sampleVariance = __values.Length > 1 ? CalculateSampleVariance(__values) : 0;
        double sampleStdDev = Math.Sqrt(sampleVariance);

        return new StatisticsResult
        {
            Count = __values.Length,
            Sum = sum,
            Mean = mean,
            Variance = variance,
            StandardDeviation = stdDev,
            SampleVariance = sampleVariance,
            SampleStandardDeviation = sampleStdDev,
            Max = maxValue,
            MaxIndex = maxIndex,
            Min = minValue,
            MinIndex = minIndex,
            Range = maxValue - minValue
        };
    }

    #endregion

    #region 선형 회귀 분석

    /// <summary>
    /// 선형 회귀 분석 수행 (최소자승법)
    /// y = slope * x + intercept
    /// </summary>
    public static LinearRegressionResult CalculateLinearRegression(double[] __xValues, double[] __yValues)
    {
        if (__xValues == null || __yValues == null)
            throw new ArgumentNullException("X 또는 Y 데이터가 null입니다.");

        if (__xValues.Length != __yValues.Length)
            throw new ArgumentException("X와 Y 데이터의 길이가 다릅니다.");

        if (__xValues.Length < 2)
            throw new ArgumentException("선형 회귀에는 최소 2개의 데이터가 필요합니다.");

        int n = __xValues.Length;

        // 평균 계산
        double meanX = CalculateMean(__xValues);
        double meanY = CalculateMean(__yValues);

        // 기울기(slope)와 절편(intercept) 계산
        double sumXY = 0;
        double sumX2 = 0;
        double sumY2 = 0;

        for (int i = 0; i < n; i++)
        {
            double dx = __xValues[i] - meanX;
            double dy = __yValues[i] - meanY;
            sumXY += dx * dy;
            sumX2 += dx * dx;
            sumY2 += dy * dy;
        }

        // 기울기
        double slope = sumX2 != 0 ? sumXY / sumX2 : 0;

        // 절편
        double intercept = meanY - slope * meanX;

        // 결정계수 R² 계산
        double ssTot = sumY2;  // Total sum of squares
        double ssRes = 0;      // Residual sum of squares

        for (int i = 0; i < n; i++)
        {
            double predicted = slope * __xValues[i] + intercept;
            double residual = __yValues[i] - predicted;
            ssRes += residual * residual;
        }

        double rSquared = ssTot != 0 ? 1 - (ssRes / ssTot) : 0;

        // 상관계수 R 계산
        double correlation = sumX2 != 0 && sumY2 != 0
            ? sumXY / Math.Sqrt(sumX2 * sumY2)
            : 0;

        return new LinearRegressionResult
        {
            Slope = slope,
            Intercept = intercept,
            RSquared = rSquared,
            Correlation = correlation,
            DataCount = n
        };
    }

    /// <summary>
    /// 선형 회귀선의 예측값 배열 생성
    /// </summary>
    public static double[] GenerateRegressionLine(double[] __xValues, LinearRegressionResult __regression)
    {
        double[] predictedY = new double[__xValues.Length];
        for (int i = 0; i < __xValues.Length; i++)
        {
            predictedY[i] = __regression.Slope * __xValues[i] + __regression.Intercept;
        }
        return predictedY;
    }

    #endregion

    #region 편차 분석

    /// <summary>
    /// 기준값 대비 편차 계산
    /// </summary>
    public static double[] CalculateDeviations(double[] __values, double __referenceValue)
    {
        if (__values == null || __values.Length == 0)
            throw new ArgumentException("데이터가 비어있습니다.", nameof(__values));

        double[] deviations = new double[__values.Length];
        for (int i = 0; i < __values.Length; i++)
        {
            deviations[i] = __values[i] - __referenceValue;
        }
        return deviations;
    }

    /// <summary>
    /// 평균 대비 편차 계산
    /// </summary>
    public static double[] CalculateDeviationsFromMean(double[] __values)
    {
        double mean = CalculateMean(__values);
        return CalculateDeviations(__values, mean);
    }

    /// <summary>
    /// 편차의 절대값 배열 반환
    /// </summary>
    public static double[] CalculateAbsoluteDeviations(double[] __values, double __referenceValue)
    {
        double[] deviations = CalculateDeviations(__values, __referenceValue);
        for (int i = 0; i < deviations.Length; i++)
        {
            deviations[i] = Math.Abs(deviations[i]);
        }
        return deviations;
    }

    /// <summary>
    /// 평균 절대 편차 (MAD) 계산
    /// </summary>
    public static double CalculateMeanAbsoluteDeviation(double[] __values)
    {
        double mean = CalculateMean(__values);
        double[] absDeviations = CalculateAbsoluteDeviations(__values, mean);
        return CalculateMean(absDeviations);
    }

    #endregion
}
