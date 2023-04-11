namespace Revcord; 

public class ChatClientService {
	private readonly List<ChatClient> m_Clients = new();
	
	public event AsyncEventHandler<MessageCreatedArgs>? MessageCreated;
	public event AsyncEventHandler<MessageUpdatedArgs>? MessageUpdated;
	public event AsyncEventHandler<MessageDeletedArgs>? MessageDeleted;
	public event AsyncEventHandler<ReactionModifiedArgs>? ReactionAdded;
	public event AsyncEventHandler<ReactionModifiedArgs>? ReactionRemoved;
	public event AsyncEventHandler<ClientErrorArgs>? ClientError;
	public event AsyncEventHandler<HandlerErrorArgs>? EventHandlerError;

	public IReadOnlyCollection<ChatClient> Clients => m_Clients;

	public void AddClient(ChatClient client) {
		m_Clients.Add(client);
		
		client.MessageCreated    += args => MessageCreated   ?.Invoke(args) ?? Task.CompletedTask;
		client.MessageUpdated    += args => MessageUpdated   ?.Invoke(args) ?? Task.CompletedTask;
		client.MessageDeleted    += args => MessageDeleted   ?.Invoke(args) ?? Task.CompletedTask;
		client.ReactionAdded     += args => ReactionAdded    ?.Invoke(args) ?? Task.CompletedTask;
		client.ReactionRemoved   += args => ReactionRemoved  ?.Invoke(args) ?? Task.CompletedTask;
		client.ClientError       += args => ClientError      ?.Invoke(args) ?? Task.CompletedTask;
		client.EventHandlerError += args => EventHandlerError?.Invoke(args) ?? Task.CompletedTask;
	}

	public async Task StartAsync() {
		foreach (ChatClient client in Clients) {
			await client.StartAsync();
		}
	}
}
