using Application;
using System.Runtime;
using Tesseract;

namespace Infrastructure
{

	public class TesseractRepository : ITesseractRepository
	{
		public string Process(string filename)
		{
			using var engine = new TesseractEngine(@"./tessdata", "eng");
			using var image = Pix.LoadFromFile(filename);
			using var page = engine.Process(image);
			var text = page.GetText();

			var filenameResults = $"{filename}.tesseract.txt";
			File.WriteAllText(filenameResults, text);
			return text;
		}
	}
}
