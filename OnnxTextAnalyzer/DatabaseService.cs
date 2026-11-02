using Microsoft.Data.Sqlite;
using Dapper;

namespace OnnxTextAnalyzer;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=analyzer.db";

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            );
            CREATE TABLE IF NOT EXISTS Messages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Text TEXT NOT NULL,
                SpamProbability REAL NOT NULL,
                CategoryId INTEGER NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
            );
            CREATE INDEX IF NOT EXISTS IDX_Messages_Category ON Messages(CategoryId);
        ");

        connection.Execute("INSERT OR IGNORE INTO Categories (Name) VALUES ('Нормальне'), ('Спам')");
    }

    public void SaveMessage(string text, double probability)
    {
        using var connection = new SqliteConnection(_connectionString);
        int categoryId = probability > 0.8 ? 2 : 1; 

        string sql = "INSERT INTO Messages (Text, SpamProbability, CategoryId) VALUES (@Text, @Prob, @CatId)";
        connection.Execute(sql, new { Text = text, Prob = probability, CatId = categoryId });
    }
}