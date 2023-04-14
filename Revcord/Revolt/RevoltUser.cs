using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltUser : IUser {
	public User Entity { get; }

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => EntityId.Of(Entity.Id);
	public bool IsSelf => Entity.Client.CurrentUser.Id == Entity.Id;
	public string DisplayName => Entity.Username;
	public string Username => Entity.Username;
	public string DiscriminatedUsername => Entity.Username;
	public string MentionString => $"<@{Entity.Id}>";
	public string AvatarUrl => Client.AutumnUrl + "/avatars/" + Entity.Avatar.Id;
	public bool IsBot => Entity.IsBot;

	public RevoltUser(RevoltChatClient client, User entity) {
		Client = client;
		Entity = entity;
	}
}