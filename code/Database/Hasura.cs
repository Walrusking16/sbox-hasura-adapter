using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using Database.GraphQL;
using Sandbox.Internal;

namespace Database;

// TODO: Implement subscriptions 
// TODO: Implement authentication
// TODO: Implement easy to use API for storing and retrieving data for specific players

[Flags]
public enum DebugLevel
{
	Query,
	Received,
	All
}

public class ClassCache
{
	public string TableName { get; set; }
	public int Limit { get; set; }
	public Hasura.CachedField[] Fields { get; set; }
}

public static partial class Hasura
{
	private static WebSocket _socket;
	private static Dictionary<int, Request> _openRequests = new();
	private static int _nextRequestId = 1;
	private static JsonSerializerOptions SerializerOptions;
	private static Dictionary<string, ClassCache> ClassFieldsCache = new();
	public static bool IsConnected => _socket.IsConnected && _connectionReceived;
	public static bool Debug { get; set; }
	public static DebugLevel DebugLevel { get; set; } = DebugLevel.All;
	
	private static bool _connectionReceived;
	
	/// <summary>
	///     Initializes the database connection
	/// </summary>
	/// <param name="url">The websocket url for graphql</param>
	/// <exception cref="DatabaseConnectionException"></exception>
	public static async void Init( string url = "ws://localhost:8080/v1/graphql" )
	{
		try
		{
			Shutdown();

			SerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IncludeFields = true };

			AddConverter( new TrackedValueJsonConverter<int>() );
			AddConverter( new TrackedListJsonConverter<int>() );
			AddConverter( new TrackedValueJsonConverter<long>() );
			AddConverter( new TrackedListJsonConverter<long>() );
			AddConverter( new TrackedValueJsonConverter<float>() );
			AddConverter( new TrackedListJsonConverter<float>() );
			AddConverter( new TrackedValueJsonConverter<double>() );
			AddConverter( new TrackedListJsonConverter<double>() );
			AddConverter( new TrackedValueJsonConverter<string>() );
			AddConverter( new TrackedListJsonConverter<string>() );
			AddConverter( new TrackedValueJsonConverter<bool>() );
			AddConverter( new TrackedListJsonConverter<bool>() );

			// does not work
			// var allowedTypes = new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(string), typeof(bool) };
			//
			// foreach ( var type in allowedTypes )
			// {
			// 	var converterType = GlobalGameNamespace.TypeLibrary.GetType( typeof(TrackedValueJsonConverter<>) )
			// 		.MakeGenericType( new[] { type } );
			// 	var converter = GlobalGameNamespace.TypeLibrary.Create( converterType.FullName, converterType );
			// 	AddConverter( converter );
			//
			// 	var listConverterType = GlobalGameNamespace.TypeLibrary.GetType( typeof(TrackedListJsonConverter<>) )
			// 		.MakeGenericType( new[] { type } );
			// 	var listConverter = GlobalGameNamespace.TypeLibrary.Create( listConverterType.FullName, listConverterType );
			// 	AddConverter( listConverter );
			// }

			foreach ( var desc in GlobalGameNamespace.TypeLibrary.GetTypes() )
			{
				var next = true;

				// This is a hack to ignore a random null reference exception that happens when getting the base type
				try
				{
					if ( desc?.BaseType == null )
					{
						next = true;
					}

					next = false;
				}
				catch
				{
					// ignored
				}

				if ( next )
				{
					continue;
				}

				if ( !(desc?.BaseType?.FullName?.Contains( "HasuraConverter" ) ?? false) )
				{
					continue;
				}

				AddConverter( GlobalGameNamespace.TypeLibrary.Create( desc.FullName, desc.TargetType, Array.Empty<object>() ) );
			}

			_socket = new WebSocket( 4194303 );

			_nextRequestId = 1;

			_socket.AddSubProtocol( "graphql-ws" );

			_socket.OnMessageReceived += HandleMessageReceived;

			await _socket.Connect( url );

			var connectionPayload = new { type = "connection_init", payload = new { lazy = true } };

			var jsonString = JsonSerializer.Serialize( connectionPayload );
			await _socket.Send( jsonString );
		}
		catch ( Exception e )
		{
			_socket.OnMessageReceived -= HandleMessageReceived;
			Log.Error( $"Hasura: {e.Message}" );
		}
	}

	private static void AddConverter<T>( T converter )
	{
		// Log.Info( converter );

		if ( converter is null || converter is not JsonConverter jsonConverter || SerializerOptions.Converters.Contains( jsonConverter ) )
		{
			return;
		}

		// Log.Info( $"Adding Converter: {jsonConverter}" );

		SerializerOptions.Converters.Add( jsonConverter );
	}

	public static async Task<bool> ValidateToken( long steamId, string token )
	{
		var data = new { steamid = steamId, token };
		var content = new StringContent( JsonSerializer.Serialize( data ), Encoding.UTF8, "application/json" );
		var result = await Http.RequestAsync( "https://services.facepunch.com/sbox/auth/token", "POST", content );

		if ( result.StatusCode != HttpStatusCode.OK )
		{
			return false;
		}

		var jsonString = await result.Content.ReadAsStringAsync();

		var response = JsonSerializer.Deserialize<ValidateAuthTokenResponse>( jsonString );

		if ( response is null || response.Status != "ok" )
		{
			return false;
		}

		return response.SteamId == steamId;
	}

	public static void Shutdown()
	{
		ClassFieldsCache.Clear();
		_socket?.Dispose();
		_openRequests.Clear();
		_connectionReceived = false;

		if ( _socket is null )
		{
			return;
		}

		_socket.OnMessageReceived -= HandleMessageReceived;
		
		_socket = null;
	}

	private static async Task<RequestData> SendRequest( Builder builder )
	{
		var jsonString = RequestString( builder, out var id );

		if (IsConnected)
		{
			return new RequestData { Request = await SendRequest( id, jsonString ), Id = id };
		}
		
		while ( !IsConnected )
		{
			await Task.Delay( 1 );
		}

		return new RequestData { Request = await SendRequest( id, jsonString ), Id = id };
	}

	private static string RequestString( Builder builder, out int id )
	{
		id = _nextRequestId;
		_nextRequestId++;
		var request = new { type = "start", id = $"{id}", payload = new { query = builder.ToString() } };

		var jsonString = JsonSerializer.Serialize( request );

		if ( Debug && DebugLevel is DebugLevel.All or DebugLevel.Query )
		{
			Log.Info( builder.ToString() );
		}

		return jsonString;
	}

	private static async Task<Request> SendRequest( int id, string json )
	{
		_openRequests[id] = new Request();

		await _socket.Send( json );

		var request = _openRequests[id];

		while ( !request.Completed )
		{
			await Task.Delay( 1 );
		}

		return request;
	}

	private static T FormatResponse<T>( int id, Builder builder, string changedName = "data" )
	{
		var doc = JsonDocument.Parse( _openRequests[id].Data.ToString() );

		var modified = new Dictionary<string, JsonElement>();
		foreach ( var prop in doc.RootElement.EnumerateObject() )
		{
			var name = prop.NameEquals( builder.Tables[0].TableName ) ? changedName : prop.Name;
			modified[name] = prop.Value;
		}

		var json = JsonSerializer.Serialize( modified, SerializerOptions );

		try
		{
			var payload = JsonSerializer.Deserialize<T>( json, SerializerOptions );

			_openRequests.Remove( id );

			return payload;
		}
		catch ( Exception e )
		{
			Log.Error( $"Error parsing response: {e.Message}" );
		}

		return default;
	}

	/// <summary>
	///     This creates a table builder with the table name and fields from the class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static Builder GetClassBuilder<T>( bool ignoreLimit = false )
	{
		var builder = new Builder();

		var cache = GetCachedClass<T>();

		builder.Table( cache.TableName, cache.Fields );

		if ( cache.Limit > 0 && !ignoreLimit )
		{
			builder.FirstTable.Limit( cache.Limit );
		}

		return builder;
	}

	private static string GetTableName<T>( T type )
	{
		var table = GlobalGameNamespace.TypeLibrary.GetAttribute<TableAttribute>( typeof(T) );

		return table.Name ?? typeof(T).Name.ToLower();
	}

	private static int GetLimit<T>( T type )
	{
		var limit = GlobalGameNamespace.TypeLibrary.GetAttribute<LimitAttribute>( typeof(T) );

		return limit?.Limit ?? -1;
	}

	public static ClassCache GetCachedClass<T>()
	{
		var name = typeof(T).Name;

		if ( ClassFieldsCache.TryGetValue( name, out var value ) )
		{
			return value;
		}

		var type = GlobalGameNamespace.TypeLibrary.GetType<T>().Create<T>();

		var cache = new ClassCache { TableName = GetTableName( type ), Fields = GetFieldsFromClass( type ), Limit = GetLimit( type ) };

		ClassFieldsCache[name] = cache;

		return cache;
	}

	// TODO: Support classes with nested classes
	public static CachedField[] GetFieldsFromClass<T>()
	{
		var cache = GetCachedClass<T>();

		return cache.Fields;
	}

	/// <summary>
	///     This will get all the properties from the class and converts them to snake case. This allows using
	///     JsonPropertyNameAttribute to change the name of the property
	/// </summary>
	/// <param name="type"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	private static CachedField[] GetFieldsFromClass<T>( T type )
	{
		var properties = GlobalGameNamespace.TypeLibrary.GetPropertyDescriptions( type )
			.Where( x => !x.HasAttribute<JsonIgnoreAttribute>() );

		return (from property in properties
			let attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>()
			let name = attribute != null
				? attribute.Name
				: string.Concat( property.Name.Select( ( x, i ) => i > 0 && char.IsUpper( x ) ? "_" + x : x.ToString() ) ).ToLower()
			select new CachedField(name)).ToArray();
	}

	private static void HandleMessageReceived( string data )
	{
		var res = JsonSerializer.Deserialize<Response>( data, SerializerOptions );

		if ( res.Type == "connection_ack")
		{
			_connectionReceived = true;
		}

		if ( res.Id == null )
		{
			return;
		}

		if ( Debug && DebugLevel is DebugLevel.All or DebugLevel.Received )
		{
			Log.Info( $"Received: {data}" );
		}

		var id = int.Parse( res.Id );
		
		switch ( res.Type )
		{
			case "complete":
				_openRequests[id].Completed = true;
				break;
			case "data":
				if ( CheckForErrors( res.Payload ) )
				{
					_openRequests[id].HasError = true;
					break;
				}

				_openRequests[id].Data = (JsonElement)res.Payload.Data;
				break;
			case "error":
				if ( CheckForErrors( res.Payload ) )
				{
					_openRequests[id].HasError = true;
					break;
				}

				if ( res.Type == "error" && res.Payload.Message != null )
				{
					Log.Error( $"Response Error: {res.Payload.Message}" );
					break;
				}

				Log.Error( "Something went wrong with the request" );
				break;
		}
	}

	private static bool CheckForErrors( PayloadResponse payload )
	{
		var hasErrors = payload.Errors != null;
		if ( payload.Errors != null )
		{
			Log.Error( $"Response Error: {payload.Errors.ToString()}" );
		}

		return hasErrors;
	}

	private class ValidateAuthTokenResponse
	{
		public string Status { get; }
		public long SteamId { get; }
	}

	public class Request
	{
		public bool HasError { get; set; }
		public bool Completed { get; set; }
		public JsonElement Data { get; set; }
	}

	public class RequestData
	{
		public Request Request { get; set; }
		public int Id { get; set; }
	}

	public class AggregateData
	{
		public int? Count { get; set; }
		public Dictionary<string, object> Max { get; set; }
		public Dictionary<string, object> Min { get; set; }
		public Dictionary<string, float> Sum { get; set; }
		public Dictionary<string, float> Avg { get; set; }
		public Dictionary<string, float> Variance { get; set; }
		public Dictionary<string, float> VarSamp { get; set; }
		public Dictionary<string, float> StdDev { get; set; }
		public Dictionary<string, float> StdDevPop { get; set; }
		public Dictionary<string, float> StdDevSamp { get; set; }
		public Dictionary<string, float> VarPop { get; set; }
	}

	public class CachedField
	{
		public string Name { get; set; }
		
		public CachedField( string name )
		{
			Name = name;
		}
	}
}
