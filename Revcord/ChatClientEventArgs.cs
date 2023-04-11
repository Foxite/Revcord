using Revcord.Entities;

namespace Revcord;

public abstract record ChatClientEventArgs {
	public virtual string EventName {
		get {
			string typeName = GetType().Name;
			if (typeName.EndsWith("EventArgs")) {
				return typeName[..^"EventArgs".Length];
			} else if (typeName.EndsWith("Args")) {
				return typeName[..^"Args".Length];
			} else {
				return typeName;
			}
		}
	}
}

public record MessageCreatedArgs(
	ChatClient Client,
	IMessage Message
) : ChatClientEventArgs;

public record MessageUpdatedArgs(
	ChatClient Sender,
	IMessage Message
) : ChatClientEventArgs;

public record MessageDeletedArgs(
	ChatClient Sender,
	IChannel Channel,
	EntityId MessageId
) : ChatClientEventArgs;

public record ReactionModifiedArgs(
	ChatClient Client,
	IMessage Message,
	IEmoji Emoji,
	IGuildMember Member,
	bool Added
) : ChatClientEventArgs;

public record HandlerErrorArgs(
	ChatClient Sender,
	string EventName,
	Exception Exception
) : ChatClientEventArgs;

public record ClientErrorArgs(
	ChatClient Sender,
	ChatClientException Exception
) : ChatClientEventArgs;
