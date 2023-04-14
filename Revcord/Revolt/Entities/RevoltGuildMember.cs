using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltGuildMember : IGuildMember {
	private readonly Server m_Server;
	
	public ServerMember Entity { get; }

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public IUser User => new RevoltUser(Client, Entity.User);
	public EntityId UserId => EntityId.Of(Entity.Id);
	public IGuild Guild => new RevoltGuild(Client, m_Server);
	public EntityId GuildId => EntityId.Of(m_Server.Id);
	public string? Nickname => Entity.Nickname;
	
	public RevoltGuildMember(RevoltChatClient client, ServerMember entity, Server server) {
		Client = client;
		Entity = entity;
		m_Server = server;
	}
}