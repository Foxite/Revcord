using DSharpPlus;
using Revcord;
using Revcord.Discord;
using Revcord.Revolt;

string which = Environment.GetEnvironmentVariable("WHICH")!;
ChatClient client = which switch {
	"discord" => new DiscordChatClient(new DiscordConfiguration() {
		Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")!,
		Intents = DiscordIntents.GuildMessages | DiscordIntents.MessageContents,
	}),
	"revolt" => new RevoltChatClient(Environment.GetEnvironmentVariable("REVOLT_TOKEN")!),
};

await client.StartAsync();

Console.WriteLine("Hello, World!");

client.MessageCreated += async (_, message) => {
	if (message.AuthorIsSelf) {
		return;
	}
	
	Console.WriteLine(message.Content);

	if (message.Content != null && message.Content.ToLower().StartsWith("ping")) {
		await client.SendMessageAsync(message.ChannelId, "Pong!");
	}

	if (message.Content != null && message.Content.ToLower().StartsWith("reply")) {
		await message.SendReplyAsync("Reply!");
	}
};

await Task.Delay(-1);
