namespace Revcord;

public class ChatClientException : Exception {
	public ChatClient Client { get; }
	
	public ChatClientException(ChatClient client, string message, Exception? inner = null) : base(message, inner) {
		Client = client;
	}
}

public class ChatConnectionException : ChatClientException {
	public ChatConnectionException(ChatClient client, string message) : base(client, message) { }
}

public class EntityNotFoundException : ChatClientException {
	public EntityNotFoundException(ChatClient client, Exception? inner) : base(client, "Entity was not found", inner) { }
}
