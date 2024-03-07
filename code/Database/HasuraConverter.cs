using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Database;

public class HasuraConverter<T> : JsonConverter<T>
{
	/// <summary>
	/// This will convert the value from the database to the type of the object
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	protected virtual T Read( object value )
	{
		return (T)value;
	}

	/// <summary>
	/// This will convert the object to the value that will be stored in the database, most likely won't need to change
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	protected virtual object Write( in T value )
	{
		if ( value is ITrackedValue trackedValue )
		{
			return trackedValue.Value;
		}
		
		return value;
	}

	/// <summary>
	/// This is how the object is deserialized from the database
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	protected virtual object Deserialize( ref Utf8JsonReader reader, JsonSerializerOptions options )
	{
		return JsonSerializer.Deserialize<T>( ref reader, options );
	}

	/// <summary>
	/// This is the the method used by the JsonConverter to convert the object, most likely won't need to change
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public override T Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		return Read( Deserialize( ref reader, options ) );
	}

	/// <summary>
	/// This is the method used by the JsonConverter to convert the object, most likely won't need to change
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="value"></param>
	/// <param name="options"></param>
	public override void Write( Utf8JsonWriter writer, T value, JsonSerializerOptions options )
	{
		JsonSerializer.Serialize( writer, Write( value ), options );
	}
}
