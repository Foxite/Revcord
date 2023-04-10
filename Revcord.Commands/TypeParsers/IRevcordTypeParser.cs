using Qmmands;

namespace Revcord.Commands;

public interface IRevcordTypeParser {
	public abstract ValueTask<TypeParserResult<object?>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context);
}
