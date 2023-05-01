using Revcord.Entities;

namespace Revcord;

public static class EntityExtensions {
	public static Task<IMessage> SendMessageAsync<T>(this ChatClient client, IChannel channel, T content, EntityId? responseTo = null) where T : notnull => client.SendMessageAsync(channel.Id, content, responseTo);
	public static Task<IMessage> SendMessageAsync<T>(this IChannel channel, T content, EntityId? responseTo = null) where T : notnull => EntityExtensions.SendMessageAsync(channel.Client, channel, content, responseTo);
	public static Task<IMessage> SendReplyAsync<T>(this IMessage message, T content) where T : notnull => EntityExtensions.SendMessageAsync(message.Client, message.Channel, content, message.Id);

	public static Task<IMessage> UpdateAsync<T>(this IMessage message, T content) where T : notnull => message.Client.UpdateMessageAsync(message.ChannelId, message.Id, content);

	public static Task AddReactionAsync(this IMessage message, IEmoji emoji) => message.Client.AddReactionAsync(message.ChannelId, message.Id, emoji);
	public static Task RemoveReactionAsync(this IMessage message, IEmoji emoji) => message.Client.RemoveReactionAsync(message.ChannelId, message.Id, emoji);
	
	public static Task DeleteAsync(this IMessage message) => message.Client.DeleteMessageAsync(message.ChannelId, message.Id);
	public static Task<IMessage> GetMessageAsync(this IChannel channel, EntityId messageId) => channel.Client.GetMessageAsync(channel.Id, messageId);
	public static Task<IGuildMember> GetMemberAsync(this IGuild guild, EntityId userId) => guild.Client.GetGuildMemberAsync(guild.Id, userId);
}
