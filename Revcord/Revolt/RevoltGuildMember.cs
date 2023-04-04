using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltGuildMember : IGuildMember {
	private readonly ServerMember m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public IUser User { get; }
	public EntityId UserId { get; }
	public IGuild Guild { get; }
	public EntityId GuildId { get; }
	public string? Nickname { get; }
	
	public RevoltGuildMember(RevoltChatClient client, ServerMember entity) {
		Client = client;
		m_Entity = entity;
	}
}