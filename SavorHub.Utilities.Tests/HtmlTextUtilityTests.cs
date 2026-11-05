using SavorHub.Utilities;

namespace SavorHub.Utilities.Tests
{
    public class HtmlTextUtilityTests
    {
        [Fact]
        public void HtmlToPlainText_RemovesParagraphTags()
        {
            Assert.Equal("Ebi Fried Shrimp", HtmlTextUtility.HtmlToPlainText("<p>Ebi Fried Shrimp</p>"));
        }

        [Fact]
        public void HtmlToPlainText_PutsEachParagraphOnItsOwnLine()
        {
            var text = HtmlTextUtility.HtmlToPlainText("<p>Hot wings</p><p>Extra <strong>sauce</strong></p>");

            Assert.Equal("Hot wings\nExtra sauce", text);
        }

        [Fact]
        public void HtmlToPlainText_DecodesHtmlEntities()
        {
            Assert.Equal("Mac & Cheese", HtmlTextUtility.HtmlToPlainText("<p>Mac&nbsp;&amp;&nbsp;Cheese</p>"));
        }

        [Fact]
        public void HtmlToPlainText_LeavesPlainTextUnchanged()
        {
            Assert.Equal("Whole Pizza topped with pepperoni.", HtmlTextUtility.HtmlToPlainText("Whole Pizza topped with pepperoni."));
        }

        [Fact]
        public void PlainTextToHtml_WrapsEachLineInParagraphTags()
        {
            Assert.Equal("<p>Hot wings</p><p>Extra sauce</p>", HtmlTextUtility.PlainTextToHtml("Hot wings\nExtra sauce"));
        }

        [Fact]
        public void PlainTextToHtml_EncodesSpecialCharacters()
        {
            Assert.Equal("<p>Mac &amp; Cheese</p>", HtmlTextUtility.PlainTextToHtml("Mac & Cheese"));
        }

        [Fact]
        public void PlainTextToHtml_KeepsExistingHtmlAsIs()
        {
            Assert.Equal("<p>Already <em>HTML</em></p>", HtmlTextUtility.PlainTextToHtml("<p>Already <em>HTML</em></p>"));
        }

        [Fact]
        public void PlainTextToHtml_ReturnsEmptyString_WhenTextIsBlank()
        {
            Assert.Equal(string.Empty, HtmlTextUtility.PlainTextToHtml("   "));
        }

        [Fact]
        public void HtmlToPlainText_ThenPlainTextToHtml_RestoresParagraphs()
        {
            var original = "<p>Chicken Wings coated with Buffalo sauce</p>";

            var restored = HtmlTextUtility.PlainTextToHtml(HtmlTextUtility.HtmlToPlainText(original));

            Assert.Equal(original, restored);
        }
    }
}
