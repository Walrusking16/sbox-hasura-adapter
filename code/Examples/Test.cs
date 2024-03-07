using System.Text.Json.Serialization;
using Database;

namespace Sandbox.Examples;

public class Test : Component
{
	protected override async void OnAwake()
	{
		Hasura.Init(  );
		
		var res = await Hasura.Select<Todo>();
		
		Log.Info($"Total: {res.Items.Length}");
		
		foreach ( var todo in res.Items )
		{
			Log.Info( $"{todo.Id.Value} - {todo.Title.Value} - {todo.IsCompleted.Value}" );
		}
	}
	
	[Table("test_todo")]
	public class Todo
	{
		public TrackedValue<int> Id { get; set; }
		public TrackedValue<string> Title { get; set; }
		[JsonPropertyName("is_completed")]
		public TrackedValue<bool> IsCompleted { get; set; }
	}
}
