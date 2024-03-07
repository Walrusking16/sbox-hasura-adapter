using System.Collections.Generic;
using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.GraphQL;

[TestClass]
public class OptionsBuilderTest
{
	[TestMethod]
	public void OptionsBuilder_ToString()
	{
		var options = new OptionsBuilder<string>( "test" );
		options.Options.Add( "test1" );
		options.Options.Add( "test2" );
		Assert.AreEqual( "test: { test1, test2 }", options.ToString() );
	}
	
	[TestMethod]
	public void OptionsBuilder_ToString_Empty()
	{
		var options = new OptionsBuilder<string>( "test" );
		Assert.AreEqual( "test: {  }", options.ToString() );
	}
	
	[TestMethod]
	public void OptionsBuilder_ToString_Set()
	{
		var options = new OptionsBuilder<string>( "test" )
		{
			Options = new List<string> { "test1", "test2" }
		};
		Assert.AreEqual( "test: { test1, test2 }", options.ToString() );
	}
}
