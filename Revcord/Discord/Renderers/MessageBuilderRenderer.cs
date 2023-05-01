using Revcord.Entities;
using SharpChannel = DSharpPlus.Entities.DiscordChannel;
using SharpMessage = DSharpPlus.Entities.DiscordMessage;

namespace Revcord.Discord.Renderers;

public class MessageBuilderRenderer : ChatClient.MessageRenderer<DiscordChatClient, MessageBuilder> {
	public MessageBuilderRenderer(DiscordChatClient chatClient) : base(chatClient) { }
	
	protected async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder) {
		SharpChannel channel = await ChatClient.DSharp.GetChannelAsync(channelId.Ulong());
		SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
		SharpMessage updatedMessage = await message.ModifyAsync(DiscordUtils.GetDiscordMessageBuilder(messageBuilder, null));
		return new DiscordMessage(ChatClient, updatedMessage);
	}

	protected async override Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo) {
		SharpChannel channel = await ChatClient.DSharp.GetChannelAsync(channelId.Ulong());
		SharpMessage message = await ChatClient.DSharp.SendMessageAsync(channel, DiscordUtils.GetDiscordMessageBuilder(messageBuilder, responseTo));
		return new DiscordMessage(ChatClient, message);
	}
}
