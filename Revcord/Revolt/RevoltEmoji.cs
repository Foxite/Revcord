using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltEmoji : IEmoji {
	private readonly Emoji m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => new EntityId(m_Entity.Id);
	public bool IsAnimated => m_Entity.IsAnimated;
	public string Name => m_Entity.Name; // TODO: test req about unicode emojis
	public bool IsCustomizedEmote => m_Entity.IsServerEmoji;
	
	public RevoltEmoji(RevoltChatClient client, Emoji entity) {
		Client = client;
		m_Entity = entity;
	}

	public bool Equals(IEmoji? other) {
		return other is RevoltEmoji otherRe
		       && m_Entity.IsServerEmoji == otherRe.m_Entity.IsServerEmoji
		       // TODO: test req about unicode emojis
		       // TODO: test req about variant names
		       && m_Entity.Name == otherRe.m_Entity.Name
		       && m_Entity.Id == otherRe.m_Entity.Id;
	}

	public override string ToString() {
		// TODO test unicode emojis
		return $":{m_Entity.Id}:";
	}
}