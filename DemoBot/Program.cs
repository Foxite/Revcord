using System.Diagnostics;
using System.Drawing;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qmmands;
using Revcord;
using Revcord.Commands;
using Revcord.Entities;

var isc = new ServiceCollection();
string whichEnv = Environment.GetEnvironmentVariable("WHICH")!;

foreach (var which in whichEnv.Split(";")) {
	ChatClient client = which switch {
		"discord" => new Revcord.Discord.DiscordChatClient(new DSharpPlus.DiscordConfiguration() {
			Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")!,
			Intents = DSharpPlus.DiscordIntents.GuildMessages | DSharpPlus.DiscordIntents.MessageContents | DSharpPlus.DiscordIntents.GuildMessageReactions | DSharpPlus.DiscordIntents.Guilds | DiscordIntents.GuildMembers,
		}),
		"revolt" => new Revcord.Revolt.RevoltChatClient(Environment.GetEnvironmentVariable("REVOLT_TOKEN")!),
	};
	
	isc.TryAddEnumerable(ServiceDescriptor.Singleton(client));
}

isc.AddRevcordCommands(commands => {
	commands.AddModule<DemoModule>();
});



var services = isc.BuildServiceProvider();
var commands = services.GetRequiredService<CommandService>();

async Task HandleCommandMessage(MessageCreatedArgs args) {
	if (args.Message.Content != null && args.Message.Content.StartsWith(args.Client.CurrentUser.MentionString)) {
		string commandText = args.Message.Content[args.Client.CurrentUser.MentionString.Length..];
		var context = new RevcordCommandContext(args.Message, services);
		IResult result = await commands.ExecuteAsync(commandText, context);
		Console.WriteLine(result.GetType().Name);
		Console.WriteLine(result);
		
		switch (result) {
			case CommandExecutionFailedResult cefr:
				Console.WriteLine($"Execution failed at {cefr.CommandExecutionStep}: {cefr.Exception.ToStringDemystified()}");
				break;
			case OverloadsFailedResult ofr:
				Console.WriteLine(ofr.FailureReason);
				foreach ((Command? command, FailedResult? failedResult) in ofr.FailedOverloads) {
					Console.WriteLine($"{command.Name}: {failedResult?.FailureReason ?? "null"}");
				}
				break;
			case TypeParseFailedResult tpfr:
				break;
		}
		
		string? resultString = result.ToString();
		if (resultString != null) {
			await context.RespondAsync(resultString);
		}
	}
}

async Task HandleMessage(MessageCreatedArgs args) {
	IMessage message = args.Message;
	
	if (message.AuthorIsSelf) {
		return;
	}

	if (message.Content != null) {
		Console.WriteLine(message.Content);
		
		string lowerContent = message.Content.ToLower();
		if (lowerContent.StartsWith("ping")) {
			await args.Client.SendMessageAsync(message.Channel, "Pong!");
		} else if (lowerContent.StartsWith("reply")) {
			await message.SendReplyAsync("Reply!");
		} else if (lowerContent.StartsWith("mention")) {
			await message.SendReplyAsync(message.Author.MentionString + " !");
		} else if (lowerContent.StartsWith("delay")) {
			await message.SendReplyAsync($"{(DateTimeOffset.UtcNow - message.CreationTimestamp).TotalMilliseconds} ms!");
		} else if (lowerContent.StartsWith("embed")) {
			await message.SendReplyAsync(new MessageBuilder().WithContent("Embed!")
				.AddEmbed(new EmbedBuilder().WithTitle("This is an embed!")
					.WithDescription("This is its description!")
					.WithIconUrl(message.Author.AvatarUrl)
					.WithColor(Color.Orange)
					.WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/3/30/Vulpes_vulpes_ssp_fulvus.jpg/1280px-Vulpes_vulpes_ssp_fulvus.jpg")
					.WithUrl("https://foxite.me")
					.AddField(new EmbedFieldBuilder("Field?!", "(TOP SECRET: doesnt work on revolt :/"))));
		}
	}
}

async Task HandleReactionModified(ReactionModifiedArgs args) {
	if (args.Added) {
		await args.Message.AddReactionAsync(args.Emoji);
	} else {
		await args.Message.RemoveReactionAsync(args.Emoji);
	}
}


foreach (var client in services.GetRequiredService<IEnumerable<ChatClient>>()) {
	await client.StartAsync();

	client.MessageCreated += HandleMessage;
	client.MessageCreated += HandleCommandMessage;
	client.ReactionAdded += HandleReactionModified;
	client.ReactionRemoved += HandleReactionModified;
}

Console.WriteLine("Hello, World!");

await Task.Delay(-1);
