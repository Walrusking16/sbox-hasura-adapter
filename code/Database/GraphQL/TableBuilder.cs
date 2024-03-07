using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Database.GraphQL;

public class TableBuilder
{
	private HashSet<System.Type> SystemTypes = new()
	{
		typeof(int),
		typeof(long),
		typeof(double),
		typeof(float),
		typeof(bool),
		typeof(char),
		typeof(byte),
		typeof(sbyte),
		typeof(short),
		typeof(ushort),
		typeof(uint),
		typeof(ulong),
		typeof(decimal),
		typeof(string)
	};

	public TableBuilder( string table )
	{
		Table( table );
	}

	public TableBuilder( string table, params Hasura.CachedField[] fields )
	{
		Table( table );

		foreach ( var cachedField in fields )
		{
			Field( cachedField.Name );
		}
	}

	public TableBuilder( string table, params string[] fields )
	{
		Table( table );

		Field( fields );
	}

	public TableBuilder( string table, params FieldBuilder[] fields )
	{
		Table( table );

		Field( fields );
	}

	public string TableName { get; set; } = "";
	private List<FieldBuilder> Fields { get; } = new();
	private OptionsBuilder<ObjectOption> PKColumnsBuilder { get; } = new("pk_columns");
	private OptionsBuilder<ObjectOption> SetBuilder { get; } = new("_set");
	private OptionsBuilder<ObjectOption> ObjectBuilder { get; } = new("object");
	private OptionsBuilder<OrderByOption> OrderByBuilder { get; } = new("order_by");
	private WhereBuilder WhereBuilder { get; } = new();
	public List<VariableBuilder> Variables { get; } = new();
	private int QueryLimit { get; set; }
	private int QueryOffset { get; set; }
	private string QueryDistinctOn { get; set; } = "";
	private string UniqueType { get; set; } = "";
	private object UniqueSearch { get; set; } = "";
	private bool UniqueAlwaysString { get; set; } = true;

	public TableBuilder Table( string table )
	{
		TableName = table;
		return this;
	}

	public TableBuilder DistinctOn( string distinctOn )
	{
		QueryDistinctOn = distinctOn;
		return this;
	}

	public TableBuilder Limit( int limit )
	{
		QueryLimit = limit;
		return this;
	}

	public TableBuilder Offset( int offset )
	{
		QueryOffset = offset;
		return this;
	}

	public TableBuilder Unique( string type, object search, bool alwaysString = true )
	{
		UniqueAlwaysString = alwaysString;
		UniqueType = type;
		UniqueSearch = search;
		return this;
	}

	/// <summary>
	///     Order by any method
	/// </summary>
	/// <param name="orderBy"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public TableBuilder OrderBy( OrderBy orderBy, string name )
	{
		OrderByBuilder.Options.Add( new OrderByOption( orderBy, name ) );

		return this;
	}

	/// <summary>
	///     Order by descending
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public TableBuilder OrderBy( string name )
	{
		OrderByBuilder.Options.Add( new OrderByOption( GraphQL.OrderBy.Desc, name ) );

		return this;
	}

	public TableBuilder OrderBy( params OrderByOption[] options )
	{
		OrderByBuilder.Options.AddRange( options );

		return this;
	}

	/// <summary>
	///     Equal
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public TableBuilder Where( string name, object value )
	{
		Where( new WhereOption( GraphQL.Where.Equal, name, value ) );

		return this;
	}

	public TableBuilder Where( WhereType type, Where where, string name, object value )
	{
		Where( new WhereOption( type, where, name, value ) );

		return this;
	}


	public TableBuilder Where( Where where, string name, object value )
	{
		Where( new WhereOption( where, name, value ) );

		return this;
	}

	public TableBuilder Where( params WhereOption[] options )
	{
		WhereBuilder.Options.AddRange( options );

		return this;
	}

	public TableBuilder Field( params FieldBuilder[] fields )
	{
		Fields.AddRange( fields );

		return this;
	}

	public TableBuilder Field( params Hasura.CachedField[] fields )
	{
		foreach ( var field in fields )
		{
			Fields.Add( new FieldBuilder( field.Name ) );
		}

		return this;
	}

	public TableBuilder Field( params string[] fields )
	{
		foreach ( var field in fields )
		{
			Fields.Add( new FieldBuilder( field ) );
		}

		return this;
	}

	public TableBuilder Object<T>( T data )
	{
		try
		{
			ProcessObject( ( name, value, json ) =>
			{
				ObjectBuilder.Options.Add(
					!json ? new ObjectOption( name, value ) : new ObjectOption( name, value, true )
				);
			}, data );
		}
		catch ( Exception e )
		{
			Log.Error( e );
		}

		return this;
	}

	private void ProcessObject( Action<string, object, bool> finished, object obj )
	{
		var properties = TypeLibrary.GetPropertyDescriptions( obj );

		foreach ( var property in properties )
		{
			var propertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;

			// BUG: This does not work properly for non generic types now
			if ( (typeof(ITrackedValue).IsAssignableFrom( property.PropertyType ) && !property.HasAttribute<JsonIgnoreAttribute>()) ||
			     (TypeLibrary.GetType( property.PropertyType ).TargetType != typeof(TrackedList<>) && property.PropertyType.IsGenericType &&
			      SystemTypes.Contains( TypeLibrary.GetGenericArguments( property.PropertyType )[0] )) )
			{
				var trackedValue = (ITrackedValue)property.GetValue( obj );

				if ( trackedValue is not { IsSet: true } )
				{
					continue;
				}

				var name = ToSnakeCase( propertyName );

				finished.Invoke( name, trackedValue.Value, false );
			}
			else if ( !SystemTypes.Contains( property.PropertyType ) && property.PropertyType != typeof(string) &&
			          !typeof(IEnumerable).IsAssignableFrom( property.PropertyType ) && !property.HasAttribute<JsonIgnoreAttribute>() )
			{
				var value = property.GetValue( obj );

				if ( value is ITrackedValue trackedValue )
				{
					// Json array support
					var isGenericType = trackedValue.Value.GetType().IsGenericType;

					var variableName = ToSnakeCase( propertyName );

					if ( isGenericType && trackedValue.Value is IEnumerable enumerable )
					{
						if ( trackedValue.Value is IDictionary dict )
						{
							var arrayList = new List<Dictionary<string, object>>();

							foreach ( var key in dict.Keys )
							{
								var val = dict[key];

								ProcessNestedProperties( val, out var nestedPropertiesDict );
								arrayList.Add( nestedPropertiesDict );
							}

							Variable( variableName, "jsonb", arrayList );

							finished.Invoke( variableName, $"${variableName}", true );
						}
						else
						{
							var arrayList = enumerable.Cast<object>().ToList();

							ListVariable( variableName, arrayList );

							finished.Invoke( variableName, $"${variableName}", true );
						}

						continue;
					}

					value = trackedValue.Value;
				}

				// Json object support
				if ( value != null )
				{
					if ( !ProcessNestedProperties( value, out var nestedPropertiesDict ) )
					{
						continue;
					}

					var variableName = ToSnakeCase( property.Name );

					Variable( variableName, "jsonb", nestedPropertiesDict );

					finished.Invoke( ToSnakeCase( property.Name ), $"${variableName}", true );
				}
			}
		}
	}

	private bool ProcessNestedProperties( object value, out Dictionary<string, object> properties )
	{
		properties = new Dictionary<string, object>();
		var jsonChanged = false;

		if ( value != null )
		{
			var nestedProperties = TypeLibrary.GetPropertyDescriptions( value );

			foreach ( var nestedProperty in nestedProperties )
			{
				var nestedPropertyName = nestedProperty.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? nestedProperty.Name;

				if ( typeof(ITrackedValue).IsAssignableFrom( nestedProperty.PropertyType ) &&
				     !nestedProperty.HasAttribute<JsonIgnoreAttribute>() )
				{
					var nestedTrackedValue = (ITrackedValue)nestedProperty.GetValue( value );

					if ( nestedTrackedValue != null )
					{
						if ( nestedTrackedValue.IsSet )
						{
							jsonChanged = true;
						}

						var nestedName = ToSnakeCase( nestedPropertyName );
						properties[nestedName] = nestedTrackedValue.Value;
					}
				}
			}
		}

		return jsonChanged;
	}

	private string ToGrapQLJson( List<string> json )
	{
		return $"[ {string.Join( ", ", json )} ]";
	}

	private string ValueToString( object value )
	{
		if ( value is Vector3 )
		{
			value = value.ToString();
		}

		return value switch
		{
			string s => $"\"{s}\"".Replace( "\"\"", "\"" ),
			bool b => b.ToString().ToLower(),
			_ => value?.ToString()
		};
	}

	private string ToGraphQLJson( Dictionary<string, object> dictionary )
	{
		var json = "{ ";

		foreach ( var dict in dictionary )
		{
			json += $"{dict.Key}: {ValueToString( dict.Value )}, ";
		}

		json = json.TrimEnd( ' ', ',' );

		return $"{json} }}";
	}

	private string ToSnakeCase( string input )
	{
		if ( string.IsNullOrEmpty( input ) ) { return input; }

		var startUnderscores = Regex.Match( input, @"^_+" );
		return startUnderscores + Regex.Replace( input, @"([a-z0-9])([A-Z])", "$1_$2" ).ToLower();
	}

	public TableBuilder ListVariable( string name, IEnumerable<object> value )
	{
		string newValue;

		newValue = ToGrapQLJson( value.Select( ValueToString ).ToList() );

		var variable = new VariableBuilder( name, "jsonb", newValue );

		Variables.Add( variable );

		return this;
	}

	public TableBuilder Variable( string name, string type, List<Dictionary<string, object>> value )
	{
		var newValue = "";

		if ( type is "jsonb" )
		{
			newValue = ToGrapQLJson( value.Select( ToGraphQLJson ).ToList() );
		}

		var variable = new VariableBuilder( name, type, newValue );

		Variables.Add( variable );

		return this;
	}

	public TableBuilder Variable( string name, string type, object value )
	{
		switch ( type )
		{
			case "jsonb":
			{
				value = ToGraphQLJson( (Dictionary<string, object>)value );
				break;
			}
			case "string":
			{
				value = $"\"{value}\"".Replace( "\"\"", "\"" );
				break;
			}
		}

		var variable = new VariableBuilder( name, type, value );

		Variables.Add( variable );

		return this;
	}

	public TableBuilder Object( Dictionary<string, object> dict )
	{
		foreach ( var (key, value) in dict )
		{
			ObjectBuilder.Options.Add( new ObjectOption( key, value ) );
		}

		return this;
	}

	public TableBuilder Object( params ObjectOption[] options )
	{
		ObjectBuilder.Options.AddRange( options );

		return this;
	}

	public TableBuilder PKColumns( string key, object value )
	{
		PKColumnsBuilder.Options.Add( new ObjectOption( key, value ) );

		return this;
	}

	public TableBuilder Set<T>( T data )
	{
		try
		{
			ProcessObject( ( name, value, json ) =>
			{
				if ( !json )
				{
					SetBuilder.Options.Add( new ObjectOption( name, value ) );
					return;
				}

				SetBuilder.Options.Add( new ObjectOption( name, value, true ) );
			}, data );
		}
		catch ( Exception e )
		{
			Log.Error( e );
		}

		return this;
	}

	public TableBuilder Set( string key, object value )
	{
		SetBuilder.Options.Add( new ObjectOption( key, value ) );

		return this;
	}

	public TableBuilder Set( params ObjectOption[] options )
	{
		SetBuilder.Options.AddRange( options );

		return this;
	}

	public TableBuilder Set( Dictionary<string, object> dict )
	{
		foreach ( var (key, value) in dict )
		{
			SetBuilder.Options.Add( new ObjectOption( key, value ) );
		}

		return this;
	}

	public override string ToString()
	{
		var options = "";

		if ( PKColumnsBuilder.Options.Count > 0 )
		{
			options += $"{PKColumnsBuilder}, ";
		}

		if ( SetBuilder.Options.Count > 0 )
		{
			options += $"{SetBuilder}, ";
		}

		if ( ObjectBuilder.Options.Count > 0 )
		{
			options += $"{ObjectBuilder}, ";
		}

		if ( UniqueType.Length > 0 )
		{
			if ( UniqueAlwaysString || UniqueSearch is string )
			{
				UniqueSearch = $"\"{UniqueSearch}\"".Replace( "\"\"", "\"" );
			}

			options += $"{UniqueType}: {UniqueSearch}, ";
		}

		if ( QueryDistinctOn.Length > 0 )
		{
			options += $"distinct_on: {QueryDistinctOn}, ";
		}

		if ( QueryLimit > 0 )
		{
			options += $"limit: {QueryLimit}, ";
		}

		if ( QueryOffset > 0 )
		{
			options += $"offset: {QueryOffset}, ";
		}

		if ( OrderByBuilder.Options.Count > 0 )
		{
			options += $"{OrderByBuilder}, ";
		}

		if ( WhereBuilder.Options.Count > 0 )
		{
			options += WhereBuilder.ToString();
		}

		options = options.TrimEnd( ' ', ',' );

		return options.Length > 0
			? $"{TableName}({options}) {{ {string.Join( " ", Fields )} }}"
			: $"{TableName} {{ {string.Join( " ", Fields )} }}";
	}

	public class VariableBuilder
	{
		public VariableBuilder( string name, string type, object value )
		{
			Name = name;
			Type = type;
			Value = value;
		}

		public string Name { get; set; }
		public string Type { get; set; }
		public object Value { get; set; }

		public override string ToString()
		{
			return $"${Name}: {Type} = {Value}";
		}
	}
}
