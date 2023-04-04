using DSharpPlus;
using Revcord;
using Revcord.Discord;
using Revcord.Revolt;

string which = Environment.GetEnvironmentVariable("WHICH")!;
ChatClient client = which switch {
	"discord" => new DiscordChatClient(Environment.GetEnvironmentVariable("DISCORD_TOKEN")!, DiscordIntents.GuildMessages, null),
	"revolt" => new RevoltChatClient(Environment.GetEnvironmentVariable("REVOLT_TOKEN")!),
};

await client.StartAsync();

Console.WriteLine("Hello, World!");

client.MessageCreated += async (_, message) => {
	Console.WriteLine(message.Content);

	if (message.Content != null && message.Content.ToLower().StartsWith("ping")) {
		await client.SendMessageAsync(message.ChannelId, "Pong!");
	}
};

await Task.Delay(-1);
