using Dapper.Contrib.Extensions;
using System.Data;

namespace SpellingTest.Web.Extension;

public static class UserExtensions
{
    public static string GetName(this string pal)
    {
        var index = pal.IndexOf('@');
        if (index > 0) pal = pal.Substring(0, index);
        return pal;
    }


    /// <summary>
    /// Inserts an entity into table "Ts" asynchronously using Task and returns identity id.
    /// </summary>
    /// <typeparam name="T">The type being inserted.</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="entityToInsert">Entity to insert</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <param name="sqlAdapter">The specific ISqlAdapter to use, auto-detected based on connection if null</param>
    /// <returns>Identity of inserted entity</returns>
    public static async Task<bool> TryInsertAsync<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null, ISqlAdapter sqlAdapter = null) where T : class
    {
        try
        {
            await connection.InsertAsync(entityToInsert, transaction, commandTimeout, sqlAdapter);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}