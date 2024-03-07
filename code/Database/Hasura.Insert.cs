using System.Threading.Tasks;
using Database.GraphQL;

namespace Database;

public partial class Hasura
{
	/// <summary>
	/// Inserts a new row into the table
	/// </summary>
	/// <param name="table"></param>
	/// <param name="data"></param>
	/// <param name="fields"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<T> Insert<T>( T data )
	{
		var builder = GetClassBuilder<T>(true);
		var tbl = builder.FirstTable;
		tbl.TableName = $"insert_{tbl.TableName}_one";
		tbl.Object( data );
		builder.Type = Type.Mutation;
		
		var res = await SendRequest( builder );
		
		return res.Request.HasError ? default : FormatResponse<UniqueResponse<T>>( res.Id, builder ).Data;
	}
}
