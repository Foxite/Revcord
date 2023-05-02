using Revcord.Revolt;

namespace Revcord.Tests; 

public class RevoltEmojiTests {
	public static string[][] TestCases = {
		new string[] { "fox_face", "🦊" },
		new string[] { "netherlands", "🇳🇱" },
	};
	
	[Test]
	[TestCaseSource(nameof(TestCases))]
	public void Test(string name, string unicode) {
		var n2uSuccess = RevoltEmoji.EmojiNamesToUnicode.TryGetValue(name, out var n2u);
		var u2nSuccess = RevoltEmoji.UnicodeToEmojiNames.TryGetValue(unicode, out var u2n);
		Assert.Multiple(() => {
			Assert.That(n2uSuccess, Is.True);
			Assert.That(u2nSuccess, Is.True);
			Assert.That(unicode, Is.EqualTo(n2u));
			Assert.That(name, Is.EqualTo(u2n));
		});
	}
}
