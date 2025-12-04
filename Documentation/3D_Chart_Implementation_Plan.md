# 3D 차트 기능 구현 계획

## 개요
HelixToolkit.WPF를 사용하여 3D Scatter Plot과 3D Surface Plot 기능을 구현합니다.

## 1. NuGet 패키지 추가

### IGA_MVVM_Module.csproj에 추가
```xml
<PackageReference Include="HelixToolkit.Wpf" Version="2.25.0" />
```

## 2. 새로 생성할 파일

### 2.1 Model/Data/ChartDataPoint3D.cs
3D 데이터 포인트 모델 클래스
```csharp
public class ChartDataPoint3D
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}
```

### 2.2 Model/Data/SurfaceData.cs
Surface Plot용 그리드 데이터 모델
```csharp
public class SurfaceData
{
    public double[,] Values { get; set; }  // Z값 그리드
    public double XMin, XMax, YMin, YMax;  // X,Y 범위
}
```

### 2.3 Model/Service/Chart3DFactory.cs
HelixToolkit 3D 요소 생성 팩토리
- CreateScatter3D(): 3D 점 시각화 (SphereVisual3D)
- CreateSurface3D(): 표면 시각화 (MeshGeometry3D + SurfacePlot3D)
- CreateAxis3D(): 3D 축 시각화
- CreateColorLegend(): 색상 범례

## 3. 수정할 파일

### 3.1 Model/Data/ChartType.cs
ChartType enum에 3D 타입 추가:
```csharp
public enum ChartType
{
    // 기존 2D 타입들...
    Line,
    Column,
    Bar,
    Area,
    Scatter,
    StepLine,
    Pie,

    // 새로운 3D 타입들
    Scatter3D,   // 3D 산점도
    Surface3D    // 3D 표면 그래프
}
```

ChartTypeItems 컬렉션에 추가:
```csharp
new ChartTypeItem(ChartType.Scatter3D, "3D 점 그래프"),
new ChartTypeItem(ChartType.Surface3D, "3D 표면 그래프")
```

### 3.2 ViewModel/DataAnalysisViewModel.cs
추가할 속성:
```csharp
// 3D 차트 표시 여부
public bool Is3DChartVisible { get; }

// 3D 데이터 포인트
public ObservableCollection<ChartDataPoint3D> Data3DPoints { get; }

// 3D Surface 데이터
public SurfaceData SurfaceData { get; }

// Z축 선택 인덱스
public int SelectedZColumnIndex { get; set; }

// Z축 선택용 체크박스 아이템 (다중 Z축 지원은 선택사항)
public ObservableCollection<YColumnSelectionItem> ZColumnSelectionItems { get; }

// 3D 카메라 설정
public Point3D CameraPosition { get; set; }
public Vector3D CameraLookDirection { get; set; }
```

추가할 메서드:
```csharp
private void DrawGraph3D_Execute(object? __parameter)
private void Draw3DScatter()
private void Draw3DSurface()
private bool Is3DChartType(ChartType type)
```

### 3.3 View/DataAnalysisView.xaml
#### XAML 네임스페이스 추가:
```xml
xmlns:helix="clr-namespace:HelixToolkit.Wpf;assembly=HelixToolkit.Wpf"
```

#### 차트 영역에 3D 뷰포트 추가:
```xml
<Border Grid.Row="2" x:Name="ChartContainer" ...>
    <Grid>
        <!-- 기존 2D 차트 -->
        <lvc:CartesianChart ... Visibility="{Binding IsCartesianChartVisible}" />
        <lvc:PieChart ... Visibility="{Binding IsPieChartVisible}" />

        <!-- 새로운 3D 차트 -->
        <helix:HelixViewport3D x:Name="Viewport3D"
                               Visibility="{Binding Is3DChartVisible}"
                               ZoomExtentsWhenLoaded="True"
                               ShowCoordinateSystem="True"
                               CoordinateSystemLabelForeground="White">
            <helix:DefaultLights />
            <!-- 3D 콘텐츠는 코드-비하인드에서 동적 추가 -->
        </helix:HelixViewport3D>
    </Grid>
</Border>
```

#### Z축 선택 UI 추가 (3D 차트용):
```xml
<!-- 3D 차트용 Z축 선택 -->
<StackPanel Orientation="Horizontal"
            Visibility="{Binding Is3DChartTypeSelected}">
    <Label Content="Z축:" FontWeight="SemiBold" Width="50" />
    <ComboBox ItemsSource="{Binding ColumnHeaders}"
              SelectedIndex="{Binding SelectedZColumnIndex}"
              MinWidth="150" />
</StackPanel>
```

### 3.4 View/DataAnalysisView.xaml.cs
3D 차트 렌더링 헬퍼 메서드 추가:
```csharp
private void RenderScatter3D(IEnumerable<ChartDataPoint3D> points)
private void RenderSurface3D(SurfaceData surface)
private void ClearViewport3D()
```

## 4. 구현 단계

### Phase 1: 기반 구조 (필수)
1. HelixToolkit.Wpf NuGet 패키지 추가
2. ChartDataPoint3D.cs 모델 생성
3. ChartType enum에 Scatter3D 추가
4. DataAnalysisViewModel에 3D 관련 속성 추가
5. DataAnalysisView.xaml에 HelixViewport3D 추가

### Phase 2: 3D Scatter Plot 구현
1. Chart3DFactory.CreateScatter3D() 구현
2. DrawGraph3D_Execute() 내 Scatter3D 로직 구현
3. 3D 축/그리드 시각화
4. 색상 매핑 (Z값 기반)
5. 마우스 회전/줌 인터랙션 (HelixToolkit 기본 제공)

### Phase 3: 3D Surface Plot 구현
1. SurfaceData 모델 생성
2. Chart3DFactory.CreateSurface3D() 구현
3. 데이터 → 그리드 보간 로직 (선택사항)
4. 컬러맵 기반 표면 렌더링
5. 와이어프레임 옵션

### Phase 4: UI 통합 및 마무리
1. 차트 유형 전환 로직 완성
2. 3D 차트 저장 기능 (렌더링 → 이미지)
3. 3D 카메라 컨트롤 UI
4. 색상 범례 표시
5. 테스트 및 버그 수정

## 5. 파일 변경 요약

### 새로 생성
| 파일 경로 | 설명 |
|-----------|------|
| Model/Data/ChartDataPoint3D.cs | 3D 데이터 포인트 |
| Model/Data/SurfaceData.cs | Surface Plot 데이터 |
| Model/Service/Chart3DFactory.cs | 3D 요소 팩토리 |

### 수정
| 파일 경로 | 변경 내용 |
|-----------|-----------|
| IGA_MVVM_Module.csproj | HelixToolkit.Wpf 패키지 참조 추가 |
| Model/Data/ChartType.cs | Scatter3D, Surface3D enum 값 추가 |
| ViewModel/DataAnalysisViewModel.cs | 3D 관련 속성/명령/메서드 추가 |
| View/DataAnalysisView.xaml | HelixViewport3D, Z축 선택 UI 추가 |
| View/DataAnalysisView.xaml.cs | 3D 렌더링 헬퍼 메서드 추가 |

## 6. 기술적 고려사항

### 성능
- 대용량 데이터(>10,000점)는 LOD(Level of Detail) 적용 고려
- Surface Plot은 그리드 해상도 제한 (최대 100x100)

### 호환성
- HelixToolkit.Wpf 2.25.0은 .NET 9.0 지원
- 기존 LiveCharts2와 충돌 없음

### 사용자 경험
- 2D↔3D 차트 전환 시 자연스러운 UI 전환
- 3D 뷰포트는 마우스 드래그 회전, 휠 줌 기본 지원
- 좌표축 표시로 방향 인식 용이

## 7. 예상 작업량

- Phase 1: 기반 구조 ~
- Phase 2: 3D Scatter ~
- Phase 3: 3D Surface ~
- Phase 4: 통합/마무리 ~

**총 예상: 4개 Phase 순차 구현**
