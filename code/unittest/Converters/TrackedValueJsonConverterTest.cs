using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.Converters;

[TestClass]
public class TrackedValueJsonConverterTest
{
	[TestMethod]
	public void TrackedJsonValueConverter_ReadJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new TrackedValueJsonConverter<string>() );
		
		var json = "{\"Value\":\"test\"}";
		
		var result = JsonSerializer.Deserialize<TrackedValueJsonTestClass>( json, options );
		
		Assert.AreEqual( "test", result.Value.Value );
	}
	
	[TestMethod]
	public void TrackedJsonValueConverter_WriteJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new TrackedValueJsonConverter<string>() );
		
		var value = new TrackedValueJsonTestClass
		{
			Value = new TrackedValue<string>( "test" )
		};
		
		var result = JsonSerializer.Serialize( value, options );
		
		Assert.AreEqual( "{\"Value\":\"test\"}", result );
	}
}

[TestClass]
public class TrackedListJsonConverterTest
{
	[TestMethod]
	public void TrackedListValueConverter_ReadJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new TrackedListJsonConverter<string>() );

		var json = "{\"Value\":[\"test\"]}";

		var result = JsonSerializer.Deserialize<TrackedListJsonTestClass>( json, options );

		Assert.AreEqual( 1, result.Value.Value.Count );
		Assert.AreEqual( "test", result.Value.Value[0] );
	}
	
	[TestMethod]
	public void TrackedListValueConverter_WriteJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new TrackedListJsonConverter<string>() );
		
		var value = new TrackedListJsonTestClass
		{
			Value = new TrackedList<string>( new List<string> { "test" } )
		};
		
		var result = JsonSerializer.Serialize( value, options );
		
		Assert.AreEqual( "{\"Value\":[\"test\"]}", result );
	}
}

public class TrackedValueJsonTestClass
{
	public TrackedValue<string> Value { get; set; }
}

public class TrackedListJsonTestClass
{
	public TrackedList<string> Value { get; set; }
}
