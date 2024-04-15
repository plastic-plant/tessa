namespace Tessa.Application.Models;

public class AppRegistry
{
    public List<TessdataModel> Tessdata { get; set; } = new();

    public class TessdataModel()
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public int Size { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
	}
}
