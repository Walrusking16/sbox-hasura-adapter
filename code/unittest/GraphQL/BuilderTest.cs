using System;
using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Type = Database.GraphQL.Type;

namespace Database.Tests.GraphQL;

[TestClass]
public class BuilderTest
{
	// Skipping this test because TypeLibrary does not exist in unit testing
	// [TestMethod]
	// public void Builder_FromClass()
	// {
	// 	var builder = Builder.FromClass<TestBuilderClass>();
	// 	Assert.AreEqual( "test_table", builder.ToString() );
	// }
	
	[TestMethod]
	public void Builder_Table()
	{
		var builder = new Builder();
		builder.Table( "test_table" );
		Assert.AreEqual( "query GetData { test_table {  } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Table_Fields()
	{
		var builder = new Builder();
		builder.Table( "test_table", "test_field" );
		Assert.AreEqual( "query GetData { test_table { test_field } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Table_Fields_WithFieldBuilder()
	{
		var builder = new Builder();
		builder.Table( "test_table", new FieldBuilder( "test_field" ) );
		Assert.AreEqual( "query GetData { test_table { test_field } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Table_Fields_WithCachedFields()
	{
		var builder = new Builder();
		builder.Table( "test_table", new Hasura.CachedField( "test_field" ), new Hasura.CachedField( "test_field2" ) );
		Assert.AreEqual( "query GetData { test_table { test_field test_field2 } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Constructor()
	{
		var builder = new Builder( "test_table", "test_field" );
		Assert.AreEqual( "query GetData { test_table { test_field } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Constructor_WithFieldBuilder()
	{
		var builder = new Builder( "test_table", new FieldBuilder( "test_field" ) );
		Assert.AreEqual( "query GetData { test_table { test_field } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Constructor_WithCachedFields()
	{
		var builder = new Builder( "test_table", new Hasura.CachedField( "test_field" ), new Hasura.CachedField( "test_field2" ) );
		Assert.AreEqual( "query GetData { test_table { test_field test_field2 } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_Table_WithTableBuilder()
	{
		var builder = new Builder();
		builder.Table( new TableBuilder( "test_table" ) );
		Assert.AreEqual( "query GetData { test_table {  } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_ToString_Mutation()
	{
		var builder = new Builder(Type.Mutation);
		builder.Table( "test_table" );
		Assert.AreEqual( "mutation SetData { test_table {  } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_ToString_Subscription()
	{
		var builder = new Builder(Type.Subscription);
		builder.Table( "test_table" );
		Assert.AreEqual( "subscription SubscribeData { test_table {  } }", builder.ToString() );
	}
	
	[TestMethod]
	public void Builder_ToString_UnknownType()
	{
		var builder = new Builder((Type) 100);
		builder.Table( "test_table" );
		Assert.ThrowsException<ArgumentOutOfRangeException>( () => builder.ToString() );
	}

	// [TestMethod]
	// public void Builder_WithVariables()
	// {
	// 	var builder = new Builder(Type.Mutation);
	// 	var tbl = builder.Table( "test_table" );
	// 	tbl.Variable( "data", "Text", "test" );
	// 	
	// 	Assert.AreEqual( "mutation SetData($data: Text = \"test\") { test_table() {  } }", builder.ToString() );
	// }
}

[Table("test_table")]
public class TestBuilderClass {
	public TrackedValue<string> TestField { get; set; }
}
