using Qmmands;
using Revcord.Entities;
using Revcord.Revolt;
using RevoltSharp;

namespace Revcord.Commands;

public class RevoltEmojiTypeParser : RevcordTypeParser<IEmoji> {
	public async override ValueTask<TypeParserResult<IEmoji>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		if (!(value.StartsWith(':') && value.EndsWith(':'))) {
			return TypeParserResult<IEmoji>.Failed("Unknown emote");
		}

		string id = value[1..^1];
		
		var revolt = (RevoltChatClient) context.Client;
		// TODO non customized emoji
		// Cast to nullable because the library doesn't have proper nullable reference types (TODO: pr to fix)

		if (RevoltEmoji.EmojiNamesToUnicode.TryGetValue(id, out string? unicode)) {
			return new TypeParserResult<IEmoji>(new RevoltEmoji(revolt, unicode));
		}
		
		var guildEmote = (Emoji?) await revolt.Revolt.Rest.GetEmojiAsync(id);
		if (guildEmote != null) {
			return TypeParserResult<IEmoji>.Successful(new RevoltEmoji(revolt, guildEmote));
		} else {
			return TypeParserResult<IEmoji>.Failed("Unknown emote");
		}
	}
}
