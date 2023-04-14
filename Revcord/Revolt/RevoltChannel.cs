using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltChannel : IChannel {
	public Channel Entity { get; }

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => EntityId.Of(Entity.Id);
	public string MentionString => $"<#{Entity.Id}>";
	public string Name => Entity switch {
		GroupChannel groupChannel => groupChannel.Name,
		TextChannel textChannel => textChannel.Name,
		UnknownServerChannel unknownServerChannel => unknownServerChannel.Name,
		VoiceChannel voiceChannel => voiceChannel.Name,
		ServerChannel serverChannel => serverChannel.Name,
		//UnknownChannel unknownChannel => ,
		_ => throw new ArgumentOutOfRangeException(nameof(Entity))
	};

	public RevoltChannel(RevoltChatClient client, Channel entity) {
		Client = client;
		Entity = entity;
	}
}