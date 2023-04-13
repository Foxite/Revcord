using Revcord.Entities;
using RevoltSharp;
using EmbedBuilderSharp = RevoltSharp.EmbedBuilder;

namespace Revcord.Revolt;

public class RevoltChatClient : ChatClient {
	public RevoltClient Revolt { get; }
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public override IUser CurrentUser => new RevoltUser(this, Revolt.CurrentUser);
	public string FrontendUrl { get; }
	public string AutumnUrl { get; }

	public RevoltChatClient(string token, ClientConfig? config = null, string frontendUrl = "https://app.revolt.chat", string autumnUrl = "https://autumn.revolt.chat") {
		Revolt = new RevoltClient(token, ClientMode.WebSocket, config!);
		FrontendUrl = frontendUrl;
		AutumnUrl = autumnUrl;

		Revolt.OnMessageRecieved += message => OnMessageCreated(new RevoltMessage(this, message));
		Revolt.OnMessageUpdated += async (channel, messageId, content) => await OnMessageUpdated(await GetMessageAsync(EntityId.Of(channel), EntityId.Of(messageId)));
		Revolt.OnMessageDeleted += (channel, id) => OnMessageDeleted(new RevoltChannel(this, channel), EntityId.Of(id)); // parameter names inferred

		Revolt.OnReactionAdded += async (emoji, channel, member, message) => await OnReactionAdded(new RevoltMessage(this, await message.DownloadAsync()), new RevoltEmoji(this, emoji), new RevoltGuildMember(this, await member.DownloadAsync(), channel.Server));
		Revolt.OnReactionRemoved += async (emoji, channel, member, message) => await OnReactionRemoved(new RevoltMessage(this, await message.DownloadAsync()), new RevoltEmoji(this, emoji), new RevoltGuildMember(this, await member.DownloadAsync(), channel.Server));

		Revolt.OnWebSocketError += error => OnClientError(new RevoltException(error.Messaage, (int) error.Type));
	}

	public async override Task StartAsync() {
		Revolt.OnReady += _ => {
			m_ReadyTcs.TrySetResult();
		};

		Revolt.OnWebSocketError += error => {
			m_ReadyTcs.TrySetException(new ChatConnectionException(this, error.Messaage));
		};
		
		await Revolt.StartAsync();
		await m_ReadyTcs.Task;
	}

	public async override Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId) {
		Message message;
		try {
			message = await Revolt.Rest.GetMessageAsync(channelId.ToString(), messageId.String());
		} catch (NullReferenceException) {
			throw new EntityNotFoundException(this, null);
		}

		return new RevoltMessage(this, message);
	}

	public override Task<IChannel> GetChannelAsync(EntityId id) {
		Channel? channel = Revolt.GetChannel(id.String());
		if (channel == null) {
			throw new EntityNotFoundException(this, null);
		}
		
		return Task.FromResult<IChannel>(new RevoltChannel(this, channel));
	}

	public override Task<IGuild> GetGuildAsync(EntityId id) {
		Server? server = Revolt.GetServer(id.String());
		if (server == null) {
			throw new EntityNotFoundException(this, null);
		}
		
		return Task.FromResult<IGuild>(new RevoltGuild(this, server));
	}

	public override Task<IUser> GetUserAsync(EntityId id) {
		User user = Revolt.GetUser(id.String());
		if (user == null) {
			throw new EntityNotFoundException(this, null);
		}
		
		return Task.FromResult<IUser>(new RevoltUser(this, user));
	}

	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId) {
		ServerMember member;
		Server server;
		try {
			member = await Revolt.Rest.GetMemberAsync(guildId.String(), userId.String());
			server = await Revolt.Rest.GetServerAsync(guildId.String());
		} catch (NullReferenceException) {
			throw new EntityNotFoundException(this, null);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}

		return new RevoltGuildMember(this, member, server);
	}

	public async override Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo = null) {
		Embed[]? embeds = messageBuilder.Embeds.Count == 0 ? null : messageBuilder.Embeds.Select(BuildEmbed).ToArray();
		MessageReply[]? messageReplies = responseTo == null ? null : new[] { new MessageReply() { id = responseTo.Value.String(), mention = false } };
		Message message;
		try {
			message = await Revolt.Rest.SendMessageAsync(channelId.String(), messageBuilder.Content, embeds: embeds!, replies: messageReplies!);
		} catch (RevoltRestException ex) when (ex is { Code: 404 }) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}

		return new RevoltMessage(this, message);
	}

	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder) {
		Message message;
		try {
			message = await Revolt.Rest.EditMessageAsync(channelId.String(), messageId.String(), new Option<string>(messageBuilder.Content), new Option<Embed[]>(messageBuilder.Embeds.Select(BuildEmbed).ToArray()));
		} catch (RevoltRestException ex) when (ex is { Code: 404 }) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}

		return new RevoltMessage(this, message);
	}

	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) {
		try {
			await Revolt.Rest.DeleteMessageAsync(channelId.String(), messageId.String());
		} catch (RevoltRestException ex) when (ex is { Code: 404 }) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}
	}

	public override Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) {
		try {
			return Revolt.Rest.AddMessageReactionAsync(channelId.String(), messageId.String(), emoji.Id.String());
		} catch (RevoltRestException ex) when (ex is { Code: 404 }) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}
	}

	public override Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) {
		try {
			return Revolt.Rest.RemoveMessageReactionAsync(channelId.String(), messageId.String(), Revolt.CurrentUser.Id, emoji.Id.String());
		} catch (RevoltRestException ex) when (ex is {Code: 404}) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}
	}

	private static Embed BuildEmbed(EmbedBuilder embedBuilder) {
		return new EmbedBuilderSharp() {
			Title = embedBuilder.Title,
			Description = embedBuilder.Description,
			Color = new RevoltColor(embedBuilder.Color.R, embedBuilder.Color.G, embedBuilder.Color.B),
			IconUrl = embedBuilder.IconUrl,
			Image = embedBuilder.ImageUrl,
			Url = embedBuilder.Url
		}.Build();
	}
}
