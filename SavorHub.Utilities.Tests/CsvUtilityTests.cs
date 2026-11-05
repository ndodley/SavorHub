using SavorHub.Utilities;

namespace SavorHub.Utilities.Tests
{
    public class CsvUtilityTests
    {
        [Fact]
        public void Escape_ReturnsValueUnchanged_WhenNoSpecialCharacters()
        {
            Assert.Equal("Fried Shrimp", CsvUtility.Escape("Fried Shrimp"));
        }

        [Fact]
        public void Escape_ReturnsEmptyString_WhenValueIsNull()
        {
            Assert.Equal(string.Empty, CsvUtility.Escape(null));
        }

        [Fact]
        public void Escape_WrapsInQuotes_WhenValueContainsComma()
        {
            Assert.Equal("\"Crispy, golden\"", CsvUtility.Escape("Crispy, golden"));
        }

        [Fact]
        public void Escape_DoublesQuotes_WhenValueContainsQuotes()
        {
            Assert.Equal("\"The \"\"best\"\" wings\"", CsvUtility.Escape("The \"best\" wings"));
        }

        [Fact]
        public void Escape_WrapsInQuotes_WhenValueContainsLineBreak()
        {
            Assert.Equal("\"Line 1\nLine 2\"", CsvUtility.Escape("Line 1\nLine 2"));
        }

        [Fact]
        public void BuildRow_JoinsEscapedValuesWithCommas()
        {
            var row = CsvUtility.BuildRow("Wings", "Hot, spicy", "3.99");

            Assert.Equal("Wings,\"Hot, spicy\",3.99", row);
        }

        [Fact]
        public void Parse_SplitsSimpleRowsAndFields()
        {
            var rows = CsvUtility.Parse("Name,Price\nWings,3.99\n");

            Assert.Equal(2, rows.Count);
            Assert.Equal(new[] { "Name", "Price" }, rows[0]);
            Assert.Equal(new[] { "Wings", "3.99" }, rows[1]);
        }

        [Fact]
        public void Parse_KeepsCommasQuotesAndLineBreaksInsideQuotedFields()
        {
            var csv = "Name,Description\r\nWings,\"<p>Hot, \"\"spicy\"\"</p>\r\n<p>Served cold</p>\"\r\n";

            var rows = CsvUtility.Parse(csv);

            Assert.Equal(2, rows.Count);
            Assert.Equal("Wings", rows[1][0]);
            Assert.Equal("<p>Hot, \"spicy\"</p>\r\n<p>Served cold</p>", rows[1][1]);
        }

        [Fact]
        public void Parse_SkipsBlankLines()
        {
            var rows = CsvUtility.Parse("A,B\n\n1,2\n\n");

            Assert.Equal(2, rows.Count);
        }

        [Fact]
        public void BuildRow_ThenParse_RoundTripsOriginalValues()
        {
            var original = new[] { "Wings", "<p>Hot, \"spicy\"</p>\n<p>Extra sauce</p>", "3.99" };

            var rows = CsvUtility.Parse(CsvUtility.BuildRow(original));

            Assert.Single(rows);
            Assert.Equal(original, rows[0]);
        }
    }
}
