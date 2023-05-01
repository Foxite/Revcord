using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt.Renderers;

public class MessageBuilderRenderer : ChatClient.MessageRenderer<RevoltChatClient, MessageBuilder> {
	public MessageBuilderRenderer(RevoltChatClient chatClient) : base(chatClient) { }
	
	protected async override Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo) {
		Embed[]? embeds = messageBuilder.Embeds.Count == 0 ? null : messageBuilder.Embeds.Select(RevoltUtils.BuildEmbed).ToArray();
		MessageReply[]? messageReplies = responseTo == null ? null : new[] { new MessageReply() { id = responseTo.Value.String(), mention = false } };
		Message message = await ChatClient.Revolt.Rest.SendMessageAsync(channelId.String(), messageBuilder.Content, embeds: embeds!, replies: messageReplies!);

		return new RevoltMessage(ChatClient, message);
	}
	
	protected async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder) {
		Message message = await ChatClient.Revolt.Rest.EditMessageAsync(channelId.String(), messageId.String(), new Option<string>(messageBuilder.Content), new Option<Embed[]>(messageBuilder.Embeds.Select(RevoltUtils.BuildEmbed).ToArray()));
		return new RevoltMessage(ChatClient, message);
	}
}
