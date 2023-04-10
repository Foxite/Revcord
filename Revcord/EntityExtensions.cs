using Revcord.Entities;

namespace Revcord;

public static class EntityExtensions {
	public static Task<IMessage> SendMessageAsync(this IChannel channel, string content, EntityId? responseTo = null) => channel.Client.SendMessageAsync(channel.Id, content, responseTo);
	public static Task<IMessage> SendReplyAsync(this IMessage message, string content) => SendMessageAsync(message.Channel, content, message.Id);
	public static Task<IMessage> UpdateAsync(this IMessage message, string content, EntityId? responseTo = null) => message.Client.UpdateMessageAsync(message.ChannelId, message.Id, content);
	public static Task DeleteAsync(this IMessage message) => message.Client.DeleteMessageAsync(message.ChannelId, message.Id);
	public static Task<IMessage> GetMessageAsync(this IChannel channel, EntityId messageId) => channel.Client.GetMessageAsync(channel.Id, messageId);
	public static Task<IGuildMember> GetMemberAsync(this IGuild guild, EntityId userId) => guild.Client.GetGuildMemberAsync(guild.Id, userId);
}
