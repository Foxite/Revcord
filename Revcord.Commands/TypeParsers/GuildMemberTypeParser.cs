using Qmmands;
using Revcord.Entities;

namespace Revcord.Commands;

public class GuildMemberTypeParser : RevcordTypeParser<IGuildMember> {
	private readonly ChatServiceObjectTypeParser<IUser> m_UserParser;

	public GuildMemberTypeParser(ChatServiceObjectTypeParser<IUser> userParser) {
		m_UserParser = userParser;
	}

	public async override ValueTask<TypeParserResult<IGuildMember>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		if (context.Guild == null) {
			return TypeParserResult<IGuildMember>.Failed("This command only works in a guild.");
		}
		
		TypeParserResult<IUser> result = await m_UserParser.ParseAsync(parameter, value, context);
		if (result.IsSuccessful) {
			IGuildMember member;
			try {
				member = await context.Guild.GetMemberAsync(result.Value.Id);
				return TypeParserResult<IGuildMember>.Successful(member);
			} catch (EntityNotFoundException) {
				return TypeParserResult<IGuildMember>.Failed("That user is not in this server.");
			}
		} else {
			return TypeParserResult<IGuildMember>.Failed("That user does not exist.");
		}
	}
}
