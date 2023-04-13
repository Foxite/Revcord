using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public partial class RevoltEmoji : IEmoji {
	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id { get; }
	public bool IsAnimated { get; }
	public string Name { get; }
	public bool IsCustomizedEmote { get; }
	
	public RevoltEmoji(RevoltChatClient client, Emoji entity) {
		Client = client;

		if (entity.IsServerEmoji) {
			Id = EntityId.Of(entity.Id);
			IsAnimated = entity.IsAnimated;
			Name = entity.Name;
			IsCustomizedEmote = true;
		} else {
			Id = EntityId.Of(entity.Id);
			IsAnimated = false;
			Name = UnicodeToEmojiNames[entity.Id];
			IsCustomizedEmote = false;
		}
	}

	public RevoltEmoji(RevoltChatClient client, string unicode) {
		Client = client;

		Id = EntityId.Of(unicode);
		IsAnimated = false;
		Name = UnicodeToEmojiNames[unicode];
		IsCustomizedEmote = false;
	}

	public bool Equals(IEmoji? other) {
		return other is RevoltEmoji otherRe
		       && IsCustomizedEmote == otherRe.IsCustomizedEmote
		       // TODO: test req about unicode emojis
		       // TODO: test req about variant names
		       && Name == otherRe.Name
		       && Id == otherRe.Id;
	}

	public override string ToString() {
		return $":{(IsCustomizedEmote ? Id.String() : Name)}:";
	}
}