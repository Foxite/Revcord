using System.Drawing;
using DSharpPlus;
using Revcord;
using Revcord.Discord;
using Revcord.Entities;
using Revcord.Revolt;

async Task HandleMessage(ChatClient client, IMessage message) {
	if (message.AuthorIsSelf) {
		return;
	}

	if (message.Content != null) {
		string lowerContent = message.Content.ToLower();
		if (lowerContent.StartsWith("ping")) {
			await client.SendMessageAsync(message.Channel, "Pong!");
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


string whichEnv = Environment.GetEnvironmentVariable("WHICH")!;

foreach (var which in whichEnv.Split(";")) {
	ChatClient client = which switch {
		"discord" => new DiscordChatClient(new DiscordConfiguration() {
			Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")!,
			Intents = DiscordIntents.GuildMessages | DiscordIntents.MessageContents,
		}),
		"revolt" => new RevoltChatClient(Environment.GetEnvironmentVariable("REVOLT_TOKEN")!),
	};

	await client.StartAsync();

	client.MessageCreated += HandleMessage;
}

Console.WriteLine("Hello, World!");

await Task.Delay(-1);
