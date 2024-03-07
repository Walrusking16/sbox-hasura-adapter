using System.Threading.Tasks;
using Database.GraphQL;

namespace Database;

public partial class Hasura
{
	public static async Task<T> Update<T>( ulong steamid, T data )
	{
		return await Update( "steamid", steamid, data );
	}

	public static async Task<T> Update<T>( string key, object value, T data )
	{
		var builder = GetClassBuilder<T>( true );
		var tbl = builder.FirstTable;
		tbl.TableName = $"update_{tbl.TableName}_by_pk";
		builder.Type = Type.Mutation;

		tbl.PKColumns( key, value )
			.Set( data );

		var res = await SendRequest( builder );

		return res.Request.HasError ? default : FormatResponse<UniqueResponse<T>>( res.Id, builder ).Data;
	}
}
