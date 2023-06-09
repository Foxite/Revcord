using System.Text.RegularExpressions;
using Qmmands;
using Revcord.Discord;
using Revcord.Entities;
using Revcord.Revolt;

namespace Revcord.Commands;

/// <summary>
/// Do not implement this interface, implement the generic version <see cref="ICommandSupport{T}"/> instead. You should preferably extend from the abstract class <see cref="CommandSupport{T}"/>.
/// This interface exists to that this library can call Install without needing to know the generic parameters at compile time.
/// The generic version exists so that this library can retrieve an object for a specific ChatClient type, which it knows at runtime.
/// </summary>
public interface ICommandSupport {
	void Install(CommandService commands, ChatClient client, IServiceProvider isp);
}

// TODO T should be contravariant?
public interface ICommandSupport<T> : ICommandSupport where T : ChatClient {
	void Install(CommandService commands, T client, IServiceProvider isp);
}

public abstract class CommandSupport<T> : ICommandSupport<T> where T : ChatClient {
	void ICommandSupport.Install(CommandService commands, ChatClient client, IServiceProvider isp) => Install(commands, (T) client, isp);
	public virtual void Install(CommandService commands, T client, IServiceProvider isp) {
		void TryAddParser<TChatObject>(ChatServiceObjectTypeParser<TChatObject> csotp, RevcordTypeParser<TChatObject>? rtp) where TChatObject : IChatServiceObject {
			if (rtp != null) {
				csotp.Add(client.GetType(), rtp);
			}
		}

		TryAddParser((ChatServiceObjectTypeParser<IUser>   ) commands.GetTypeParser<IUser>(),    GetUserParser());
		TryAddParser((ChatServiceObjectTypeParser<IChannel>) commands.GetTypeParser<IChannel>(), GetChannelParser());
		TryAddParser((ChatServiceObjectTypeParser<IEmoji>  ) commands.GetTypeParser<IEmoji>(),   GetEmojiParser());
	}

	protected abstract RevcordTypeParser<IUser>?    GetUserParser();
	protected abstract RevcordTypeParser<IChannel>? GetChannelParser();
	protected abstract RevcordTypeParser<IEmoji>?   GetEmojiParser();
}

public class RevoltCommandSupport : CommandSupport<RevoltChatClient> {
	protected override RevcordTypeParser<IUser>    GetUserParser()    => new RegexTypeParser<IUser>(new Regex(@"<@(?<Id>[0-9A-Z]+)>"), (context, match) => context.Client.GetUserAsync(EntityId.Of(match.Groups["Id"].Value)), "That is not a valid user mention.");
	protected override RevcordTypeParser<IChannel> GetChannelParser() => new RegexTypeParser<IChannel>(new Regex(@"<#(?<Id>[0-9A-Z]+)>"), (context, match) => context.Client.GetChannelAsync(EntityId.Of(match.Groups["Id"].Value)), "That is not a valid channel mention.");
	protected override RevcordTypeParser<IEmoji>   GetEmojiParser()   => new RevoltEmojiTypeParser();
}

public class DiscordCommandSupport : CommandSupport<DiscordChatClient> {
	protected override RevcordTypeParser<IUser>    GetUserParser()    => new RegexTypeParser<IUser>(new Regex(@"<@(?<Id>[0-9]+)>"), (context, match) => context.Client.GetUserAsync(EntityId.Of(ulong.Parse(match.Groups["Id"].Value))), "That is not a valid user mention.");
	protected override RevcordTypeParser<IChannel> GetChannelParser() => new RegexTypeParser<IChannel>(new Regex(@"<#(?<Id>[0-9]+)>"), (context, match) => context.Client.GetChannelAsync(EntityId.Of(ulong.Parse(match.Groups["Id"].Value))), "That is not a valid channel mention.");
	protected override RevcordTypeParser<IEmoji>   GetEmojiParser()   => new DiscordEmojiTypeParser();
}
