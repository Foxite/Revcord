using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltChannel : IChannel {
	private readonly Channel m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => EntityId.Of(m_Entity.Id);
	public string MentionString => $"<#{m_Entity.Id}>";
	public string Name => m_Entity switch {
		GroupChannel groupChannel => groupChannel.Name,
		TextChannel textChannel => textChannel.Name,
		UnknownServerChannel unknownServerChannel => unknownServerChannel.Name,
		VoiceChannel voiceChannel => voiceChannel.Name,
		ServerChannel serverChannel => serverChannel.Name,
		//UnknownChannel unknownChannel => ,
		_ => throw new ArgumentOutOfRangeException(nameof(m_Entity))
	};

	public RevoltChannel(RevoltChatClient client, Channel entity) {
		Client = client;
		m_Entity = entity;
	}
}