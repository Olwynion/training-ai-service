using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Training.AI.Domain.Repositories;

namespace Training.AI.Infrastructure.Factories;

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
