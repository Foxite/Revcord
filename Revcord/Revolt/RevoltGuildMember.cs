using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltGuildMember : IGuildMember {
	private readonly ServerMember m_Entity;
	private readonly Server m_Server;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public IUser User => new RevoltUser(Client, m_Entity.User);
	public EntityId UserId => new EntityId(m_Entity.Id);
	public IGuild Guild => new RevoltGuild(Client, m_Server);
	public EntityId GuildId => new EntityId(m_Server.Id);
	public string? Nickname => m_Entity.Nickname;
	
	public RevoltGuildMember(RevoltChatClient client, ServerMember entity, Server server) {
		Client = client;
		m_Entity = entity;
		m_Server = server;
	}
}