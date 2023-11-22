using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using PolyhydraGames.Data.Dapper;

namespace SpellingTest.Web.Services;

public class BaseApi
{
    private IDBConnectionFactory _connectionFactory;
    protected IDbConnection GetConnection() => _connectionFactory.GetConnection();


    public BaseApi(IDBConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<T>> Execute<T>(string spName, params object[] objs) where T : IEnumerable
    {
        using var connection = GetConnection();
        var results = connection.Query<T>(spName, objs, commandType: CommandType.StoredProcedure).ToList();
        return results;
    }
    public async Task<T> GetItem<T>(IDBConnectionFactory factory, [CallerMemberName] string memberName = "")

    {
        var sql = $"exec {this.GetStoredProcedureValue(memberName)} ";
        using var con = factory.GetConnection();
        return (await con.QueryAsync<T>(sql)).FirstOrDefault();
    }

    public async Task<T> GetItem<T>(IDBConnectionFactory factory, ITuple tuple,
        [CallerMemberName] string memberName = "")

    {
        var sql = $"exec {this.GetStoredProcedureValue(memberName)} ";
        using var con = factory.GetConnection();
        return (await con.QueryAsync<T>(sql, tuple, commandType: CommandType.StoredProcedure)).FirstOrDefault();
    }


    public async Task<IEnumerable<T>> GetItems<T>(object owner, IDBConnectionFactory factory,
        [CallerMemberName] string memberName = "")

    {
        var sql = $"exec {owner.GetStoredProcedureValue(memberName)} ";
        using var con = factory.GetConnection();
        return await con.QueryAsync<T>(sql);
    }
}