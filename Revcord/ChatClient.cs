using System.Reflection;
using Revcord.Entities;

namespace Revcord;

public abstract class ChatClient {
	public event AsyncEventHandler<MessageCreatedArgs>? MessageCreated;
	public event AsyncEventHandler<MessageUpdatedArgs>? MessageUpdated;
	public event AsyncEventHandler<MessageDeletedArgs>? MessageDeleted;
	public event AsyncEventHandler<ReactionModifiedArgs>? ReactionAdded;
	public event AsyncEventHandler<ReactionModifiedArgs>? ReactionRemoved;
	public event AsyncEventHandler<ClientErrorArgs>? ClientError;
	public event AsyncEventHandler<HandlerErrorArgs>? EventHandlerError;
	
	public abstract IUser CurrentUser { get; }
	
	public abstract Task StartAsync();

	private async Task HandleHandlerError<T>(AsyncEventHandler<T>? @event, string eventName, T args) where T : ChatClientEventArgs {
		try {
			if (@event != null) {
				await @event.Invoke(args);
			}
		} catch (Exception e) {
			if (EventHandlerError != null) {
				await EventHandlerError(new HandlerErrorArgs(this, eventName, e));
			}
		}
	}

	protected Task OnMessageCreated(IMessage message) => HandleHandlerError(MessageCreated, "MessageCreated", new MessageCreatedArgs(this, message));
	protected Task OnMessageUpdated(IMessage after) => HandleHandlerError(MessageUpdated, "MessageUpdated", new MessageUpdatedArgs(this, after));
	protected Task OnMessageDeleted(EntityId id) => HandleHandlerError(MessageDeleted, "MessageDeleted", new MessageDeletedArgs(this, id));
	
	protected Task OnReactionAdded(IMessage message, IEmoji emoji) => HandleHandlerError(ReactionAdded, "ReactionAdded", new ReactionModifiedArgs(this, message, emoji, true));
	protected Task OnReactionRemoved(IMessage message, IEmoji emoji) => HandleHandlerError(ReactionRemoved, "ReactionRemoved", new ReactionModifiedArgs(this, message, emoji, false));
	
	protected Task OnClientError(Exception exception) => ClientError?.Invoke(new ClientErrorArgs(this, new ChatClientException(this, "An exception was thrown by the underlying chat client", exception))) ?? Task.CompletedTask;

	public abstract Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId);
	public abstract Task<IChannel> GetChannelAsync(EntityId id);
	public abstract Task<IGuild> GetGuildAsync(EntityId id);
	public abstract Task<IUser> GetUserAsync(EntityId id);
	public abstract Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId);
	// todo: get dm channel with user
	
	public abstract Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo = null);
	public abstract Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder);
	public abstract Task DeleteMessageAsync(EntityId channelId, EntityId messageId);

	public abstract Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji);
	public abstract Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji);
}
