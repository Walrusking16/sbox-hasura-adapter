using System;
using System.Linq;
using System.Threading.Tasks;
using Database.GraphQL;

namespace Database;

public partial class Hasura
{
	/// <summary>
	///     Gets all rows from the table from the class attribute
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<QueryResponse<T>> Select<T>()
	{
		return await Select<T>( GetClassBuilder<T>() );
	}

	/// <summary>
	///     Gets all rows from the table from the class attribute and allows modifying the builder
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<QueryResponse<T>> Select<T>( Action<Builder> modify )
	{
		var builder = GetClassBuilder<T>();
		modify( builder );
		return await Select<T>( builder );
	}

	/// <summary>
	///     Gets all rows from the table
	/// </summary>
	/// <param name="builder"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<QueryResponse<T>> Select<T>( Builder builder )
	{
		var res = await SendRequest( builder );

		return FormatResponse<QueryResponse<T>>( res.Id, builder, "items" );
	}

	/// <summary>
	///     Gets a users data by steamid
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<UniqueResponse<T>> Unique<T>( ulong steamid )
	{
		return await Unique<T>( "steamid", steamid );
	}

	/// <summary>
	///     Gets a single row from the table
	/// </summary>
	/// <param name="key">The column in the database to check</param>
	/// <param name="value">The value to find</param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<UniqueResponse<T>> Unique<T>( string key, object value )
	{
		var builder = GetClassBuilder<T>();
		var tbl = builder.FirstTable;
		tbl.TableName = $"{tbl.TableName}_by_pk";
		tbl.Unique( key, value );

		var res = await SendRequest( builder );

		var formatted = FormatResponse<UniqueResponse<T>>( res.Id, builder );

		formatted.IsValid = formatted.Data != null;

		return res.Request.HasError ? new UniqueResponse<T>() : formatted;
	}

	/// <summary>
	///     Automatically adds the _aggregate suffix to the table name, use colon to specify aggregate fields
	/// </summary>
	/// <param name="table"></param>
	/// <param name="fields"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<AggregateResponse<T>> Aggregate<T>( params string[] fields )
	{
		var builder = new Builder();

		var table = new TableBuilder( $"{GetCachedClass<T>().TableName}_aggregate" );

		var aggFields = new FieldBuilder( "aggregate" );

		var colonFields = fields.Where( x => x.Contains( ':' ) ).ToArray();

		foreach ( var colonField in colonFields )
		{
			var split = colonField.Split( ':' );
			var type = split[0].Trim().ToLower();
			var values = split[1].Trim().Split( ',' );

			if ( type == "count" )
			{
				aggFields.Field( type );
				continue;
			}

			aggFields.Field( new FieldBuilder( type ).Field( values ) );
		}

		if ( colonFields.Length > 0 )
		{
			table.Field( aggFields );
		}

		table.Field( new FieldBuilder( "nodes" ).Field( GetFieldsFromClass<T>() ) );

		builder.Table( table );

		var res = await SendRequest( builder );

		return res.Request.HasError ? new AggregateResponse<T>() : FormatResponse<InternalAggregateResponse<T>>( res.Id, builder ).Data;
	}

	public class QueryResponse<T>
	{
		public T[] Items { get; set; }
	}

	public class UniqueResponse
	{
		public bool IsValid { get; set; }
	}

	public class UniqueResponse<T> : UniqueResponse
	{
		public T Data { get; set; }
	}

	private class InternalAggregateResponse<T>
	{
		public AggregateResponse<T> Data { get; }
	}

	public class AggregateResponse<T>
	{
		public AggregateData? Aggregate { get; set; }
		public T[]? Nodes { get; set; }
	}
}
