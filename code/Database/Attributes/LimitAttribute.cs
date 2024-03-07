using System;

namespace Database;

[AttributeUsage(AttributeTargets.Class)]
public class LimitAttribute : Attribute
{
	public int Limit { get; set; }
	
	public LimitAttribute( int limit )
	{
		Limit = limit;
	}
}

