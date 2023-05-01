using Foxite.Text;
using Foxite.Text.Parsers;
using Revcord.Entities;

namespace Revcord;

// TODO a mock implementation for unit testing
// Also: NUnit support
public abstract class ChatClient {
	public event AsyncEventHandler<MessageCreatedArgs>? MessageCreated;
	public event AsyncEventHandler<MessageUpdatedArgs>? MessageUpdated;
	public event AsyncEventHandler<MessageDeletedArgs>? MessageDeleted;
	public event AsyncEventHandler<ReactionModifiedArgs>? ReactionAdded;
	public event AsyncEventHandler<ReactionModifiedArgs>? ReactionRemoved;
	public event AsyncEventHandler<ClientErrorArgs>? ClientError;
	public event AsyncEventHandler<HandlerErrorArgs>? EventHandlerError;
	
	protected Dictionary<Type, IMessageRenderer> Renderers { get; } = new();

	public abstract IUser CurrentUser { get; }
	public abstract ITextFormatter TextFormatter { get; }
	public abstract Parser TextParser { get; }
	
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
	protected Task OnMessageDeleted(IChannel channel, EntityId id) => HandleHandlerError(MessageDeleted, "MessageDeleted", new MessageDeletedArgs(this, channel, id));
	
	protected Task OnReactionAdded(IMessage message, IEmoji emoji, IGuildMember member) => HandleHandlerError(ReactionAdded, "ReactionAdded", new ReactionModifiedArgs(this, message, emoji, member, true));
	protected Task OnReactionRemoved(IMessage message, IEmoji emoji, IGuildMember member) => HandleHandlerError(ReactionRemoved, "ReactionRemoved", new ReactionModifiedArgs(this, message, emoji, member, false));
	
	protected Task OnClientError(Exception exception) => HandleHandlerError(ClientError, "ClientError", new ClientErrorArgs(this, exception));

	protected IMessageRenderer GetRenderer(object obj) {
		return Renderers[obj.GetType()];
	}

	public void AddRenderer<TChatClient, TMessage>(MessageRenderer<TChatClient, TMessage> renderer) where TChatClient : ChatClient {
		Renderers[typeof(TMessage)] = renderer;
	}

	public abstract Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId);
	public abstract Task<IChannel> GetChannelAsync(EntityId id);
	public abstract Task<IGuild> GetGuildAsync(EntityId id);
	public abstract Task<IUser> GetUserAsync(EntityId id);
	public abstract Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId);
	// todo: get dm channel with user

	public virtual Task<IMessage> SendMessageAsync<T>(EntityId channelId, T content, EntityId? responseTo = null) where T : notnull => GetRenderer(content).SendMessageAsync(channelId, content, responseTo);
	public virtual Task<IMessage> UpdateMessageAsync<T>(EntityId channelId, EntityId messageId, T content) where T : notnull => GetRenderer(content).UpdateMessageAsync(channelId, messageId, content);
	public abstract Task DeleteMessageAsync(EntityId channelId, EntityId messageId);

	public abstract Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji);
	public abstract Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji);
	
	protected interface IMessageRenderer {
		Task<IMessage> SendMessageAsync(EntityId channelId, object contents, EntityId? responseTo);
		Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, object contents);
	}
	
	protected interface IMessageRenderer<in T> {
		Task<IMessage> SendMessageAsync(EntityId channelId, T contents, EntityId? responseTo);
		Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, T contents);
	}

	public abstract class MessageRenderer<TChatClient, TMessage> : IMessageRenderer<TMessage>, IMessageRenderer where TChatClient : ChatClient {
		protected TChatClient ChatClient { get; }
		
		protected MessageRenderer(TChatClient chatClient) {
			ChatClient = chatClient;
		}
		
		Task<IMessage> IMessageRenderer.SendMessageAsync(EntityId channelId, object contents, EntityId? responseTo) => this.SendMessageAsync(channelId, (TMessage) contents, responseTo);
		Task<IMessage> IMessageRenderer.UpdateMessageAsync(EntityId channelId, EntityId messageId, object contents) => this.UpdateMessageAsync(channelId, messageId, (TMessage) contents);

		Task<IMessage> IMessageRenderer<TMessage>.SendMessageAsync(EntityId channelId, TMessage contents, EntityId? responseTo) => this.SendMessageAsync(channelId, contents, responseTo);
		Task<IMessage> IMessageRenderer<TMessage>.UpdateMessageAsync(EntityId channelId, EntityId messageId, TMessage contents) => this.UpdateMessageAsync(channelId, messageId, contents);

		protected abstract Task<IMessage> SendMessageAsync(EntityId channelId, TMessage contents, EntityId? responseTo);
		protected abstract Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, TMessage contents);
	}
}
