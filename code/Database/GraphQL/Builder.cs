using System;
using System.Collections.Generic;

namespace Database.GraphQL;

public class Builder
{
	public Type Type { get; set; } = Type.Query;
	
	public Builder()
	{
	}

	public static Builder FromClass<T>()
	{
		return Hasura.GetClassBuilder<T>();
	}
	
	public Builder( Type type )
	{
		Type = type;
	}
	
	public Builder( string table, params Hasura.CachedField[] fields )
	{
		Table( table, fields );
	}

	public Builder( string table, params string[] fields )
	{
		Table( table, fields );
	}

	public Builder( string table, params FieldBuilder[] fields )
	{
		Table( table, fields );
	}

	public List<TableBuilder> Tables { get; set; } = new();
	public TableBuilder FirstTable => Tables[0];
	
	public TableBuilder Table( string table )
	{
		var builder = new TableBuilder( table );
		Tables.Add( builder );
		return builder;
	}
	
	public TableBuilder Table( string table, params Hasura.CachedField[] fields )
	{
		var tbl = new TableBuilder( table );
		
		foreach (var cachedField in fields)
		{
			tbl.Field( cachedField.Name );
		}

		Tables.Add( tbl );
		return tbl;
	}

	public TableBuilder Table( string table, params string[] fields )
	{
		var tbl = new TableBuilder( table );
		tbl.Field( fields );

		Tables.Add( tbl );
		return tbl;
	}

	public TableBuilder Table( string table, params FieldBuilder[] fields )
	{
		var tbl = new TableBuilder( table );
		tbl.Field( fields );

		Tables.Add( tbl );
		return tbl;
	}

	public Builder Table( params TableBuilder[] tables )
	{
		Tables.AddRange( tables );

		return this;
	}

	public override string ToString()
	{
		var title = Type switch
		{
			Type.Query => "query GetData",
			Type.Mutation => "mutation SetData",
			Type.Subscription => "subscription SubscribeData",
			_ => throw new ArgumentOutOfRangeException()
		};
		
		var variables = FirstTable.Variables;

		if ( variables.Count > 0 )
		{
			return $"{title}({string.Join( ", ", variables )}) {{ {string.Join( " ", Tables )} }}";
		}

		return $"{title} {{ {string.Join( " ", Tables )} }}";
	}
}
