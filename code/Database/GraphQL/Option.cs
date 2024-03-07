using System;
using System.Collections.Generic;

namespace Database.GraphQL;

public class Option
{
	public Option( string name )
	{
		Name = name;
	}

	public string Name { get; set; }
}

public class OptionBuilder<T>
{
	public List<T> Options { get; set; } = new();
}

public class ObjectOption : Option
{
	public object Value { get; set; }

	private bool _ignoreQuotes = false;
	
	public ObjectOption( string name, object value, bool ignoreQuotes ) : base( name )
	{
		Value = value;
		_ignoreQuotes = ignoreQuotes;
	}

	
	public ObjectOption( string name, object value ) : base( name )
	{
		Value = value;
	}

	public override string ToString()
	{
		if ( Value is string && !_ignoreQuotes)
		{
			Value = $"\"{Value}\"".Replace( "\"\"", "\"" );
		}
		
		return $"{Name}: {Value}";
	}
}

public class OrderByOption : Option
{
	public OrderByOption( OrderBy orderBy, string name ) : base( name )
	{
		OrderBy = orderBy;
	}

	public OrderBy OrderBy { get; set; }

	public string GetId()
	{
		return OrderBy switch
		{
			OrderBy.Asc => "asc",
			OrderBy.AscNullsFirst => "asc_nulls_first",
			OrderBy.AscNullsLast => "asc_nulls_last",
			OrderBy.Desc => "desc",
			OrderBy.DescNullsFirst => "desc_nulls_first",
			OrderBy.DescNullsLast => "desc_nulls_last",
			_ => throw new NotImplementedException()
		};
	}

	public override string ToString()
	{
		return $"{Name}: {GetId()}";
	}
}
