using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qmmands;
using Revcord.Entities;

namespace Revcord.Commands;

public static class RevcordCommands {
	public static void AddRevcordCommands(this IServiceCollection isc, Action<CommandService>? configureCommandService = null) {
		isc.TryAddSingleton(isp => new CommandServiceConfiguration() {
			DefaultRunMode = RunMode.Sequential,
		});
		
		isc.AddSingleton(isp => {
			var ret = new CommandService(isp.GetRequiredService<CommandServiceConfiguration>());

			var userParser = new ChatServiceObjectTypeParser<IUser>();
			var channelParser = new ChatServiceObjectTypeParser<IChannel>();
			var emojiParser = new ChatServiceObjectTypeParser<IEmoji>();
			ret.AddTypeParser(userParser);
			ret.AddTypeParser(channelParser);
			ret.AddTypeParser(emojiParser);
			ret.AddTypeParser(new GuildMemberTypeParser(userParser));
			//ret.AddTypeParser(new ChatServiceObjectTypeParser<IMessage>());
			//ret.AddTypeParser(new ChatServiceObjectTypeParser<IGuild>());
			
			foreach (ChatClient client in isp.GetRequiredService<IEnumerable<ChatClient>>()) {
				if (client is Discord.DiscordChatClient) {
					userParser   .Add(client.GetType(), new RegexTypeParser<IUser>(new Regex(@"<@(?<Id>[0-9]+)>"), (context, match) => context.Client.GetUserAsync(new EntityId(ulong.Parse(match.Groups["Id"].Value))), "That is not a valid user mention."));
					channelParser.Add(client.GetType(), new RegexTypeParser<IChannel>(new Regex(@"<#(?<Id>[0-9]+)>"), (context, match) => context.Client.GetChannelAsync(new EntityId(ulong.Parse(match.Groups["Id"].Value))), "That is not a valid channel mention."));
					emojiParser  .Add(client.GetType(), new DiscordEmojiTypeParser());
				} else if (client is Revolt.RevoltChatClient) {
					userParser   .Add(client.GetType(), new RegexTypeParser<IUser>(new Regex(@"<@(?<Id>[0-9A-Z]+)>"), (context, match) => context.Client.GetUserAsync(new EntityId(match.Groups["Id"].Value)), "That is not a valid user mention."));
					channelParser.Add(client.GetType(), new RegexTypeParser<IChannel>(new Regex(@"<#(?<Id>[0-9A-Z]+)>"), (context, match) => context.Client.GetChannelAsync(new EntityId(match.Groups["Id"].Value)), "That is not a valid channel mention."));
					emojiParser  .Add(client.GetType(), new RevoltEmojiTypeParser());
				}
			}

			configureCommandService?.Invoke(ret);

			return ret;
		});
	}
}