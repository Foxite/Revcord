using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Foxite.Text;
using Foxite.Text.Parsers;
using Revcord.Discord.Renderers;
using Revcord.Entities;
using SharpGuild   = DSharpPlus.Entities.DiscordGuild;
using SharpChannel = DSharpPlus.Entities.DiscordChannel;
using SharpMessage = DSharpPlus.Entities.DiscordMessage;
using SharpUser	   = DSharpPlus.Entities.DiscordUser;
using SharpMember  = DSharpPlus.Entities.DiscordMember;

namespace Revcord.Discord;

public class DiscordChatClient : ChatClient {
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public DiscordClient DSharp { get; }
	
	public override IUser CurrentUser => new DiscordUser(this, DSharp.CurrentUser);
	
	// TODO add spoilers, etc
	public override ITextFormatter TextFormatter => ModularTextFormatter.Markdown();
	public override Parser TextParser => new MarkdownParser();

	public DiscordChatClient(DiscordConfiguration configuration) {
		DSharp = new DiscordClient(configuration);

		DSharp.MessageCreated += (_, args) => OnMessageCreated(new DiscordMessage(this, args.Message));
		DSharp.MessageUpdated += (_, args) => OnMessageUpdated(new DiscordMessage(this, args.Message));
		DSharp.MessageDeleted += (_, args) => OnMessageDeleted(new DiscordChannel(this, args.Channel), EntityId.Of(args.Message.Id));
		DSharp.MessageReactionAdded += async (_, args) => await OnReactionAdded(new DiscordMessage(this, await args.Channel.GetMessageAsync(args.Message.Id)), new DiscordEmoji(this, args.Emoji), new DiscordMember(this, (SharpMember) args.User));
		DSharp.MessageReactionRemoved += async (_, args) => await OnReactionRemoved(new DiscordMessage(this, await args.Channel.GetMessageAsync(args.Message.Id)), new DiscordEmoji(this, args.Emoji), new DiscordMember(this, (SharpMember) args.User));
		DSharp.ClientErrored += (_, args) => OnClientError(args.Exception);
		DSharp.SocketErrored += (_, args) => OnClientError(args.Exception);
		
		AddRenderer(new StringRenderer(this));
		AddRenderer(new MessageBuilderRenderer(this));
	}

	public async override Task StartAsync() {
		Task OnReady(DiscordClient discordClient, ReadyEventArgs readyEventArgs) {
			m_ReadyTcs.SetResult();
			return Task.CompletedTask;
		}
		
		Task OnError(DiscordClient discordClient, SocketErrorEventArgs socketErrorEventArgs) {
			m_ReadyTcs.SetException(new ChatClientException(this, "DSharpPlus failed to connect.", socketErrorEventArgs.Exception));
			return Task.CompletedTask;
		}

		DSharp.Ready += OnReady;
		DSharp.SocketErrored += OnError;

		try {
			await DSharp.ConnectAsync();
			await m_ReadyTcs.Task;
		} finally {
			DSharp.Ready -= OnReady;
			DSharp.SocketErrored -= OnError;
		}
	}

	// todo: almost all of these do multiple api call because dsharpplus has a stupid abstraction and doesn't expose its rest client. to fix this, replace dsharpplus with something else
	public override Task<IMessage> GetMessageAsync(EntityId channelId, EntityId messageId) {
		return CallRest(async () => {
			SharpChannel channel = await DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			return (IMessage) new DiscordMessage(this, message);
		});
	}

	public override Task<IChannel> GetChannelAsync(EntityId id) {
		return CallRest(async () => {
			SharpChannel channel = await DSharp.GetChannelAsync(id.Ulong());
			return (IChannel) new DiscordChannel(this, channel);
		});
	}

	public override Task<IGuild> GetGuildAsync(EntityId id) {
		return CallRest(async () => {
			SharpGuild guild = await DSharp.GetGuildAsync(id.Ulong());
			return (IGuild) new DiscordGuild(this, guild);
		});
	}

	public override Task<IUser> GetUserAsync(EntityId id) {
		return CallRest(async () => {
			SharpUser user = await DSharp.GetUserAsync(id.Ulong());
			return (IUser) new DiscordUser(this, user);
		});
	}

	public override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId) {
		return CallRest(async () => {
			SharpGuild guild = await DSharp.GetGuildAsync(guildId.Ulong());
			SharpMember member = await guild.GetMemberAsync(userId.Ulong());
			return (IGuildMember) new DiscordMember(this, member);
		});
	}

	public override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) {
		return CallRest(async () => {
			SharpChannel channel = await DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			await message.DeleteAsync();
		});
	}

	public override Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) {
		return CallRest(async () => {
			SharpChannel channel = await DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			await message.CreateReactionAsync(((DiscordEmoji) emoji).Entity);
		});
	}

	public override Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) {
		return CallRest(async () => {
			SharpChannel channel = await DSharp.GetChannelAsync(channelId.Ulong());
			SharpMessage message = await channel.GetMessageAsync(messageId.Ulong());
			await message.DeleteOwnReactionAsync(((DiscordEmoji) emoji).Entity);
		});
	}
	
	private Task<T> CallRest<T>(Func<Task<T>> func) {
		try {
			return func();
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}

	private Task CallRest(Func<Task> func) {
		try {
			return func();
		} catch (NotFoundException ex) {
			throw new EntityNotFoundException(this, ex);
		} catch (DiscordException ex) {
			throw new ChatClientException(this, "DSharpPlus threw an exception", ex);
		}
	}
}