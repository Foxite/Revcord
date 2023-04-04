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

	bool AuthorIsSelf { get; }
	DateTimeOffset CreationTimestamp { get; }
	IReadOnlyCollection<IReaction> Reactions { get; }
}