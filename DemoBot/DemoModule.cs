using Qmmands;
using Revcord;
using Revcord.Commands;
using Revcord.Entities;

namespace DemoBot; 

public class DemoModule : ModuleBase<RevcordCommandContext> {
	[Command("ping")]
	public CommandResult Ping() {
		return new ObjectResult("Pong command!");
	}

	[Command("messagebuilder")]
	public CommandResult MessageBuilder() {
		return new ObjectResult(new MessageBuilder().WithContent("Sent by message builder!"));
	}

	[Command("embed")]
	public CommandResult Embed(int hey, string bla) {
		return new ObjectResult(new TestObject(hey, bla));
	}

	[Command("user")]
	public CommandResult UserMention(IUser user) {
		return new ObjectResult(user.DiscriminatedUsername + "!");
	}

	[Command("member")]
	public CommandResult MemberMention(IGuildMember guildMember) {
		return new ObjectResult(guildMember.DisplayName + "!");
	}

	[Command("channel")]
	public CommandResult ChannelMention(IChannel channel) {
		return new ObjectResult(channel.Name + "!");
	}

	[Command("emoji")]
	public CommandResult EmojiMention(IEmoji emoji) {
		return new ObjectResult($"{emoji.ToString()}! {emoji.Id}! {emoji.Name}!");
	}

	[Command("countdown")]
	public async Task Countdown() {
		var message = await Context.Message.SendReplyAsync("3");
		await Task.Delay(TimeSpan.FromSeconds(1));
		await message.UpdateAsync("2");
		await Task.Delay(TimeSpan.FromSeconds(1));
		await message.UpdateAsync("1");
	}
}