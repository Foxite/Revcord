using Qmmands;
using Revcord.Revolt;
using RevoltSharp;

namespace Revcord.Commands;

public class RevoltEmojiTypeParser : RevcordTypeParser<RevoltEmoji> {
	public async override ValueTask<TypeParserResult<RevoltEmoji>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		if (!(value.StartsWith(':') && value.EndsWith(':'))) {
			return TypeParserResult<RevoltEmoji>.Failed("Unknown emote");
		}

		string id = value[1..^1];
		
		var revolt = (RevoltChatClient) context.Client;
		// TODO non customized emoji
		// Cast to nullable because the library doesn't have proper nullable reference types (TODO: pr to fix)

		if (RevoltEmoji.EmojiNamesToUnicode.TryGetValue(id, out string? unicode)) {
			return new TypeParserResult<RevoltEmoji>(new RevoltEmoji(revolt, unicode));
		}
		
		var guildEmote = (Emoji?) await revolt.Revolt.Rest.GetEmojiAsync(id);
		if (guildEmote != null) {
			return TypeParserResult<RevoltEmoji>.Successful(new RevoltEmoji(revolt, guildEmote));
		} else {
			return TypeParserResult<RevoltEmoji>.Failed("Unknown emote");
		}
	}
}
