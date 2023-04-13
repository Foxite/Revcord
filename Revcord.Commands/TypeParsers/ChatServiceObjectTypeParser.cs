using Qmmands;
using Revcord.Entities;

namespace Revcord.Commands;

public class ChatServiceObjectTypeParser<T> : RevcordTypeParser<T> where T : IChatServiceObject {
	private readonly Dictionary<Type, IRevcordTypeParser> m_TypeParsers = new();

	public delegate ValueTask<TypeParserResult<TEntity>> ParseDelegate<TEntity>(Parameter parameter, string value, RevcordCommandContext context) where TEntity : T;

	public void Add(Type chatClientType, IRevcordTypeParser typeParser) => m_TypeParsers[chatClientType] = typeParser;

	public async override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, RevcordCommandContext context) {
		if (m_TypeParsers.TryGetValue(context.Client.GetType(), out IRevcordTypeParser? parser)) {
			TypeParserResult<object?> result = await parser.ParseAsync(parameter, value, context);
			if (result.IsSuccessful) {
				return new TypeParserResult<T>((T) result.Value!);
			} else {
				return new TypeParserResult<T>(result.FailureReason);
			}
		} else {
			return TypeParserResult<T>.Failed($"No installed ChatClient support for parsing {typeof(T).Name}");
		}
	}
}
