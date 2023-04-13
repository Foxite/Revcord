using System.Text.RegularExpressions;
using Qmmands;
using Revcord.Discord;
using Revcord.Entities;
using DiscordGuild = DSharpPlus.Entities.DiscordGuild;

namespace Revcord.Commands;

public class DiscordEmojiTypeParser : RevcordTypeParser<IEmoji> {
	public static readonly Regex Regex = new Regex(@"^<a?:\w+:(?<Id>[0-9]+)>$");
		
	public override ValueTask<TypeParserResult<IEmoji>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
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
				return TypeParserResult<IEmoji>.Failed("Unknown emote");
			}
		} else {
			try {
				result = DSharpPlus.Entities.DiscordEmoji.FromUnicode(value);
			} catch (ArgumentException) {
				return TypeParserResult<IEmoji>.Failed("Not a emote or emoji");
			}
		}
		
		
		return TypeParserResult<IEmoji>.Successful(new DiscordEmoji(context.Client, result));
	}
}
