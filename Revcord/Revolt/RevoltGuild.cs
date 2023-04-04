using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltGuild : IGuild {
	private readonly Server m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	
	public RevoltGuild(RevoltChatClient client, Server entity) {
		Client = client;
		m_Entity = entity;
	}

	public EntityId Id => new EntityId(m_Entity.Id);
	public string Name => m_Entity.Name;
	public IReadOnlyList<IChannelCategory> ChannelCategories => throw new NotImplementedException(); // todo: model only exposes channel id list
}