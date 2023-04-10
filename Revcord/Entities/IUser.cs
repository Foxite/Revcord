namespace Revcord.Entities;

public interface IUser : IEntity {
	/// <summary>
	/// True if this user is the bot.
	/// </summary>
	bool IsSelf { get; }
	
	/// <summary>
	/// String to identify the user by to humans, but consider that it may be an <see cref="IGuildMember"/>.
	/// </summary>
	string DisplayName { get; }
	
	/// <summary>
	/// The username, without any discriminators used by the chat service.
	/// </summary>
	string Username { get; }
	
	/// <summary>
	/// The username including any discriminators.
	/// </summary>
	string DiscriminatedUsername { get; }
	
	/// <summary>
	/// A string that can be included in a message to ping the user.
	/// </summary>
	string MentionString { get; }

	/// <summary>
	/// Url to the user's avatar.
	/// </summary>
	string AvatarUrl { get; }
	
	bool IsBot { get; }
}
