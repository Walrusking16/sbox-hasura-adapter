using System;
using System.Linq;

namespace Database.GraphQL;

public class WhereBuilder : OptionBuilder<WhereOption>
{
	public override string ToString()
	{
		var @string = "";

		{
			var options = Options.Where( x => x.WhereType == WhereType.And ).ToList();

			if ( options.Count > 0 )
			{
				@string += $"_and: {{ {string.Join( ", ", options )} }}, ";
			}
		}

		{
			var options = Options.Where( x => x.WhereType == WhereType.Or ).ToList();
			
			if ( options.Count > 0 )
			{
				@string += $"_or: {{ {string.Join( ", ", options )} }}, ";
			}
		}

		{
			var options = Options.Where( x => x.WhereType == WhereType.Not ).ToList();
			
			if ( options.Count > 0 )
			{
				@string += $"_not: {{ {string.Join( ", ", options )} }}, ";
			}
		}

		return @string.Length > 0
			? $"where: {{ {string.Join( ", ", @string.TrimEnd( ' ', ',' ) )} }}"
			: $"where: {{ {string.Join( ", ", Options )} }}";
	}
	
}

public class WhereOption : Option
{
	public WhereOption( WhereType type, Where where, string name, object value ) : base( name )
	{
		WhereType = type;
		Where = where;
		Value = value;
	}

	public WhereOption( Where where, string name, object value ) : base( name )
	{
		Where = where;
		Value = value;
	}
	
	public WhereType? WhereType { get; set; }
	private Where Where { get; set; }
	private object Value { get; set; }

	public override string ToString()
	{
		var id = Where switch
		{
			Where.Equal => "eq",
			Where.NotEqual => "neq",
			Where.GreaterThan => "gt",
			Where.GreaterThanOrEqual => "gte",
			Where.LessThan => "lt",
			Where.LessThanOrEqual => "lte",
			Where.In => "in",
			Where.NotIn => "nin",
			Where.IsNull => "is_null",
			Where.InsensitiveLike => "ilike",
			Where.NotInsensitiveLike => "nilike",
			Where.Like => "like",
			Where.NotLike => "nlike",
			Where.Regex => "regex",
			Where.NotRegex => "nregex",
			Where.InsensitiveRegex => "iregex",
			Where.NotInsensitiveRegex => "niregex",
			Where.NotSimilar => "nsimilar",
			Where.Similar => "similar",

			_ => throw new NotImplementedException()
		};

		if ( Value is string )
		{
			Value = $"\"{Value}\"".Replace( "\"\"", "\"" );
		}

		return $"{Name}: {{ _{id}: {Value} }}";
	}
}
