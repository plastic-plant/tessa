using System.Reflection;

namespace Infrastructure.Tests
{
	public class TesseractRepositoryTests
	{

		[Fact]
		public void Process_Jpeg_Returns()
		{
			var repository = new TesseractRepository();
			var filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "examples", "example2.jpeg");

			var result = repository.Process(filename);

			Assert.NotNull(result);
		}
	}
}