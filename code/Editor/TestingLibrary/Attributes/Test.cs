using System;

namespace TestingLibrary;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class Test : Attribute
{
	public string Name;

	public Test()
	{
		Name = null;
	}

	public Test( string name )
	{
		Name = name;
	}
}
