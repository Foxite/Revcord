using System.Text.RegularExpressions;
using Qmmands;
using Revcord.Discord;
using DiscordGuild = DSharpPlus.Entities.DiscordGuild;

namespace Revcord.Commands;

public class DiscordEmojiTypeParser : RevcordTypeParser<DiscordEmoji> {
	public static readonly Regex Regex = new Regex(@"^<a?:\w+:(?<Id>[0-9]+)>$");
		
	public override ValueTask<TypeParserResult<DiscordEmoji>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		foreach (DiscordGuild guild in ((DiscordChatClient) context.Client).DSharp.Guilds.Values) {
			Console.WriteLine(guild.Emojis.Count);
		}
		
		DSharpPlus.Entities.DiscordEmoji result;
		Match match = Regex.Match(value);
		if (match.Success) {
			ulong id = ulong.Parse(match.Groups["Id"].Value);
			try {
				result = DSharpPlus.Entities.DiscordEmoji.FromGuildEmote(((DiscordChatClient) context.Client).DSharp, id);
			} catch (KeyNotFoundException) {
				return TypeParserResult<DiscordEmoji>.Failed("Unknown emote");
			}
		} else {
			try {
				result = DSharpPlus.Entities.DiscordEmoji.FromUnicode(value);
			} catch (ArgumentException) {
				return TypeParserResult<DiscordEmoji>.Failed("Not a emote or emoji");
			}
		}
		
		
		return TypeParserResult<DiscordEmoji>.Successful(new DiscordEmoji(context.Client, result));
	}
}
