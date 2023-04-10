namespace Revcord.Entities;

public interface IEmoji : IEntity, IEquatable<IEmoji> {
	bool IsAnimated { get; }
	
	string Name { get; }
	
	bool IsCustomizedEmote { get; }
	
	/// <summary>
	/// Requirements:
	/// - When this string is used in a message sent by the bot, it results in the correct emote, always.
	/// - This string is unique to that emote.
	/// - This string must always be the same for that emote.
	/// - It must work the same for unicode emojis and customized emotes. Callers must not be required to care about which it is.
	///
	/// The string does NOT need to be identical to the string "normally" used to represent this emote on the service, but see point 1.
	/// Implementations must account for, and eliminate any subtleties originating from the chat service.
	/// For example, Discord emotes may sometimes represent the same emote to the bot in different ways:
	/// - &lt;wholesome:1234&gt;
	/// - &lt;wholesome~1:1234&gt;
	/// When calling ToString, these differences must be eliminated. In particular, the value must be able to be stored in a database and later matched to another IEmoji object received from the chat service.
	/// A Discord implementation might replace the emote name entirely.
	/// </summary>
	string ToString();
}