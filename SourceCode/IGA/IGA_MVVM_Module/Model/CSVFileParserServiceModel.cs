namespace IGA_GUI_Module.Model;

public class CSVFileParserServiceModel
{
    public static string delimiter = ",";

    public static string[] Parse(string __csv)
    {
        return __csv.Split(delimiter);
    }
}