using Revcord.Entities;
using RevoltSharp;
using EmbedBuilderSharp = RevoltSharp.EmbedBuilder;

namespace Revcord.Revolt; 

public class RevoltChatClient : ChatClient {
	public RevoltClient Revolt { get; }
	private readonly TaskCompletionSource m_ReadyTcs = new();

	public override IUser CurrentUser => new RevoltUser(this, Revolt.CurrentUser);
	public string FrontendUrl { get; }
	public string AutumnUrl { get; }

	public RevoltChatClient(string token, ClientConfig? config = null, string frontendUrl = "https://app.revolt.chat", string autumnUrl = "https://autumn.revolt.chat") {
		Revolt = new RevoltClient(token, ClientMode.WebSocket, config!);
		FrontendUrl = frontendUrl;
		AutumnUrl = autumnUrl;

		Revolt.OnMessageRecieved += message => OnMessageCreated(new RevoltMessage(this, message));
		Revolt.OnMessageUpdated += async (channel, messageId, content) => await OnMessageUpdated(await GetMessageAsync(new EntityId(channel), new EntityId(messageId)));
		Revolt.OnMessageDeleted += (channel, id) => OnMessageDeleted(new EntityId(id)); // parameter names inferred

		//Revolt.OnReactionAdded += (emoji, channel, member, message) => OnReactionAdded(new RevoltMessage(this, message), new RevoltEmoji(this, emoji));
		//Revolt.OnReactionRemoved += (emoji, channel, member, message) => OnReactionRemoved(new RevoltMessage(this, message), new RevoltEmoji(this, emoji));

		Revolt.OnWebSocketError += error => OnClientError(new RevoltException(error.Messaage, (int) error.Type));
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
	public async override Task<IGuildMember> GetGuildMemberAsync(EntityId guildId, EntityId userId) => new RevoltGuildMember(this, await Revolt.Rest.GetMemberAsync(guildId.String(), userId.String()));
	public async override Task<IMessage> SendMessageAsync(EntityId channelId, MessageBuilder messageBuilder, EntityId? responseTo = null) => new RevoltMessage(this, await Revolt.Rest.SendMessageAsync(channelId.String(), messageBuilder.Content, embeds: messageBuilder.Embeds.Count == 0 ? null! : messageBuilder.Embeds.Select(BuildEmbed).ToArray(), replies: responseTo == null ? default! : new [] { new MessageReply() { id = responseTo.Value.String(), mention = false } }));
	public async override Task<IMessage> UpdateMessageAsync(EntityId channelId, EntityId messageId, MessageBuilder messageBuilder) => new RevoltMessage(this, await Revolt.Rest.EditMessageAsync(channelId.String(), messageId.String(), new Option<string>(messageBuilder.Content), new Option<Embed[]>(messageBuilder.Embeds.Select(BuildEmbed).ToArray())));
	public async override Task DeleteMessageAsync(EntityId channelId, EntityId messageId) => await Revolt.Rest.DeleteMessageAsync(channelId.String(), messageId.String());
	public override Task AddReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) => Revolt.Rest.AddMessageReactionAsync(channelId.String(), messageId.String(), emoji.Id.String());
	public override Task RemoveReactionAsync(EntityId channelId, EntityId messageId, IEmoji emoji) => Revolt.Rest.RemoveMessageReactionAsync(channelId.String(), messageId.String(), Revolt.CurrentUser.Id, emoji.Id.String());

	private static Embed BuildEmbed(EmbedBuilder embedBuilder) {
		return new EmbedBuilderSharp() {
			Title = embedBuilder.Title,
			Description = embedBuilder.Description,
			Color = new RevoltColor(embedBuilder.Color.R, embedBuilder.Color.G, embedBuilder.Color.B),
			IconUrl = embedBuilder.IconUrl,
			Image = embedBuilder.ImageUrl,
			Url = embedBuilder.Url
		}.Build();
	}
}
