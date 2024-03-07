namespace Database.GraphQL;

public class OptionsBuilder<T> : OptionBuilder<T>
{
	private string Name { get; set; }
	
	public OptionsBuilder( string name )
	{
		Name = name;
	}
	
	public override string ToString()
	{
		return $"{Name}: {{ {string.Join( ", ", Options )} }}";
	}
}
