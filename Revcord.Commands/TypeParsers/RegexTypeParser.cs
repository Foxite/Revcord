using System.Text.RegularExpressions;
using Qmmands;

namespace Revcord.Commands;

public class RegexTypeParser<T> : RevcordTypeParser<T> {
	private readonly Regex m_Regex;
	private readonly Func<RevcordCommandContext, Match, Task<T>> m_Factory;
	private readonly string m_FailureReason;

	public RegexTypeParser(Regex regex, Func<RevcordCommandContext, Match, Task<T>> factory, string failureReason) {
		m_Regex = regex;
		m_Factory = factory;
		m_FailureReason = failureReason;
	}

	public async override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		var match = m_Regex.Match(value);
		if (match.Success) {
			return TypeParserResult<T>.Successful(await m_Factory(context, match));
		} else {
			return TypeParserResult<T>.Failed(m_FailureReason);
		}
	}
}
