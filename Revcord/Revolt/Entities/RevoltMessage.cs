using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltMessage : IMessage {
	public Message Entity { get; }

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => EntityId.Of(Entity.Id);
	public EntityId? GuildId => Entity.Channel is ServerChannel sc ? EntityId.Of(sc.ServerId) : null;
	public EntityId ChannelId => EntityId.Of(Entity.ChannelId);
	public EntityId AuthorId => EntityId.Of(Entity.AuthorId);
	public IGuild? Guild => Entity.Channel is ServerChannel sc ? new RevoltGuild(Client, sc.Server) : null;
	public IChannel Channel => new RevoltChannel(Client, Entity.Channel);
	public IUser Author => new RevoltUser(Client, Entity.Author);
	public IGuildMember? AuthorMember => Entity.Channel is ServerChannel sc ? new RevoltGuildMember(Client, sc.Server.GetCachedMember(Entity.AuthorId), sc.Server) : null;
	public string? Content => (Entity as UserMessage)?.Content;
	public bool AuthorIsSelf => Entity.Author.Client.CurrentUser.Id == Entity.AuthorId;
	public DateTimeOffset CreationTimestamp => Ulid.Parse(Entity.Id).Time;
	public IReadOnlyCollection<IReaction> Reactions => Entity is UserMessage um ? um.Reactions.CollectionSelect(kvp => new RevoltReaction(Client, new RevoltEmoji(Client, kvp.Key), kvp.Value.Length)) : Array.Empty<IReaction>();
	
	public string JumpLink => $"{Client.FrontendUrl}/{(Entity.Channel is ServerChannel sc ? $"server/{sc.ServerId}/" : "")}channel/{Entity.Channel.Id}/{Entity.Id}";
	public bool IsSystemMessage => Entity is not UserMessage;

	public RevoltMessage(RevoltChatClient client, Message entity) {
		Client = client;
		Entity = entity;
	}
}
