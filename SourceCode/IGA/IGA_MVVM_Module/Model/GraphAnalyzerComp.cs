namespace IGA_GUI_Module.Model;

public class GraphAnalyzerComp
{
    private string _delimiter = ",";

    public void SetDelimiter(string __delimiter)
    {
        _delimiter = __delimiter;
    }

    public string GetDelimiter()
    {
        return _delimiter;
    }

    public T? GetMaxValue<T>(string __data)
    {
        try
        {
            List<T> parsedData = __data.Split(_delimiter).Select(__item => (T)Convert.ChangeType(__item, typeof(T))).ToList();

            return parsedData.Max();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public T? GetMinValue<T>(string __data)
    {
        try
        {
            List<T> parsedData = __data.Split(_delimiter).Select(__item => (T)Convert.ChangeType(__item, typeof(T))).ToList();
            
            return parsedData.Min();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}