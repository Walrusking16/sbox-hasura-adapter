using System.Threading.Tasks;
using Database.GraphQL;

namespace Database;

public partial class Hasura
{
	public static async Task<T> Delete<T>( string table, string key, string value )
	{
		return await Delete<T>( table, key, (object) value );
	}
	
	public static async Task<T> Delete<T>( string table, string key, int value )
	{
		return await Delete<T>( table, key, (object) value );
	}
	
	private static async Task<T> Delete<T>( string table, string key, object value )
	{
		var builder = new Builder(GraphQL.Type.Mutation);
		var tbl = new TableBuilder( $"delete_{table}_by_pk", GetFieldsFromClass<T>() );

		tbl.Unique( key, value, false );
		
		builder.Table( tbl );
		
		var res = await SendRequest( builder );
		
		return res.Request.HasError ? default : FormatResponse<UniqueResponse<T>>( res.Id, builder ).Data;
	}
}
