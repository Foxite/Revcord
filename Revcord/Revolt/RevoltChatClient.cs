using Foxite.Text;
using Foxite.Text.Parsers;
using Revcord.Entities;
using Revcord.Revolt.Renderers;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltChatClient : ChatClient {
	public RevoltClient Revolt { get; }
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public override IUser CurrentUser => new RevoltUser(this, Revolt.CurrentUser);
	
	public override ITextFormatter TextFormatter => ModularTextFormatter.Markdown();
	public override Parser TextParser => new MarkdownParser();
	
	public string FrontendUrl { get; }
	public string AutumnUrl { get; }

	public RevoltChatClient(string token, ClientConfig? config = null, string frontendUrl = "https://app.revolt.chat", string autumnUrl = "https://autumn.revolt.chat") {
		Revolt = new RevoltClient(token, ClientMode.WebSocket, config!);
		FrontendUrl = frontendUrl;
		AutumnUrl = autumnUrl;

		Revolt.OnMessageRecieved += message => OnMessageCreated(new RevoltMessage(this, message));
		Revolt.OnMessageUpdated += async (channel, messageId, content) => await OnMessageUpdated(await GetMessageAsync(EntityId.Of(channel.Id), EntityId.Of(messageId)));
		Revolt.OnMessageDeleted += (channel, id) => OnMessageDeleted(new RevoltChannel(this, channel), EntityId.Of(id)); // parameter names inferred

		Revolt.OnReactionAdded += async (emoji, channel, member, message) => await OnReactionAdded(new RevoltMessage(this, await message.DownloadAsync()), new RevoltEmoji(this, emoji), new RevoltGuildMember(this, await member.DownloadAsync(), channel.Server));
		Revolt.OnReactionRemoved += async (emoji, channel, member, message) => await OnReactionRemoved(new RevoltMessage(this, await message.DownloadAsync()), new RevoltEmoji(this, emoji), new RevoltGuildMember(this, await member.DownloadAsync(), channel.Server));

		Revolt.OnWebSocketError += error => OnClientError(new RevoltException(error.Messaage, (int) error.Type));

		AddRenderer(new StringRenderer(this));
		AddRenderer(new MessageBuilderRenderer(this));
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
			message = await Revolt.Rest.GetMessageAsync(channelId.String(), messageId.String());
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

	public override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId) {
		return CallRest(async () => {
			ServerMember member = await Revolt.Rest.GetMemberAsync(guildId.String(), userId.String());
			Server server = await Revolt.Rest.GetServerAsync(guildId.String());
			return (IGuildMember) new RevoltGuildMember(this, member, server);
		});
	}

	public override Task<IMessage> SendMessageAsync<T>(EntityId channelId, T content, EntityId? responseTo = null) => CallRest(() => base.SendMessageAsync(channelId, content, responseTo));
	public override Task<IMessage> UpdateMessageAsync<T>(EntityId channelId, EntityId messageId, T content) => CallRest(() => base.UpdateMessageAsync(channelId, messageId, content));
	public override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => CallRest(() => Revolt.Rest.DeleteMessageAsync(channelId.String(), messageId.String()));
	public override Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) => CallRest(() => Revolt.Rest.AddMessageReactionAsync(channelId.String(), messageId.String(), emoji.Id.String()));
	public override Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) => CallRest(() => Revolt.Rest.RemoveMessageReactionAsync(channelId.String(), messageId.String(), Revolt.CurrentUser.Id, emoji.Id.String()));

	private Task<T> CallRest<T>(Func<Task<T>> func) {
		try {
			return func();
		} catch (RevoltRestException ex) when (ex is {Code: 404}) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}
	}

	private Task CallRest(Func<Task> func) {
		try {
			return func();
		} catch (RevoltRestException ex) when (ex is {Code: 404}) {
			throw new EntityNotFoundException(this, ex);
		} catch (RevoltRestException ex) {
			throw new ChatClientException(this, "RevoltSharp threw an exception", ex);
		}
	}
}
