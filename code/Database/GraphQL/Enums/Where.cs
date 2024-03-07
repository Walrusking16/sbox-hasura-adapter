namespace Database.GraphQL;

public enum Where
{
	Equal,
	NotEqual,
	GreaterThan,
	GreaterThanOrEqual,
	LessThan,
	LessThanOrEqual,
	In,
	NotIn,
	IsNull,
	InsensitiveLike, // Should only be used on strings
	NotInsensitiveLike, // Should only be used on strings
	Like, // Should only be used on strings
	NotLike, // Should only be used on strings
	Regex, // Should only be used on strings
	NotRegex, // Should only be used on strings
	InsensitiveRegex, // Should only be used on strings
	NotInsensitiveRegex, // Should only be used on strings
	NotSimilar, // Should only be used on strings
	Similar // Should only be used on strings
}

public enum WhereType
{
	And,
	Or,
	Not
}
