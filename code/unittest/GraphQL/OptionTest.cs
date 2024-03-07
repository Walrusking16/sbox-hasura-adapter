using System;
using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.GraphQL;

[TestClass]
public class OptionTest
{
	[TestMethod]
	public void Option_Constructor()
	{
		var option = new Option( "test" );
		Assert.AreEqual( "test", option.Name );
	}
}

[TestClass]
public class ObjectOptionTest
{
	[TestMethod]
	public void ObjectOption_Constructor()
	{
		var option = new ObjectOption( "test", "test" );
		Assert.AreEqual( "test", option.Value );
	}
	
	[TestMethod]
	public void ObjectOption_Constructor_IgnoreQuotes()
	{
		var option = new ObjectOption( "test", "test", true );
		Assert.AreEqual( "test", option.Value );
	}
	
	[TestMethod]
	public void ObjectOption_ToString()
	{
		var option = new ObjectOption( "test", "test" );
		Assert.AreEqual( "test: \"test\"", option.ToString() );
	}
}

[TestClass]
public class OrderByOptionTest
{
	[TestMethod]
	public void OrderByOption_Constructor()
	{
		var option = new OrderByOption( OrderBy.Asc, "test" );
		Assert.AreEqual( OrderBy.Asc, option.OrderBy );
	}
	
	[TestMethod]
	public void OrderByOption_GetId()
	{
		var option = new OrderByOption( OrderBy.Asc, "test" );
		Assert.AreEqual( "asc", option.GetId() );
	}
	
	[TestMethod]
	public void OrderByOption_GetId_AscNullsFirst()
	{
		var option = new OrderByOption( OrderBy.AscNullsFirst, "test" );
		Assert.AreEqual( "asc_nulls_first", option.GetId() );
	}
	
	[TestMethod]
	public void OrderByOption_GetId_AscNullsLast()
	{
		var option = new OrderByOption( OrderBy.AscNullsLast, "test" );
		Assert.AreEqual( "asc_nulls_last", option.GetId() );
	}
	
	[TestMethod]
	public void OrderByOption_GetId_Desc()
	{
		var option = new OrderByOption( OrderBy.Desc, "test" );
		Assert.AreEqual( "desc", option.GetId() );
	}
	
	[TestMethod]
	public void OrderByOption_GetId_DescNullsFirst()
	{
		var option = new OrderByOption( OrderBy.DescNullsFirst, "test" );
		Assert.AreEqual( "desc_nulls_first", option.GetId() );
	}
	
	[TestMethod]
	public void OrderByOption_GetId_DescNullsLast()
	{
		var option = new OrderByOption( OrderBy.DescNullsLast, "test" );
		Assert.AreEqual( "desc_nulls_last", option.GetId() );
	}

	[TestMethod]
	public void OrderByOption_GetId_Unknown()
	{
		var option = new OrderByOption( (OrderBy) 100, "test" );
		Assert.ThrowsException<NotImplementedException>( () => option.GetId());
	}
	
	[TestMethod]
	public void OrderByOption_ToString()
	{
		var option = new OrderByOption( OrderBy.Asc, "test" );
		Assert.AreEqual( "test: asc", option.ToString() );
	}
}
