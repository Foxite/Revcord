using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Revcord.Revolt; 

// Copied from https://github.com/revoltchat/revite/blob/master/src/assets/emojis.ts.
// TODO: autogenerate this file as part of build process, remove from git.
public partial class RevoltEmoji {
	// Note: another member below this dictionary initializer (sorry, it can't be above because this initializer needs to run first)
	public static IReadOnlyDictionary<string, string> EmojiNamesToUnicode { get; }
	public static IReadOnlyDictionary<string, string> UnicodeToEmojiNames { get; }

	static RevoltEmoji() {
		// Read resource file
		// https://stackoverflow.com/a/3314213
		var assembly = Assembly.GetExecutingAssembly();
		const string resourceName = "Revolt/Entities/RevoltEmojiNames.json";

		var emojiNamesToUnicode = new Dictionary<string, string>();
		var unicodeToEmojiName  = new Dictionary<string, string>();
		
		using (Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception("Resource missing: " + resourceName))
		using (StreamReader streamReader = new StreamReader(stream))
		using (JsonReader jsonReader = new JsonTextReader(streamReader)) {
			foreach (JProperty prop in JObject.Load(jsonReader).Properties()) {
				string propValue = prop.ToObject<string>()!;
				emojiNamesToUnicode[prop.Name] = propValue;
				unicodeToEmojiName[propValue] = prop.Name;
			}
		}
		
		EmojiNamesToUnicode = emojiNamesToUnicode;
		UnicodeToEmojiNames = unicodeToEmojiName;
	}
}
