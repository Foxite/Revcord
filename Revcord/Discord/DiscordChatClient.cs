using DSharpPlus;
using Microsoft.Extensions.Logging;
using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordChatClient : ChatClient {
	private readonly DiscordClient m_DSharp;
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public DiscordChatClient(string botToken, DiscordIntents intents, ILoggerFactory? loggerFactory) {
		m_DSharp = new DiscordClient(new DiscordConfiguration() {
			Intents = intents,
			LoggerFactory = loggerFactory,
			Token = botToken
		});

		m_DSharp.MessageCreated += (_, args) => OnMessageCreated(new DiscordMessage(this, args.Message));
		m_DSharp.MessageUpdated += (_, args) => OnMessageUpdated(new DiscordMessage(this, args.Message));
		m_DSharp.MessageDeleted += (_, args) => OnMessageDeleted(new EntityId(args.Message.Id));
	}

	public async override Task StartAsync() {
		m_DSharp.Ready += (_, _) => {
			m_ReadyTcs.SetResult();
			return Task.CompletedTask;
		};
		await m_DSharp.ConnectAsync();
		await m_ReadyTcs.Task;
	}

	// todo: almost all of these do an api call because dsharpplus has a stupid abstraction and doesn't expose its rest client. to fix this, replace dsharpplus with something else
	public async override Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId) => new DiscordMessage(this, await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong()));
	public async override Task<IChannel> GetChannelAsync(EntityId id) => new DiscordChannel(this, await m_DSharp.GetChannelAsync(id.Ulong()));
	public async override Task<IGuild> GetGuildAsync(EntityId id) => new DiscordGuild(this, await m_DSharp.GetGuildAsync(id.Ulong()));
	public async override Task<IUser> GetUserAsync(EntityId id) => new DiscordUser(this, await m_DSharp.GetUserAsync(id.Ulong()));
	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId guildUser) => new DiscordMember(this, await (await m_DSharp.GetGuildAsync(guildId.Ulong())).GetMemberAsync(guildId.Ulong()));
	public async override Task<IMessage> SendMessageAsync(EntityId channelId, string message) => new DiscordMessage(this, (await m_DSharp.SendMessageAsync(await m_DSharp.GetChannelAsync(channelId.Ulong()), message)));
	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string message) => new DiscordMessage(this, await (await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong())).ModifyAsync(message));
	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => await (await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong())).DeleteAsync();
}

internal static class EntityIdExtensions {
	public static ulong Ulong(this EntityId entityId) => (ulong) entityId.UnderlyingId;
}
