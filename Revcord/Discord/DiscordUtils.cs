using DSharpPlus.Entities;
using Revcord.Entities;

namespace Revcord.Discord;

internal static class DiscordUtils {
	public static ulong Ulong(this EntityId entityId) => (ulong) entityId.UnderlyingId;
	
	public static Action<DiscordMessageBuilder> GetDiscordMessageBuilder(MessageBuilder messageBuilder, EntityId? responseTo) {
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

	public static DiscordEmbed BuildEmbed(EmbedBuilder embed) {
		var ret = new DiscordEmbedBuilder() {
			Title = embed.Title,
			Description = embed.Description,
			Color = new DiscordColor(embed.Color.R, embed.Color.G, embed.Color.B),
			ImageUrl = embed.ImageUrl,
			Url = embed.Url,
			Author = new DiscordEmbedBuilder.EmbedAuthor() {
				IconUrl = embed.IconUrl
			},
		};

		foreach (EmbedFieldBuilder field in embed.Fields) {
			ret.AddField(field.Title, field.Description);
		}
		
		return ret.Build();
	}
}
