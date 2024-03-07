using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Database;

public interface ITrackedValue
{
	bool IsSet { get; }
	object Value { get; }
}

/// <summary>
///     Used to track if a value has been set or not, for use with update queries
/// </summary>
/// <typeparam name="T"></typeparam>
public class TrackedValue<T> : ITrackedValue
{
	private T _value;

	public TrackedValue()
	{
		_value = default;
	}

	public TrackedValue( T value, bool isSet = false )
	{
		IsSet = isSet;
		_value = value;
	}

	public T Value
	{
		get => _value;
		set
		{
			_value = value;
			IsSet = true;
		}
	}

	public bool IsSet { get; set; }

	bool ITrackedValue.IsSet => IsSet;
	object ITrackedValue.Value => Value;

	public static implicit operator TrackedValue<T>( T value )
	{
		return new TrackedValue<T>( value, true );
	}

	public static implicit operator T( TrackedValue<T> trackedValue )
	{
		return trackedValue.Value;
	}
}

public class TrackedList<T> : ITrackedValue
{
	private List<T> _value;

	public TrackedList()
	{
		_value = new List<T>();
	}

	public TrackedList( List<T> value, bool isSet = false )
	{
		IsSet = isSet;
		_value = value;
	}

	public T this[ int index ]
	{
		get => _value[index];
		set
		{
			_value[index] = value;
			IsSet = true;
		}
	}

	public List<T> Value
	{
		get => _value;
		set
		{
			_value = value;
			IsSet = true;
		}
	}

	public bool IsSet { get; set; }

	bool ITrackedValue.IsSet => IsSet;
	object ITrackedValue.Value => Value;

	public void Add( T value )
	{
		_value.Add( value );
		IsSet = true;
	}

	public void Remove( T value )
	{
		_value.Remove( value );
		IsSet = true;
	}

	public void RemoveAt( int index )
	{
		_value.RemoveAt( index );
		IsSet = true;
	}

	public void Clear()
	{
		_value.Clear();
		IsSet = true;
	}

	public bool Contains( T value )
	{
		return _value.Contains( value );
	}

	public int IndexOf( T value )
	{
		return _value.IndexOf( value );
	}

	public void Insert( int index, T value )
	{
		_value.Insert( index, value );
		IsSet = true;
	}

	public void Sort()
	{
		_value.Sort();
		IsSet = true;
	}

	public void Sort( Comparison<T> comparison )
	{
		_value.Sort( comparison );
		IsSet = true;
	}

	public void Sort( IComparer<T> comparer )
	{
		_value.Sort( comparer );
		IsSet = true;
	}

	public void Reverse()
	{
		_value.Reverse();
		IsSet = true;
	}

	public void Reverse( int index, int count )
	{
		_value.Reverse( index, count );
		IsSet = true;
	}

	public static implicit operator TrackedList<T>( List<T> value )
	{
		return new TrackedList<T>( value, true );
	}

	public static implicit operator List<T>( TrackedList<T> trackedValue )
	{
		return trackedValue.Value;
	}
}

public class TrackedValueJsonConverter<T> : JsonConverter<TrackedValue<T>>
{
	public override TrackedValue<T> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		var value = JsonSerializer.Deserialize<T>( ref reader, options );
		return new TrackedValue<T>( value );
	}

	public override void Write( Utf8JsonWriter writer, TrackedValue<T> value, JsonSerializerOptions options )
	{
		JsonSerializer.Serialize( writer, value.Value, options );
	}
}

public class TrackedListJsonConverter<T> : JsonConverter<TrackedList<T>>
{
	public override TrackedList<T> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		var value = JsonSerializer.Deserialize<List<T>>( ref reader, options );
		return new TrackedList<T>( value );
	}

	public override void Write( Utf8JsonWriter writer, TrackedList<T> value, JsonSerializerOptions options )
	{
		JsonSerializer.Serialize( writer, value.Value, options );
	}
}
