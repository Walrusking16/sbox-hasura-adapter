using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.GraphQL;

[TestClass]
public class TableBuilderTest
{
	[TestMethod]
	public void TableBuilder_Constructor_FieldBuilder()
	{
		var builder = new TableBuilder( "test_table", new FieldBuilder( "test_field" ) );
		Assert.AreEqual( "test_table { test_field }", builder.ToString() );
	}
	
	[TestMethod]
	public void TableBuilder_Constructor_CachedField()
	{
		var builder = new TableBuilder( "test_table", new Hasura.CachedField( "test_field" ) );
		Assert.AreEqual( "test_table { test_field }", builder.ToString() );
	}
	
	[TestMethod]
	public void TableBuilder_Constructor_StringFields()
	{
		var builder = new TableBuilder( "test_table", "test_field" );
		Assert.AreEqual( "test_table { test_field }", builder.ToString() );
	}
}
