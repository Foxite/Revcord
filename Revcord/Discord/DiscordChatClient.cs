using DSharpPlus;
using DSharpPlus.Entities;
using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordChatClient : ChatClient {
	private readonly DiscordClient m_DSharp;
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public override IUser CurrentUser => new DiscordUser(this, m_DSharp.CurrentUser);

	public DiscordChatClient(DiscordConfiguration configuration) {
		m_DSharp = new DiscordClient(configuration);

		m_DSharp.MessageCreated += (_, args) => OnMessageCreated(new DiscordMessage(this, args.Message));
		m_DSharp.MessageUpdated += (_, args) => OnMessageUpdated(new DiscordMessage(this, args.Message));
		m_DSharp.MessageDeleted += (_, args) => OnMessageDeleted(new EntityId(args.Message.Id));
		m_DSharp.MessageReactionAdded += (_, args) => OnReactionAdded(new DiscordMessage(this, args.Message), new DiscordEmoji(this, args.Emoji));
		m_DSharp.MessageReactionRemoved += (_, args) => OnReactionRemoved(new DiscordMessage(this, args.Message), new DiscordEmoji(this, args.Emoji));
		m_DSharp.ClientErrored += (_, args) => OnClientError(args.Exception);
		m_DSharp.SocketErrored += (_, args) => OnClientError(args.Exception);
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
	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId) => new DiscordMember(this, await (await m_DSharp.GetGuildAsync(guildId.Ulong())).GetMemberAsync(guildId.Ulong()));
	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder) => new DiscordMessage(this, await (await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong())).ModifyAsync(GetDiscordMessageBuilder(messageBuilder, null)));
	public async override Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo = null) => new DiscordMessage(this, (await m_DSharp.SendMessageAsync(await m_DSharp.GetChannelAsync(channelId.Ulong()), GetDiscordMessageBuilder(messageBuilder, responseTo))));
	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => await (await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong())).DeleteAsync();
	public async override Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) => await (await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong())).CreateReactionAsync(((DiscordEmoji) emoji).Entity);
	public async override Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) => await (await (await m_DSharp.GetChannelAsync(channelId.Ulong())).GetMessageAsync(messageId.Ulong())).DeleteOwnReactionAsync(((DiscordEmoji) emoji).Entity);

	private Action<DiscordMessageBuilder> GetDiscordMessageBuilder(MessageBuilder messageBuilder, EntityId? responseTo) {
		return dmb => {
			if (responseTo.HasValue) {
				dmb.WithReply(responseTo.Value.Ulong());
			}

			dmb.WithContent(messageBuilder.Content);

			foreach (EmbedBuilder embed in messageBuilder.Embeds) {
				dmb.AddEmbed(BuildEmbed(embed));
			}
		};
	}

	private static DiscordEmbed BuildEmbed(EmbedBuilder embed) {
		return new DiscordEmbedBuilder() {
			Title = embed.Title,
			Description = embed.Description,
			Color = new DiscordColor(embed.Color.R, embed.Color.G, embed.Color.B),
			ImageUrl = embed.ImageUrl,
			Url = embed.Url,
			Author = new DiscordEmbedBuilder.EmbedAuthor() {
				IconUrl = embed.IconUrl
			}
		}.Build();
	}
}

internal static class EntityIdExtensions {
	public static ulong Ulong(this EntityId entityId) => (ulong) entityId.UnderlyingId;
}
