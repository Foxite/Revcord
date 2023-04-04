using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordEmoji : IEmoji {
	private readonly DSharpPlus.Entities.DiscordEmoji m_Entity;
	
	public ChatClient Client { get; }
	
	public DiscordEmoji(ChatClient chatClient, DSharpPlus.Entities.DiscordEmoji entity) {
		m_Entity = entity;
		Client = chatClient;
	}

	public bool IsAnimated => m_Entity.IsAnimated;
	public string Name => m_Entity.Name;
	public bool IsCustomizedEmote => m_Entity.Id == 0;

	public override string ToString() {
		if (m_Entity.Id == 0) {
			return m_Entity.Name;
		} else {
			return $"<{(m_Entity.IsAnimated ? "a" : "")}:emote:{m_Entity.Id}>";
		}
	}
	
	public bool Equals(DiscordEmoji? other) {
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return m_Entity.Equals(other.m_Entity);
	}

	public bool Equals(IEmoji? other) {
		return Equals(other as DiscordEmoji);
	}

	public override bool Equals(object? obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((DiscordEmoji) obj);
	}

	public override int GetHashCode() {
		return m_Entity.GetHashCode();
	}

	public static bool operator ==(DiscordEmoji? left, DiscordEmoji? right) {
		return Equals(left, right);
	}

	public static bool operator !=(DiscordEmoji? left, DiscordEmoji? right) {
		return !Equals(left, right);
	}
}