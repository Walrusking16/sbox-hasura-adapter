using System.Collections.Generic;

namespace Database.GraphQL;

public class FieldBuilder
{
	public FieldBuilder( string field )
	{
		FieldName = field;
	}

	private string FieldName { get; set; }
	public List<FieldBuilder> SubFields { get; set; } = new();

	public FieldBuilder Field( params FieldBuilder[] fields )
	{
		foreach ( var field in fields )
		{
			SubFields.Add( field );
		}

		return this;
	}
	
	public FieldBuilder Field( params Hasura.CachedField[] fields )
	{
		foreach ( var field in fields )
		{
			SubFields.Add( new FieldBuilder( field.Name ) );
		}

		return this;
	}

	public FieldBuilder Field( params string[] fields )
	{
		foreach ( var field in fields )
		{
			SubFields.Add( new FieldBuilder( field ) );
		}

		return this;
	}

	public override string ToString()
	{
		return SubFields.Count == 0 ? FieldName : $"{FieldName} {{ {string.Join( " ", SubFields )} }}";
	}
}
