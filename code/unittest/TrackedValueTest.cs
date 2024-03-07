using System.Collections.Generic;
using System.Text.Json;
using Database;
using Database.Tests.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests;

[TestClass]
public class TrackedValueTest
{
	[TestMethod]
	public void TrackedValue_ReadJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new TrackedValueJsonConverter<string>() );
		
		var json = "{\"Value\":\"test\"}";
		
		var result = JsonSerializer.Deserialize<TrackedValueJsonTestClass>( json, options );
		
		Assert.AreEqual( "test", result.Value.Value );
	}
	
	[TestMethod]
	public void TrackedValue_WriteJson()
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
	
	[TestMethod]
	public void TrackedValue_Constructor()
	{
		var value = new TrackedValue<string>();
		
		Assert.AreEqual( default, value.Value );
		Assert.AreEqual( false, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedValue_Constructor_WithValue()
	{
		var value = new TrackedValue<string>( "test" );
		
		Assert.AreEqual( "test", value.Value );
		Assert.AreEqual( false, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedValue_SetValue()
	{
		var value = new TrackedValue<string> { Value = "test" };

		Assert.AreEqual( "test", value.Value );
		Assert.AreEqual( true, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedValue_SetValue_ITrackedValue()
	{
		var value = new TrackedValue<string> { Value = "test" };
		value.Value = "test2";
		
		var iTrackedValue = (ITrackedValue) value;
		
		Assert.AreEqual( "test2", iTrackedValue.Value );
		Assert.AreEqual( true, iTrackedValue.IsSet );
	}

	[TestMethod]
	public void TrackedValue_ImplicitOperator()
	{
		TrackedValue<string> value = "test";

		Assert.AreEqual( "test", value.Value );
		Assert.AreEqual( true, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedValue_ImplicitOperator_FromTrackedValue()
	{
		var value = new TrackedValue<string>( "test" );
		string result = value;
		
		Assert.AreEqual( "test", result );
	}
}

[TestClass]
public class TrackedListTest
{
	[TestMethod]
	public void TrackedList_ReadJson()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add( new TrackedListJsonConverter<string>() );

		var json = "{\"Value\":[\"test\"]}";

		var result = JsonSerializer.Deserialize<TrackedListJsonTestClass>( json, options );

		Assert.AreEqual( 1, result.Value.Value.Count );
		Assert.AreEqual( "test", result.Value.Value[0] );
	}
	
	[TestMethod]
	public void TrackedList_WriteJson()
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
	
	[TestMethod]
	public void TrackedList_Constructor()
	{
		var value = new TrackedList<string>();
		
		Assert.AreEqual( 0, value.Value.Count );
		Assert.AreEqual( false, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedList_Constructor_WithValue()
	{
		var value = new TrackedList<string>( new List<string> { "test" } );
		
		Assert.AreEqual( 1, value.Value.Count );
		Assert.AreEqual( "test", value.Value[0] );
		Assert.AreEqual( false, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedList_SetValue()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };

		Assert.AreEqual( 1, value.Value.Count );
		Assert.AreEqual( "test", value.Value[0] );
		Assert.AreEqual( true, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedList_SetValue_ITrackedValue()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		value.Value = new List<string> { "test2" };
		
		var iTrackedValue = (ITrackedValue) value;
		
		var list = (List<string>) iTrackedValue.Value;
		
		Assert.AreEqual( 1, list.Count );
		Assert.AreEqual( "test2", list[0] );
		Assert.AreEqual( true, iTrackedValue.IsSet );
	}
	
	[TestMethod]
	public void TrackedList_This()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		
		Assert.AreEqual( 1, value.Value.Count );
		Assert.AreEqual( "test", value[0] );
		Assert.AreEqual( true, value.IsSet );
	}

	[TestMethod]
	public void TrackedList_This_Set()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		value[0] = "test2";
		
		Assert.AreEqual( 1, value.Value.Count );
		Assert.AreEqual( "test2", value[0] );
		Assert.AreEqual( true, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedList_Add()
	{
		var value = new TrackedList<string>();
		value.Add( "test" );
		
		Assert.AreEqual( 1, value.Value.Count );
		Assert.AreEqual( "test", value[0] );
		Assert.AreEqual( true, value.IsSet );
	}

	[TestMethod]
	public void TrackedList_Clear()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		value.Clear();
		
		Assert.AreEqual( 0, value.Value.Count );
	}
	
	[TestMethod]
	public void TrackedList_Contains()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		
		Assert.AreEqual( true, value.Contains( "test" ) );
	}
	
	[TestMethod]
	public void TrackedList_IndexOf()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		
		Assert.AreEqual( 0, value.IndexOf( "test" ) );
	}
	
	[TestMethod]
	public void TrackedList_Insert()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		value.Insert( 0, "test2" );
		
		Assert.AreEqual( 2, value.Value.Count );
		Assert.AreEqual( "test2", value[0] );
		Assert.AreEqual( "test", value[1] );
	}
	
	[TestMethod]
	public void TrackedList_Remove()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		value.Remove( "test" );
		
		Assert.AreEqual( 0, value.Value.Count );
	}
	
	[TestMethod]
	public void TrackedList_RemoveAt()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test" } };
		value.RemoveAt( 0 );
		
		Assert.AreEqual( 0, value.Value.Count );
	}
	
	[TestMethod]
	public void TrackedList_Reverse()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test", "test2" } };
		value.Reverse();
		
		Assert.AreEqual( 2, value.Value.Count );
		Assert.AreEqual( "test2", value[0] );
		Assert.AreEqual( "test", value[1] );
	}
	
	[TestMethod]
	public void TrackedList_Reverse_WithCount()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test", "test2" } };
		value.Reverse( 1, 1 );
		
		Assert.AreEqual( 2, value.Value.Count );
		Assert.AreEqual( "test", value[0] );
		Assert.AreEqual( "test2", value[1] );
	}
	
	[TestMethod]
	public void TrackedList_Sort()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test2", "test" } };
		value.Sort();
		
		Assert.AreEqual( 2, value.Value.Count );
		Assert.AreEqual( "test", value[0] );
		Assert.AreEqual( "test2", value[1] );
	}
	
	[TestMethod]
	public void TrackedList_Sort_WithComparison()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test2", "test" } };
		value.Sort( string.Compare );
		
		Assert.AreEqual( 2, value.Value.Count );
		Assert.AreEqual( "test", value[0] );
		Assert.AreEqual( "test2", value[1] );
	}
	
	[TestMethod]
	public void TrackedList_Sort_Comparer()
	{
		var value = new TrackedList<string> { Value = new List<string> { "test2", "test" } };
		value.Sort( Comparer<string>.Default );
		
		Assert.AreEqual( 2, value.Value.Count );
		Assert.AreEqual( "test", value[0] );
		Assert.AreEqual( "test2", value[1] );
	}
	
	[TestMethod]
	public void TrackedList_ImplicitOperator()
	{
		TrackedList<string> value = new List<string> { "test" };

		Assert.AreEqual( 1, value.Value.Count );
		Assert.AreEqual( "test", value.Value[0] );
		Assert.AreEqual( true, value.IsSet );
	}
	
	[TestMethod]
	public void TrackedList_ImplicitOperator_FromTrackedList()
	{
		var value = new TrackedList<string>( new List<string> { "test" } );
		List<string> result = value;
		
		Assert.AreEqual( 1, result.Count );
		Assert.AreEqual( "test", result[0] );
	}
}
