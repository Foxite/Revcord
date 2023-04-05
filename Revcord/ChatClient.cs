using Revcord.Entities;

namespace Revcord;

public abstract class ChatClient {
	public event AsyncEventHandler<ChatClient, IMessage>? MessageCreated;
	public event AsyncEventHandler<ChatClient, IMessage>? MessageUpdated;
	public event AsyncEventHandler<ChatClient, EntityId>? MessageDeleted;
	
	public abstract IUser CurrentUser { get; }
	
	public abstract Task StartAsync();

	protected Task OnMessageCreated(IMessage message) => MessageCreated?.Invoke(this, message) ?? Task.CompletedTask;
	protected Task OnMessageUpdated(IMessage after) => MessageUpdated?.Invoke(this, after) ?? Task.CompletedTask;
	protected Task OnMessageDeleted(EntityId id) => MessageDeleted?.Invoke(this, id) ?? Task.CompletedTask;

	public abstract Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId);
	public abstract Task<IChannel> GetChannelAsync(EntityId id);
	public abstract Task<IGuild> GetGuildAsync(EntityId id);
	public abstract Task<IUser> GetUserAsync(EntityId id);
	public abstract Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId);
	// todo: get dm channel with user
	
	// todo: message builder
	public abstract Task<IMessage> SendMessageAsync(EntityId channelId, string message, EntityId? responseTo = null);
	public abstract Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string message);
	public abstract Task DeleteMessageAsync(EntityId channelId, EntityId messageId);
}
