using System.Drawing;

namespace Revcord;

public class MessageBuilder {
	public string Content { get; set; }
	public List<EmbedBuilder> Embeds { get; set; } = new List<EmbedBuilder>();
	
	public MessageBuilder WithContent(string content) {
		Content = content;
		return this;
	}

	public MessageBuilder AddEmbed(EmbedBuilder embedBuilder) {
		Embeds.Add(embedBuilder);
		return this;
	}
}

public class EmbedBuilder {
	public string Title { get; set; }
	public string Description { get; set; }
	public Color Color { get; set; }
	public string ImageUrl { get; set; }
	public string Url { get; set; }
	public string IconUrl { get; set; }
	public List<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();

	public EmbedBuilder WithTitle(string title) {
		Title = title;
		return this;
	}

	public EmbedBuilder WithDescription(string description) {
		Description = description;
		return this;
	}

	public EmbedBuilder WithImageUrl(string imageUrl) {
		ImageUrl = imageUrl;
		return this;
	}

	public EmbedBuilder WithColor(Color color) {
		Color = color;
		return this;
	}

	public EmbedBuilder WithIconUrl(string iconUrl) {
		IconUrl = iconUrl;
		return this;
	}

	public EmbedBuilder WithUrl(string url) {
		Url = url;
		return this;
	}

	public EmbedBuilder AddField(EmbedFieldBuilder efb) {
		Fields.Add(efb);
		return this;
	}
}

public class EmbedFieldBuilder {
	public string Title { get; set; }
	public string Description { get; set; }

	public EmbedFieldBuilder(string title, string description) {
		Title = title;
		Description = description;
	}
	
	public EmbedFieldBuilder WithTitle(string title) {
		Title = title;
		return this;
	}

	public EmbedFieldBuilder WithDescription(string description) {
		Description = description;
		return this;
	}
}
