> [!WARNING]  
> This is not ready for production use. It is under development.

> [!CAUTION]
> This is not fully tested and will be undergoing a refector soon.

## Requirements
This requires a [hasura](https://hasura.io/) server to be running. You can either use their cloud service or run it locally with docker-compose.
* [Cloud Quick Start](https://hasura.io/docs/latest/getting-started/getting-started-cloud/)
* [Docker Quick Start](https://hasura.io/docs/latest/getting-started/docker-simple/)

## Quickstart
Database setup:
* Create a schema called `test`
* Create a table called `todo` with the following columns:
    * id (integer) - Frequently used (auto-increment, primary key)
    * title (text)
    * is_completed (boolean) - Default value: false

Ingame code example:
```csharp
using System.Text.Json.Serialization;
using Database;

namespace Sandbox.Examples;

public class Test : Component
{
	protected override async void OnAwake()
	{
        // Used to initialize the Hasura client using default url ws://localhost:8080/v1/graphql
		Hasura.Init();
		
        // Get all the todo items from the database
		var res = await Hasura.Select<Todo>();
		
		Log.Info($"Total: {res.Items.Length}");
		
		foreach ( var todo in res.Items )
		{
			Log.Info( $"{todo.Id.Value} - {todo.Title.Value} - {todo.IsCompleted.Value}" );
		}
	}
	
    // Create a todo item
	[Table("test_todo")]
	public class Todo
	{
        // Use TrackedValue to track changes to the properties
		public TrackedValue<int> Id { get; set; }
		public TrackedValue<string> Title { get; set; }
        // Use the name of the column in the database
		[JsonPropertyName("is_completed")]
		public TrackedValue<bool> IsCompleted { get; set; }
	}
}

```