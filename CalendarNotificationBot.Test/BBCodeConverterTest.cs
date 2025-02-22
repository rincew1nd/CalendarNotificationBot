using CalendarNotificationBot.Domain.Extensions;
using Xunit.Abstractions;

namespace CalendarNotificationBot.Test;

public class BBCodeTestData
{
    public static IEnumerable<object[]> TestCases()
    {
        yield return ["[B]bold[/B]", "<b>bold</b>"];
        yield return ["[I]italic[/I]", "<i>italic</i>"];
        yield return ["[U]underscore[/U]", "<u>underscore</u>"];
        yield return ["[S]stroked[/S]", "<strike>stroked</strike>"];
        yield return ["[URL=http://wret.wqert]qwer[/URL]", "<a href=\"http://wret.wqert\">qwer</a>"];
        yield return ["[IMG]http://asdfasdf.asdf/[/IMG]", "<a href=\"http://asdfasdf.asdf/\">image link</a>"];
        yield return ["line1\\nline2", "line1<br />line2"];
        yield return ["[B]bold [I]italic and bold[/I][/B]", "<b>bold <i>italic and bold</i></b>"];
        yield return ["text before [B]bold[/B] text after", "text before <b>bold</b> text after"];
        yield return ["[B]bold[/B]\\n[I]italic[/I]\\n[U]underscore[/U]\\n[S]stroked[/S]\\n[COLOR=#00ffff]color[/COLOR]\\n[FONT=monospace]font][/FONT]\\n[LIST=1][*]list1[*]list2[*]list3[/LIST]\\n[CENTER]asdfasdf[/CENTER]\\n[RIGHT]asdf[/RIGHT]\\n[JUSTIFY]asdfasdf[/JUSTIFY]\\n[LEFT]asdf[/LEFT]\\n[URL=http://wret.wqert]qwer[/URL]\\n[IMG]http://asdfasdf.asdf/[/IMG]", "<b>bold</b><br /><i>italic</i><br /><u>underscore</u><br /><strike>stroked</strike><br />color<br />font]<br />list1list2list3<br />asdfasdf<br />asdf<br />asdfasdf<br />asdf<br /><a href=\"http://wret.wqert\">qwer</a><br /><a href=\"http://asdfasdf.asdf/\">image link</a>"];
    }
}

public class BBCodeConverterTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BBCodeConverterTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [MemberData(nameof(BBCodeTestData.TestCases), MemberType = typeof(BBCodeTestData))]
    public void BBCodeToHtmlConverterTests(string bbCode, string expectedHtml)
    {
        string actualHtml = bbCode.EscapeStringForHtml();
        _testOutputHelper.WriteLine(actualHtml);
        Assert.Equal(expectedHtml, actualHtml);
    }
}
