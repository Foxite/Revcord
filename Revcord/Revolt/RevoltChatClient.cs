using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt; 

public class RevoltChatClient : ChatClient {
	public RevoltClient Revolt { get; }
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public override IUser CurrentUser => new RevoltUser(this, Revolt.CurrentUser);
	public string FrontendUrl { get; }

	public RevoltChatClient(string token, ClientConfig? config = null, string frontendUrl = "https://app.revolt.chat") {
		Revolt = new RevoltClient(token, ClientMode.WebSocket, config!);
		FrontendUrl = frontendUrl;

		Revolt.OnMessageRecieved += message => OnMessageCreated(new RevoltMessage(this, message));
		Revolt.OnMessageUpdated += async (channel, messageId, content) => await OnMessageUpdated(await GetMessageAsync(new EntityId(channel), new EntityId(messageId)));
		Revolt.OnMessageDeleted += (channel, id) => OnMessageDeleted(new EntityId(id)); // parameter names inferred
	}

	public async override Task StartAsync() {
		Revolt.OnReady += _ => {
			m_ReadyTcs.TrySetResult();
		};

		Revolt.OnWebSocketError += error => {
			m_ReadyTcs.TrySetException(new ChatConnectionException(error.Messaage));
		};
		
		await Revolt.StartAsync();
		await m_ReadyTcs.Task;
	}

	public async override Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId) => new RevoltMessage(this, await Revolt.Rest.GetMessageAsync(channelId.ToString(), messageId.String()));
	public override Task<IChannel> GetChannelAsync(EntityId id) => Task.FromResult<IChannel>(new RevoltChannel(this, Revolt.GetChannel(id.String())));
	public override Task<IGuild> GetGuildAsync(EntityId id) => Task.FromResult<IGuild>(new RevoltGuild(this, Revolt.GetServer(id.String())));
	public override Task<IUser> GetUserAsync(EntityId id) => Task.FromResult<IUser>(new RevoltUser(this, Revolt.GetUser(id.String())));
	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId guildUser) => new RevoltGuildMember(this, await Revolt.Rest.GetMemberAsync(guildId.String(), guildUser.String()));
	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string message) => new RevoltMessage(this, await Revolt.Rest.EditMessageAsync(channelId.String(), messageId.String(), new Option<string>(message)));
	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => await Revolt.Rest.DeleteMessageAsync(channelId.String(), messageId.String());
	public async override Task<IMessage> SendMessageAsync(EntityId channelId, string message, EntityId? responseTo = null) => new RevoltMessage(this, await Revolt.Rest.SendMessageAsync(channelId.String(), message)); // TODO replies (waiting on https://github.com/xXBuilderBXx/RevoltSharp/pull/8)
}