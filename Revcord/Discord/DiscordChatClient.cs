using DSharpPlus;
using Microsoft.Extensions.Logging;
using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordChatClient : ChatClient {
	private readonly DiscordClient m_Discord;
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public DiscordChatClient(string botToken, DiscordIntents intents, ILoggerFactory loggerFactory) {
		m_Discord = new DiscordClient(new DiscordConfiguration() {
			Intents = intents,
			LoggerFactory = loggerFactory,
			Token = botToken
		});

		m_Discord.MessageCreated += (_, args) => OnMessageCreated(new DiscordMessage(this, args.Message));
		m_Discord.MessageUpdated += (_, args) => OnMessageUpdated(new DiscordMessage(this, args.Message));
		m_Discord.MessageDeleted += (_, args) => OnMessageDeleted(new EntityId(args.Message.Id));
	}

	public async override Task StartAsync() {
		m_Discord.Ready += (_, _) => {
			m_ReadyTcs.SetResult();
			return Task.CompletedTask;
		};
		await m_Discord.ConnectAsync();
		await m_ReadyTcs.Task;
	}

	public override Task<IMessage> GetMessageAsync(EntityId id) => throw new NotImplementedException();
	public override Task<IChannel> GetChannelAsync(EntityId id) => throw new NotImplementedException();
	public override Task<IGuild> GetGuildAsync(EntityId id) => throw new NotImplementedException();
	public override Task<IUser> GetUserAsync(EntityId id) => throw new NotImplementedException();
	public override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId guildUser) => throw new NotImplementedException();
	public override Task<IMessage> SendMessageAsync(EntityId channelId, string message) => throw new NotImplementedException();
	public override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string message) => throw new NotImplementedException();
	public override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => throw new NotImplementedException();
}
