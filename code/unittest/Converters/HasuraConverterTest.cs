using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.Converters;

[TestClass]
public class HasuraConverterTest
{
	[TestMethod]
	public void HasuraConverter_ReadJson()
	{
		var converter = new HasuraConverter<HasuraConverterTestClass>();
		
		var json = "{\"Value\":\"test\"}";
		
		var input = new Utf8JsonReader( Encoding.UTF8.GetBytes( json ) );
		
		var result = converter.Read( ref input, typeof( HasuraConverterTestClass ), new JsonSerializerOptions() );
		
		Assert.AreEqual( "test", result.Value );
	}
	
	[TestMethod]
	public void HasuraConverter_WriteJson()
	{
		var converter = new HasuraConverter<HasuraConverterTestClass>();
		
		var value = new HasuraConverterTestClass
		{
			Value = "test"
		};
		
		using var stream = new MemoryStream();
		
		using var writer = new Utf8JsonWriter( stream );
		
		converter.Write( writer, value, new JsonSerializerOptions() );
		
		writer.Flush();
		
		var result = Encoding.UTF8.GetString( stream.ToArray() );
		
		Assert.AreEqual( "{\"Value\":\"test\"}", result );
	}
	
	[TestMethod]
	public void HasuraConverter_ReadJson_Tracked()
	{
		var converter = new HasuraConverter<HasuraConverterTestClassTracked>();
		
		var json = "{\"Value\":\"test\"}";
		
		var input = new Utf8JsonReader( Encoding.UTF8.GetBytes( json ) );
		
		var result = converter.Read( ref input, typeof( HasuraConverterTestClassTracked ), new JsonSerializerOptions() );
		
		Assert.AreEqual( "test", result.Value );
	}
	
	[TestMethod]
	public void HasuraConverter_WriteJson_Tracked()
	{
		var converter = new HasuraConverter<HasuraConverterTestClassTracked>();
		
		var value = new HasuraConverterTestClassTracked
		{
			Value = "test"
		};
		
		using var stream = new MemoryStream();
		
		using var writer = new Utf8JsonWriter( stream );
		
		converter.Write( writer, value, new JsonSerializerOptions() );
		
		writer.Flush();
		
		var result = Encoding.UTF8.GetString( stream.ToArray() );
		
		Assert.AreEqual( "\"test\"", result );
	}
	
	[TestMethod]
	public void HasuraConverter_ReadJson_Custom()
	{
		var converter = new HasuraConverter<HasuraConverterTestClassCustom>();
		
		var json = "{\"Value\": 5}";
		
		var input = new Utf8JsonReader( Encoding.UTF8.GetBytes( json ) );
		
		var result = converter.Read( ref input, typeof( HasuraConverterTestClassCustom ), new JsonSerializerOptions() );
		
		Assert.AreEqual( 5, result.Value );
	}
	
	[TestMethod]
	public void HasuraConverter_WriteJson_Custom()
	{
		var converter = new HasuraConverter<HasuraConverterTestClassCustom>();
		
		var value = new HasuraConverterTestClassCustom( 5 );
		
		using var stream = new MemoryStream();
		
		using var writer = new Utf8JsonWriter( stream );
		
		converter.Write( writer, value, new JsonSerializerOptions() );
		
		writer.Flush();
		
		var result = Encoding.UTF8.GetString( stream.ToArray() );
		
		Assert.AreEqual( "5", result );
	}
}

public class HasuraConverterTestClass
{
	public string Value { get; set; }
}

public class HasuraConverterTestClassTracked : TrackedValue<string>
{
	
}

public class HasuraConverterTestClassCustom : TrackedValue<long>
{
	public HasuraConverterTestClassCustom(long value)
	{
		Value = value;
	}
}

public class HasuraConverterTestClassCustomConverter : HasuraConverter<HasuraConverterTestClassCustom>
{
	protected override HasuraConverterTestClassCustom Read( object value )
	{
		return new HasuraConverterTestClassCustom(value as long? ?? 0);
	}
	
	protected override object Deserialize( ref Utf8JsonReader reader, JsonSerializerOptions options )
	{
		return JsonSerializer.Deserialize<long>( ref reader, options );
	}
}
