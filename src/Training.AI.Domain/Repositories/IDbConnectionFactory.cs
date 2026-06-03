namespace Training.AI.Domain.Repositories;

public interface IDbConnectionFactory
{
    System.Data.IDbConnection CreateConnection();
}
