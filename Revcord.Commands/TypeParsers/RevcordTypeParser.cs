using Qmmands;

namespace Revcord.Commands;

public abstract class RevcordTypeParser<T> : TypeParser<T>, IRevcordTypeParser {
	public sealed override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, CommandContext context) {
		if (context is RevcordCommandContext revcordContext) {
			return ParseAsync(parameter, value, revcordContext);
		} else {
			throw new InvalidOperationException("Context is not a RevcordCommandContext.");
		}
	}

	public abstract ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context);
	
	async ValueTask<TypeParserResult<object?>> IRevcordTypeParser.ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		TypeParserResult<T> result = await ParseAsync(parameter, value, context);
		if (result.IsSuccessful) {
			return TypeParserResult<object?>.Successful(result.Value);
		} else {
			return TypeParserResult<object?>.Failed(result.FailureReason);
		}
	}
}
