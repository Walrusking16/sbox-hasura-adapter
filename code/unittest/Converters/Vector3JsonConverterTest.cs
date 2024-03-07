using System;
using System.Collections.Generic;
using System.Text.Json;
using Database.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sandbox.Internal;

namespace Database.Tests.Converters;

[TestClass]
public class Vector3JsonConverterTest
{
	[TestMethod]
	public void ReadJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new Vector3JsonConverter() );
		
		var json = "{\"Value\":\"1,2,3\"}";
		
		var result = JsonSerializer.Deserialize<Vector3JsonTest>( json, options );
		
		Assert.AreEqual( new Vector3( 1, 2, 3 ), result.Value.Value );
	}
	
	[TestMethod]
	public void WriteJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new Vector3JsonConverter() );
		
		var value = new Vector3JsonTest
		{
			Value = new Vector3( 1, 2, 3 )
		};
		
		var result = JsonSerializer.Serialize( value, options );
		
		Assert.AreEqual( "{\"Value\":\"1,2,3\"}", result );
	}
}

[TestClass]
public class Vector3ListJsonConverterTest
{
	[TestMethod]
	public void ReadJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new Vector3JsonConverter() );
		options.Converters.Add( new Vector3ListJsonConverter() );
		
		var json = "{\"Value\":[\"1,2,3\",\"4,5,6\"]}";
		
		var result = JsonSerializer.Deserialize<Vector3ListJsonTest>( json, options );
		
		Assert.AreEqual(2, result.Value.Value.Count);
		CollectionAssert.AreEqual( new List<Vector3> { new( 1, 2, 3 ), new( 4, 5, 6 ) }, result.Value.Value );
	}
	
	[TestMethod]
	public void WriteJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new Vector3JsonConverter() );
		options.Converters.Add( new Vector3ListJsonConverter() );
		
		var value = new Vector3ListJsonTest
		{
			Value = new List<Vector3> { new( 1, 2, 3 ), new( 4, 5, 6 ) }
		};
		
		var result = JsonSerializer.Serialize( value, options );
		
		Assert.AreEqual( "{\"Value\":[\"1,2,3\",\"4,5,6\"]}", result );
	}
}

public class Vector3JsonTest
{
	public TrackedValue<Vector3> Value { get; set; }
}

public class Vector3ListJsonTest
{
	public TrackedList<Vector3> Value { get; set; }
}
