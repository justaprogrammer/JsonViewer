using EPocalipse.Json.Viewer;
using NUnit.Framework;

namespace JsonViewer.Test.Domain
{
	[TestFixture]
	public class JsonFieldsTests
	{

		[Test]
		public void Should_Contstruct_New_Fields_Correctly()
		{
			var parent = new JsonObject();
			var fields = new JsonFields(parent);

			Assert.That(fields.Count, Is.EqualTo(0));
		}
	}
}
