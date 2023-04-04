using Revcord.Entities;
using RevoltSharp;
using RevoltSharpMessage      = RevoltSharp.Message;
using RevoltSharpChannel      = RevoltSharp.Channel;
using RevoltSharpServer       = RevoltSharp.Server;
using RevoltSharpUser         = RevoltSharp.User;
using RevoltSharpServerMember = RevoltSharp.ServerMember;
using RevoltSharpEmoji        = RevoltSharp.Emoji;

namespace Revcord.Revolt; 

public class RevoltChatClient : ChatClient {
	public RevoltClient Revolt { get; }
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public RevoltChatClient(string token) {
		Revolt = new RevoltClient(token, ClientMode.WebSocket);

		Revolt.OnMessageRecieved += message => OnMessageCreated(new RevoltMessage(this, message));
		Revolt.OnMessageUpdated += async (channel, messageId, content) => await OnMessageUpdated(await GetMessageAsync(new EntityId(channel), new EntityId(messageId)));
		Revolt.OnMessageDeleted += (channel, id) => OnMessageDeleted(new EntityId(id)); // parameter names inferred
	}
	
	public async override Task StartAsync() {
		Revolt.OnReady += _ => {
			m_ReadyTcs.TrySetResult();
		};

		Revolt.OnWebSocketError += error => {
			m_ReadyTcs.TrySetException(new ChatConnectionException(error.Messaage));
		};
		
		await Revolt.StartAsync();
		await m_ReadyTcs.Task;
	}

	public async override Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId) => new RevoltMessage(this, await Revolt.Rest.GetMessageAsync(channelId.ToString(), messageId.String()));
	public override Task<IChannel> GetChannelAsync(EntityId id) => Task.FromResult<IChannel>(new RevoltChannel(this, Revolt.GetChannel(id.String())));
	public override Task<IGuild> GetGuildAsync(EntityId id) => Task.FromResult<IGuild>(new RevoltGuild(this, Revolt.GetServer(id.String())));
	public override Task<IUser> GetUserAsync(EntityId id) => Task.FromResult<IUser>(new RevoltUser(this, Revolt.GetUser(id.String())));
	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId guildUser) => new RevoltGuildMember(this, await Revolt.Rest.GetMemberAsync(guildId.String(), guildUser.String()));
	public async override Task<IMessage> SendMessageAsync(EntityId channelId, string message) => new RevoltMessage(this, await Revolt.Rest.SendMessageAsync(channelId.String(), message));
	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, string message) => new RevoltMessage(this, await Revolt.Rest.EditMessageAsync(channelId.String(), messageId.String(), new Option<string>(message)));
	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => await Revolt.Rest.DeleteMessageAsync(channelId.String(), messageId.String());
}

internal static class EntityIdExtensions {
	public static string String(this EntityId entityId) => (string) entityId.UnderlyingId;
}

public class RevoltMessage : IMessage {
	private readonly RevoltSharpMessage m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => new EntityId(m_Entity.Id);
	public EntityId? GuildId => m_Entity.Channel is ServerChannel sc ? new EntityId(sc.ServerId) : null;
	public EntityId ChannelId => new EntityId(m_Entity.ChannelId);
	public EntityId AuthorId => new EntityId(m_Entity.AuthorId);
	public IGuild? Guild => m_Entity.Channel is ServerChannel sc ? new RevoltGuild(Client, sc.Server) : null;
	public IChannel Channel => new RevoltChannel(Client, m_Entity.Channel);
	public IUser Author => new RevoltUser(Client, m_Entity.Author);
	public IGuildMember? AuthorMember => m_Entity.Channel is ServerChannel sc ? new RevoltGuildMember(Client, sc.Server.GetCachedMember(m_Entity.AuthorId)) : null;
	public string? Content => (m_Entity as UserMessage)?.Content;
	public bool AuthorIsSelf => m_Entity.Author.Client.CurrentUser.Id == m_Entity.Id;
	public DateTimeOffset CreationTimestamp => Ulid.Parse(m_Entity.Id).Time;
	public IReadOnlyCollection<IReaction> Reactions => m_Entity is UserMessage um ? um.Reactions.CollectionSelect(kvp => new RevoltReaction(Client, new RevoltEmoji(Client, kvp.Key), kvp.Value.Length)) : Array.Empty<IReaction>();
	
	public RevoltMessage(RevoltChatClient client, RevoltSharpMessage entity) {
		Client = client;
		m_Entity = entity;
	}
}

public class RevoltChannel : IChannel {
	private readonly RevoltSharpChannel m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => new EntityId(m_Entity.Id);
	public string Name => m_Entity switch {
		GroupChannel groupChannel => groupChannel.Name,
		TextChannel textChannel => textChannel.Name,
		UnknownServerChannel unknownServerChannel => unknownServerChannel.Name,
		VoiceChannel voiceChannel => voiceChannel.Name,
		ServerChannel serverChannel => serverChannel.Name,
		//UnknownChannel unknownChannel => ,
		_ => throw new ArgumentOutOfRangeException(nameof(m_Entity))
	};
	
	public RevoltChannel(RevoltChatClient client, RevoltSharpChannel entity) {
		Client = client;
		m_Entity = entity;
	}
}

public class RevoltGuild : IGuild {
	private readonly RevoltSharpServer m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	
	public RevoltGuild(RevoltChatClient client, RevoltSharpServer entity) {
		Client = client;
		m_Entity = entity;
	}

	public EntityId Id => new EntityId(m_Entity.Id);
	public string Name => m_Entity.Name;
	public IReadOnlyList<IChannelCategory> ChannelCategories => throw new NotImplementedException(); // todo: model only exposes channel id list
}

public class RevoltUser : IUser {
	private readonly RevoltSharpUser m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => new EntityId(m_Entity.Id);
	public bool IsSelf => m_Entity.Client.CurrentUser.Id == m_Entity.Id;
	public string DisplayName => m_Entity.Username;
	public string Username => m_Entity.Username;
	public string DiscriminatedUsername => m_Entity.Username;
	public string MentionString => $"<@{m_Entity.Id}>";

	public RevoltUser(RevoltChatClient client, RevoltSharpUser entity) {
		Client = client;
		m_Entity = entity;
	}
}

public class RevoltGuildMember : IGuildMember {
	private readonly RevoltSharpServerMember m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public IUser User { get; }
	public EntityId UserId { get; }
	public IGuild Guild { get; }
	public EntityId GuildId { get; }
	public string? Nickname { get; }
	
	public RevoltGuildMember(RevoltChatClient client, RevoltSharpServerMember entity) {
		Client = client;
		m_Entity = entity;
	}
}

public class RevoltEmoji : IEmoji {
	private readonly RevoltSharpEmoji m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public bool IsAnimated => m_Entity.IsAnimated;
	public string Name => m_Entity.Name; // TODO: test req about unicode emojis
	public bool IsCustomizedEmote => m_Entity.IsServerEmoji;
	
	public RevoltEmoji(RevoltChatClient client, RevoltSharpEmoji entity) {
		Client = client;
		m_Entity = entity;
	}

	public bool Equals(IEmoji? other) {
		return other is RevoltEmoji otherRe
		    && m_Entity.IsServerEmoji == otherRe.m_Entity.IsServerEmoji
			// TODO: test req about unicode emojis
		    // TODO: test req about variant names
		    && m_Entity.Name == otherRe.m_Entity.Name
		    && m_Entity.Id == otherRe.m_Entity.Id;
	}

	public override string ToString() {
		// TODO test unicode emojis
		return $":{m_Entity.Id}:";
	}
}

public class RevoltReaction : IReaction {
	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public IEmoji Emoji { get; }
	public int Count { get; }
	
	public RevoltReaction(RevoltChatClient client, IEmoji emoji, int count) {
		Client = client;
		Emoji = emoji;
		Count = count;
	}
}
