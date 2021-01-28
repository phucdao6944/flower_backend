using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace flower.Repositories
{
    public abstract class BaseRepository
    {
        protected IConfiguration _configuration;
        protected string _connectionString;
        protected BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionString:DefaultConnection").Value;
            SqlMapper.Settings.CommandTimeout = int.Parse(_configuration.GetSection("ConnectionString:CommandTimeout").Value);
        }

        // use for buffered queries that return a type
        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await getData(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
                //throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        // use for buffered queries that do not return a type
        protected async Task WithConnection(Func<IDbConnection, Task> getData)
        {
            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await getData(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        //use for non-buffered queries that return a type
        protected async Task<TResult> WithConnection<TRead, TResult>(Func<IDbConnection, Task<TRead>> getData, Func<TRead, Task<TResult>> process)
        {
            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var data = await getData(connection);
                    return await process(data);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }


        protected async Task WithTransactionConnection(Func<IDbConnection, Task> processData)
        {
            try
            {
                // transaction scope
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // connection
                    await using (var connection = new SqlConnection(_connectionString))
                    {
                        // open connection asynchronously
                        await connection.OpenAsync();

                        await processData(connection);
                    }

                    scope.Complete();
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithTransactionConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithTransactionConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }

        }

        protected async Task WithBulkCopy(Func<IDbConnection, Task> processData)
        {
            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await processData(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithBulkCopy() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithBulkCopy() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

    }
}
