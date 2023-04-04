namespace Revcord.Entities;

public interface IGuildMember : IChatServiceObject {
	IUser User { get; }
	EntityId UserId { get; }
	IGuild Guild { get; }
	EntityId GuildId { get; }
	
	string? Nickname { get; }
	
	// todo: roles
	// todo: avatar
}