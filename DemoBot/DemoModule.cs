using Qmmands;
using Revcord;
using Revcord.Commands;
using Revcord.Entities;

public class DemoModule : ModuleBase<RevcordCommandContext> {
	[Command("ping")]
	public CommandResult Ping() {
		return new TextResult("Pong command!");
	}

	[Command("user")]
	public CommandResult UserMention(IUser user) {
		return new TextResult(user.DiscriminatedUsername + "!");
	}

	[Command("member")]
	public CommandResult MemberMention(IGuildMember guildMember) {
		return new TextResult(guildMember.DisplayName + "!");
	}

	[Command("channel")]
	public CommandResult ChannelMention(IChannel channel) {
		return new TextResult(channel.Name + "!");
	}

	[Command("emoji")]
	public CommandResult EmojiMention(IEmoji emoji) {
		return new TextResult($"{emoji.ToString()}! {emoji.Id}! {emoji.Name}!");
	}

	[Command("countdown")]
	public async Task Countdown() {
		var message = await Context.RespondAsync("3");
		await Task.Delay(TimeSpan.FromSeconds(1));
		await message.UpdateAsync("2");
		await Task.Delay(TimeSpan.FromSeconds(1));
		await message.UpdateAsync("1");
	}
}

public class TextResult : CommandResult {
	public override bool IsSuccessful => true;
	
	public string Response { get; set; }
	
	public TextResult(string response) {
		Response = response;
	}

	public override string ToString() => Response;
}