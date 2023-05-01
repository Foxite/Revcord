using DSharpPlus.Entities;
using Revcord.Entities;

namespace Revcord.Discord.Renderers;

public class StringRenderer : ChatClient.MessageRenderer<DiscordChatClient, string> {
	public StringRenderer(DiscordChatClient chatClient) : base(chatClient) { }
	
	protected async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string content) {
		DSharpPlus.Entities.DiscordChannel channel = await ChatClient.DSharp.GetChannelAsync(channelId.Ulong());
		DSharpPlus.Entities.DiscordMessage message = await channel.GetMessageAsync(messageId.Ulong());
		DSharpPlus.Entities.DiscordMessage updatedMessage = await message.ModifyAsync( new Optional<string>(content));
		return new DiscordMessage(ChatClient, updatedMessage);
	}

	protected async override Task<IMessage> SendMessageAsync(EntityId channelId, string content, EntityId? responseTo) {
		DSharpPlus.Entities.DiscordChannel channel = await ChatClient.DSharp.GetChannelAsync(channelId.Ulong());
		DSharpPlus.Entities.DiscordMessage message = await ChatClient.DSharp.SendMessageAsync(channel, content);
		return new DiscordMessage(ChatClient, message);
	}
}
