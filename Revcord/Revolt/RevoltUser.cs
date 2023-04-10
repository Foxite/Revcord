using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltUser : IUser {
	private readonly User m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => new EntityId(m_Entity.Id);
	public bool IsSelf => m_Entity.Client.CurrentUser.Id == m_Entity.Id;
	public string DisplayName => m_Entity.Username;
	public string Username => m_Entity.Username;
	public string DiscriminatedUsername => m_Entity.Username;
	public string MentionString => $"<@{m_Entity.Id}>";
	public string AvatarUrl => Client.AutumnUrl + "/avatars/" + m_Entity.Avatar.Id;

	public RevoltUser(RevoltChatClient client, User entity) {
		Client = client;
		m_Entity = entity;
	}
}