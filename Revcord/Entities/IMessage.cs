namespace Revcord.Entities;

public interface IMessage : IEntity {
	IGuild? Guild { get; }
	EntityId? GuildId { get; }
	IChannel Channel { get; }
	EntityId ChannelId { get; }
	IUser Author { get; }
	IGuildMember? AuthorMember { get; }
	EntityId AuthorId { get; }
	
	string? Content { get; }
	
	// TODO embed
	// TODO attachments
	// todo: type

	bool AuthorIsSelf => Author.IsSelf;
	DateTimeOffset CreationTimestamp { get; }
	IReadOnlyCollection<IReaction> Reactions { get; }
	
	/// <summary>
	/// A link that can be sent to a user. When the user clicks the link, they will be taken to the message.
	/// </summary>
	string JumpLink { get; }
	
	bool IsSystemMessage { get; }
}
