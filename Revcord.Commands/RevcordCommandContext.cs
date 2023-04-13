using Qmmands;
using Revcord.Entities;

namespace Revcord.Commands;

public class RevcordCommandContext : CommandContext {
	public IMessage Message { get; }
	public IChannel Channel { get; }
	public IUser User { get; }
	public IGuild? Guild { get; }
	public IGuildMember? Member { get; }
	public ChatClient Client { get; }
	
	public RevcordCommandContext(IMessage message, IServiceProvider serviceProvider) : base(serviceProvider) {
		Message = message;
		Channel = message.Channel;
		Guild = message.Guild;
		User = message.Author;
		Member = message.AuthorMember;
		Client = message.Client;
	}

	public Task<IMessage> RespondAsync(MessageBuilder messageBuilder) => Message.SendReplyAsync(messageBuilder);
	public Task<IMessage> RespondAsync(string message) => Message.SendReplyAsync(message);
}
