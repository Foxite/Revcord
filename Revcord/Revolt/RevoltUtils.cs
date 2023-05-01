using RevoltSharp;

namespace Revcord.Revolt;

public static class RevoltUtils {
	public static Embed BuildEmbed(EmbedBuilder embedBuilder) {
		return new RevoltSharp.EmbedBuilder() {
			Title = embedBuilder.Title,
			Description = embedBuilder.Description,
			Color = new RevoltColor(embedBuilder.Color.R, embedBuilder.Color.G, embedBuilder.Color.B),
			IconUrl = embedBuilder.IconUrl,
			Image = embedBuilder.ImageUrl,
			Url = embedBuilder.Url
		}.Build();
	}
}
