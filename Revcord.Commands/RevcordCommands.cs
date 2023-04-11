using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qmmands;
using Revcord.Discord;
using Revcord.Entities;
using Revcord.Revolt;

namespace Revcord.Commands;

public static class RevcordCommands {
	public static IServiceCollection AddRevcordCommands(this IServiceCollection isc, Action<CommandService>? configureCommandService = null) {
		isc.TryAddTransient<ICommandSupport<RevoltChatClient>, RevoltCommandSupport>();
		isc.TryAddTransient<ICommandSupport<DiscordChatClient>, DiscordCommandSupport>();
		
		isc.TryAddSingleton(isp => new CommandServiceConfiguration() {
			DefaultRunMode = RunMode.Sequential,
		});
		
		isc.AddSingleton(isp => {
			var ret = new CommandService(isp.GetRequiredService<CommandServiceConfiguration>());

			var userParser = new ChatServiceObjectTypeParser<IUser>();
			ret.AddTypeParser(userParser);
			ret.AddTypeParser(new ChatServiceObjectTypeParser<IChannel>());
			ret.AddTypeParser(new ChatServiceObjectTypeParser<IEmoji>());
			ret.AddTypeParser(new GuildMemberTypeParser(userParser));
			//ret.AddTypeParser(new ChatServiceObjectTypeParser<IMessage>());
			//ret.AddTypeParser(new ChatServiceObjectTypeParser<IGuild>());
			
			foreach (ChatClient client in isp.GetRequiredService<IEnumerable<ChatClient>>()) {
				var commandSupport = (ICommandSupport?) isp.GetService(typeof(ICommandSupport<>).MakeGenericType(client.GetType()));

				if (commandSupport != null) {
					commandSupport.Install(ret, client, isp);
				} else {
					// TODO logger
					Console.WriteLine($"No command support available for {client.GetType().Name}");
				}
			}

			configureCommandService?.Invoke(ret);

			return ret;
		});
		
		return isc;
	}
}
