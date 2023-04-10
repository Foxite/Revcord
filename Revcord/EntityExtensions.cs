using Revcord.Entities;

namespace Revcord;

public static class EntityExtensions {
	public static Task<IMessage> SendMessageAsync(this ChatClient client, IChannel channel, MessageBuilder messageBuilder, EntityId? responseTo = null) => client.SendMessageAsync(channel.Id, messageBuilder, responseTo);
	public static Task<IMessage> SendMessageAsync(this ChatClient client, IChannel channel, string content, EntityId? responseTo = null) => client.SendMessageAsync(channel.Id, new MessageBuilder().WithContent(content), responseTo);
	
	public static Task<IMessage> UpdateMessageAsync(this ChatClient client, IMessage message, string content) => client.UpdateMessageAsync(message.ChannelId, message.Id, new MessageBuilder().WithContent(content));
	
	public static Task<IMessage> SendMessageAsync(this IChannel channel, MessageBuilder messageBuilder, EntityId? responseTo = null) => channel.Client.SendMessageAsync(channel, messageBuilder, responseTo);
	public static Task<IMessage> SendMessageAsync(this IChannel channel, string content, EntityId? responseTo = null) => channel.Client.SendMessageAsync(channel.Id, new MessageBuilder().WithContent(content), responseTo);
	
	public static Task<IMessage> SendReplyAsync(this IMessage message, MessageBuilder messageBuilder) => SendMessageAsync(message.Channel, messageBuilder, message.Id);
	public static Task<IMessage> SendReplyAsync(this IMessage message, string content) => SendMessageAsync(message.Channel, new MessageBuilder().WithContent(content), message.Id);

	public static Task<IMessage> UpdateAsync(this IMessage message, MessageBuilder messageBuilder) => message.Client.UpdateMessageAsync(message.ChannelId, message.Id, messageBuilder);
	public static Task<IMessage> UpdateAsync(this IMessage message, string content) => message.Client.UpdateMessageAsync(message.ChannelId, message.Id, new MessageBuilder().WithContent(content));

	public static Task AddReactionAsync(this IMessage message, IEmoji emoji) => message.Client.AddReactionAsync(message.ChannelId, message.Id, emoji);
	public static Task RemoveReactionAsync(this IMessage message, IEmoji emoji) => message.Client.RemoveReactionAsync(message.ChannelId, message.Id, emoji);
	
	public static Task DeleteAsync(this IMessage message) => message.Client.DeleteMessageAsync(message.ChannelId, message.Id);
	public static Task<IMessage> GetMessageAsync(this IChannel channel, EntityId messageId) => channel.Client.GetMessageAsync(channel.Id, messageId);
	public static Task<IGuildMember> GetMemberAsync(this IGuild guild, EntityId userId) => guild.Client.GetGuildMemberAsync(guild.Id, userId);
}
