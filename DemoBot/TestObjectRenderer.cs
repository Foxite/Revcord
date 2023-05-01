using Revcord;
using Revcord.Entities;

namespace DemoBot; 

public class TestObjectRenderer : ChatClient.MessageRenderer<ChatClient, TestObject> {
	public TestObjectRenderer(ChatClient client) : base(client) { }
	
	protected override Task<IMessage> SendMessageAsync(EntityId channelId, TestObject obj, EntityId? responseTo) {
		return ChatClient.SendMessageAsync(channelId, ToMessageBuilder(obj), responseTo);
	}
	
	protected override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, TestObject obj) {
		return ChatClient.UpdateMessageAsync(channelId, messageId, ToMessageBuilder(obj));
	}
	
	private MessageBuilder ToMessageBuilder(TestObject obj) {
		return new MessageBuilder()
			.WithContent("Test object!")
			.AddEmbed(
				new EmbedBuilder()
					.WithTitle($"Hey: {obj.Hey}!")
					.WithDescription($"Bla: {obj.Bla}!")
			);
	}
}