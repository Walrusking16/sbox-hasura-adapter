using System;
using System.Text.Json;

namespace Database.Converters;

public class Vector3JsonConverter : HasuraConverter<TrackedValue<Vector3>>
{
	protected override TrackedValue<Vector3> Read( object value )
	{
		var newValue = (string)value;
		var split = newValue.Split( ',' );

		return new TrackedValue<Vector3>( new Vector3( float.Parse( split[0] ), float.Parse( split[1] ), float.Parse( split[2] ) ) );
	}

	protected override object Deserialize( ref Utf8JsonReader reader, JsonSerializerOptions options )
	{
		return JsonSerializer.Deserialize<string>( ref reader, options );
	}
}

public class Vector3ListJsonConverter : HasuraConverter<TrackedList<Vector3>>
{
	public override TrackedList<Vector3> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		var value = JsonSerializer.Deserialize<List<Vector3>>( ref reader, options );
		return new TrackedList<Vector3>( value );
	}
}
