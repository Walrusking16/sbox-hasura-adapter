using System;
using System.Collections.Generic;
using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.GraphQL;

[TestClass]
public class WhereBuilderTest
{
	[TestMethod]
	public void WhereBuilder_ToString()
	{
		var where = new WhereBuilder();
		where.Options.Add( new WhereOption( Where.Equal, "id", "test" ) );
		Assert.AreEqual( "where: { id: { _eq: \"test\" } }", where.ToString() );
	}
	
	[TestMethod]
	public void WhereBuilder_ToString_Empty()
	{
		var where = new WhereBuilder();
		Assert.AreEqual( "where: {  }", where.ToString() );
	}
	
	[TestMethod]
	public void WhereBuilder_ToString_Set()
	{
		var where = new WhereBuilder {
			Options = new List<WhereOption> {
				new( Where.Equal, "id", "test" )
			}
		};
		Assert.AreEqual( "where: { id: { _eq: \"test\" } }", where.ToString() );
	}
	
	[TestMethod]
	public void WhereBuilder_ToString_WhereAndType()
	{
		var where = new WhereBuilder();
		where.Options.Add( new WhereOption( WhereType.And, Where.Equal, "id", "test" ) );
		Assert.AreEqual( "where: { _and: { id: { _eq: \"test\" } } }", where.ToString() );
	}
	
	[TestMethod]
	public void WhereBuilder_ToString_WhereOrType()
	{
		var where = new WhereBuilder();
		where.Options.Add( new WhereOption( WhereType.Or, Where.Equal, "id", "test" ) );
		Assert.AreEqual( "where: { _or: { id: { _eq: \"test\" } } }", where.ToString() );
	}
	
	[TestMethod]
	public void WhereBuilder_ToString_WhereNotType()
	{
		var where = new WhereBuilder();
		where.Options.Add( new WhereOption( WhereType.Not, Where.Equal, "id", "test" ) );
		Assert.AreEqual( "where: { _not: { id: { _eq: \"test\" } } }", where.ToString() );
	}
}

[TestClass]
public class WhereOptionTest
{
	[TestMethod]
	public void WhereOption_ToString()
	{
		var option = new WhereOption( Where.Equal, "id", "test" );
		Assert.AreEqual( "id: { _eq: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_WhereType()
	{
		var option = new WhereOption( WhereType.And, Where.Equal, "id", "test" );
		Assert.AreEqual( "id: { _eq: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotEqual()
	{
		var option = new WhereOption( Where.NotEqual, "id", "test" );
		Assert.AreEqual( "id: { _neq: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_GreaterThan()
	{
		var option = new WhereOption( Where.GreaterThan, "id", "test" );
		Assert.AreEqual( "id: { _gt: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_GreaterThanOrEqual()
	{
		var option = new WhereOption( Where.GreaterThanOrEqual, "id", "test" );
		Assert.AreEqual( "id: { _gte: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_LessThan()
	{
		var option = new WhereOption( Where.LessThan, "id", "test" );
		Assert.AreEqual( "id: { _lt: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_LessThanOrEqual()
	{
		var option = new WhereOption( Where.LessThanOrEqual, "id", "test" );
		Assert.AreEqual( "id: { _lte: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_In()
	{
		var option = new WhereOption( Where.In, "id", "test" ) ;
		Assert.AreEqual( "id: { _in: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotIn()
	{
		var option = new WhereOption( Where.NotIn, "id", "test" );
		Assert.AreEqual( "id: { _nin: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_IsNull()
	{
		var option = new WhereOption( Where.IsNull, "id", "test" );
		Assert.AreEqual( "id: { _is_null: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_InsensitiveLike()
	{
		var option = new WhereOption( Where.InsensitiveLike, "id", "test" );
		Assert.AreEqual( "id: { _ilike: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotInsensitiveLike()
	{
		var option = new WhereOption( Where.NotInsensitiveLike, "id", "test" );
		Assert.AreEqual( "id: { _nilike: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_Like()
	{
		var option = new WhereOption( Where.Like, "id", "test" );
		Assert.AreEqual( "id: { _like: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotLike()
	{
		var option = new WhereOption( Where.NotLike, "id", "test" );
		Assert.AreEqual( "id: { _nlike: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_Regex()
	{
		var option = new WhereOption( Where.Regex, "id", "test" );
		Assert.AreEqual( "id: { _regex: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotRegex()
	{
		var option = new WhereOption( Where.NotRegex, "id", "test" );
		Assert.AreEqual( "id: { _nregex: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_InsensitiveRegex()
	{
		var option = new WhereOption( Where.InsensitiveRegex, "id", "test" );
		Assert.AreEqual( "id: { _iregex: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotInsensitiveRegex()
	{
		var option = new WhereOption( Where.NotInsensitiveRegex, "id", "test" );
		Assert.AreEqual( "id: { _niregex: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_NotSimilar()
	{
		var option = new WhereOption( Where.NotSimilar, "id", "test" );
		Assert.AreEqual( "id: { _nsimilar: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOption_ToString_Similar()
	{
		var option = new WhereOption( Where.Similar, "id", "test" );
		Assert.AreEqual( "id: { _similar: \"test\" }", option.ToString() );
	}
	
	[TestMethod]
	public void WhereOptions_ToString_Unknown()
	{
		var option = new WhereOption( ( Where ) 100, "id", "test" );
		Assert.ThrowsException<NotImplementedException>( () => option.ToString() );
	}
}
