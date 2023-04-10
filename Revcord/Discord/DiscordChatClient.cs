using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Revcord.Entities;
using SharpGuild   = DSharpPlus.Entities.DiscordGuild;
using SharpChannel = DSharpPlus.Entities.DiscordChannel;
using SharpMessage = DSharpPlus.Entities.DiscordMessage;
using SharpUser	   = DSharpPlus.Entities.DiscordUser;
using SharpMember  = DSharpPlus.Entities.DiscordMember;

namespace Revcord.Discord;

public class DiscordChatClient : ChatClient {
	private readonly DiscordClient m_DSharp;
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public override IUser CurrentUser => new DiscordUser(this, m_DSharp.CurrentUser);

	public DiscordChatClient(DiscordConfiguration configuration) {
		m_DSharp = new DiscordClient(configuration);

		m_DSharp.MessageCreated += (_, args) => OnMessageCreated(new DiscordMessage(this, args.Message));
		m_DSharp.MessageUpdated += (_, args) => OnMessageUpdated(new DiscordMessage(this, args.Message));
		m_DSharp.MessageDeleted += (_, args) => OnMessageDeleted(new EntityId(args.Message.Id));
		m_DSharp.MessageReactionAdded += (_, args) => OnReactionAdded(new DiscordMessage(this, args.Message), new DiscordEmoji(this, args.Emoji));
		m_DSharp.MessageReactionRemoved += (_, args) => OnReactionRemoved(new DiscordMessage(this, args.Message), new DiscordEmoji(this, args.Emoji));
		m_DSharp.ClientErrored += (_, args) => OnClientError(args.Exception);
		m_DSharp.SocketErrored += (_, args) => OnClientError(args.Exception);
	}

	public async override Task StartAsync() {
		m_DSharp.Ready += (_, _) => {
			m_ReadyTcs.SetResult();
			return Task.CompletedTask;
		};
		await m_DSharp.ConnectAsync();
		await m_ReadyTcs.Task;
	}

	// todo: almost all of these do multiple api call because dsharpplus has a stupid abstraction and doesn't expose its rest client. to fix this, replace dsharpplus with something else
	public async override Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			return new DiscordMessage(this, message);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task<IChannel> GetChannelAsync(EntityId id) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(id.Ulong());
			return new DiscordChannel(this, channel);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task<IGuild> GetGuildAsync(EntityId id) {
		try {
			SharpGuild guild = await m_DSharp.GetGuildAsync(id.Ulong());
			return new DiscordGuild(this, guild);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task<IUser> GetUserAsync(EntityId id) {
		try {
			SharpUser user = await m_DSharp.GetUserAsync(id.Ulong());
			return new DiscordUser(this, user);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId) {
		try {
			SharpGuild guild = await m_DSharp.GetGuildAsync(guildId.Ulong());
			SharpMember member = await guild.GetMemberAsync(guildId.Ulong());
			return new DiscordMember(this, member);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			SharpMessage updatedMessage = await message.ModifyAsync(GetDiscordMessageBuilder(messageBuilder, null));
			return new DiscordMessage(this, updatedMessage);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo = null) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await m_DSharp.SendMessageAsync(channel, GetDiscordMessageBuilder(messageBuilder, responseTo));
			return new DiscordMessage(this, message);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			await message.DeleteAsync();
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			await message.CreateReactionAsync(((DiscordEmoji) emoji).Entity);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	public async override Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) {
		try {
			SharpChannel channel = await m_DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			await message.DeleteOwnReactionAsync(((DiscordEmoji) emoji).Entity);
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	private static Action<DiscordMessageBuilder> GetDiscordMessageBuilder(MessageBuilder messageBuilder, EntityId? responseTo) {
		return dmb => {
			if (responseTo.HasValue) {
				dmb.WithReply(responseTo.Value.Ulong());
			}

			dmb.WithContent(messageBuilder.Content);

			foreach (EmbedBuilder embed in messageBuilder.Embeds) {
				dmb.AddEmbed(BuildEmbed(embed));
			}
		};
	}

	private static DiscordEmbed BuildEmbed(EmbedBuilder embed) {
		return new DiscordEmbedBuilder() {
			Title = embed.Title,
			Description = embed.Description,
			Color = new DiscordColor(embed.Color.R, embed.Color.G, embed.Color.B),
			ImageUrl = embed.ImageUrl,
			Url = embed.Url,
			Author = new DiscordEmbedBuilder.EmbedAuthor() {
				IconUrl = embed.IconUrl
			}
		}.Build();
	}
}

internal static class EntityIdExtensions {
	public static ulong Ulong(this EntityId entityId) => (ulong) entityId.UnderlyingId;
}
