using System.Collections.Generic;
using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.GraphQL;

[TestClass]
public class FieldBuilderTest
{
	[TestMethod]
	public void FieldBuilder_Constructor()
	{
		var field = new FieldBuilder( "test" );
		Assert.AreEqual( "test", field.ToString() );
	}
	
	[TestMethod]
	public void FieldBuilder_Field()
	{
		var field = new FieldBuilder( "test" );
		field.Field( new FieldBuilder( "test1" ), new FieldBuilder( "test2" ) );
		Assert.AreEqual( "test { test1 test2 }", field.ToString() );
	}
	
	[TestMethod]
	public void FieldBuilder_Field_CachedField()
	{
		var field = new FieldBuilder( "test" );
		field.Field( new Hasura.CachedField( "test1" ), new Hasura.CachedField( "test2" ) );
		Assert.AreEqual( "test { test1 test2 }", field.ToString() );
	}
	
	[TestMethod]
	public void FieldBuilder_Field_String()
	{
		var field = new FieldBuilder( "test" );
		field.Field( "test1", "test2" );
		Assert.AreEqual( "test { test1 test2 }", field.ToString() );
	}
	
	[TestMethod]
	public void FieldBuilder_Field_Set()
	{
		var field = new FieldBuilder( "test" )
		{
			SubFields = new List<FieldBuilder>
			{
				new( "test1" ),
				new( "test2" )
			}
		};
		Assert.AreEqual( "test { test1 test2 }", field.ToString() );
	}
	
}
