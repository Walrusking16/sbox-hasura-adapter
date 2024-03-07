using Database.GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Tests.GraphQL;

[TestClass]
public class ResponseTest
{
	[TestMethod]
	public void Response_Constructor()
	{
		var response = new Response();
		
		Assert.IsNotNull( response );
	}
	
	[TestMethod]
	public void Response_Constructor_WithData()
	{
		var response = new Response()
		{
			Id = "test",
			Payload = new PayloadResponse(),
			Type = "test"
		};
		
		Assert.IsNotNull( response );
		Assert.AreEqual( "test", response.Id );
		Assert.IsNotNull( response.Payload );
		Assert.AreEqual( "test", response.Type );
	}
}

[TestClass]
public class PayloadResponseTest
{
	[TestMethod]
	public void PayloadResponse_Constructor()
	{
		var response = new PayloadResponse();
		
		Assert.IsNotNull( response );
	}
	
	[TestMethod]
	public void PayloadResponse_Constructor_WithData()
	{
		var response = new PayloadResponse
		{
			Data = "test",
			Errors = "test",
			Message = "test"
		};
		
		Assert.IsNotNull( response );
		Assert.AreEqual( "test", response.Data );
		Assert.AreEqual( "test", response.Errors );
		Assert.AreEqual( "test", response.Message );
	}
}
