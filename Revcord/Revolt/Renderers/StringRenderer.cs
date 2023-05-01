using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt.Renderers;

public class StringRenderer : ChatClient.MessageRenderer<RevoltChatClient, string> {
	public StringRenderer(RevoltChatClient chatClient) : base(chatClient) { }
	
	protected async override Task<IMessage> SendMessageAsync(EntityId channelId, string contents, EntityId? responseTo) {
		MessageReply[]? messageReplies = responseTo == null ? null : new[] { new MessageReply() { id = responseTo.Value.String(), mention = false } };
		Message message = await ChatClient.Revolt.Rest.SendMessageAsync(channelId.String(), contents, replies: messageReplies!);

		return new RevoltMessage(ChatClient, message);
	}
	
	protected async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string contents) {
		Message message = await ChatClient.Revolt.Rest.EditMessageAsync(channelId.String(), messageId.String(), new Option<string>(contents));
		return new RevoltMessage(ChatClient, message);
	}
}
