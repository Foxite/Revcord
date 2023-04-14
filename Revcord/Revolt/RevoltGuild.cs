using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltGuild : IGuild {
	public Server Entity { get; }

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	
	public RevoltGuild(RevoltChatClient client, Server entity) {
		Client = client;
		Entity = entity;
	}

	public EntityId Id => EntityId.Of(Entity.Id);
	public string Name => Entity.Name;
	public IReadOnlyList<IChannelCategory> ChannelCategories => throw new NotImplementedException(); // todo: model only exposes channel id list
}