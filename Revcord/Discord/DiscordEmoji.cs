using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordEmoji : IEmoji {
	public DSharpPlus.Entities.DiscordEmoji Entity { get; }
	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(Entity.Id);
	public bool IsAnimated => Entity.IsAnimated;
	public string Name => Entity.Name;
	public bool IsCustomizedEmote => Entity.Id == 0;
	
	public DiscordEmoji(ChatClient chatClient, DSharpPlus.Entities.DiscordEmoji entity) {
		Entity = entity;
		Client = chatClient;
	}


	public override string ToString() {
		if (Entity.Id == 0) {
			return Entity.Name;
		} else {
			return $"<{(Entity.IsAnimated ? "a" : "")}:emote:{Entity.Id}>";
		}
	}
	
	public bool Equals(DiscordEmoji? other) {
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Entity.Equals(other.Entity);
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
		return Entity.GetHashCode();
	}

	public static bool operator ==(DiscordEmoji? left, DiscordEmoji? right) {
		return Equals(left, right);
	}

	public static bool operator !=(DiscordEmoji? left, DiscordEmoji? right) {
		return !Equals(left, right);
	}
}