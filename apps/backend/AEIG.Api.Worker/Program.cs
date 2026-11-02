using AEIG.Api.Worker;
using AEIG.IoC;
using Microsoft.Data.Sqlite;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = ResolveSqlitePath(
    builder.Configuration.GetConnectionString("Default") ?? "Data Source=../AEIG.Api/Database/aeig.db",
    builder.Environment.ContentRootPath);
builder.Services.AddDependencyInjectionConfiguration(connectionString);
builder.Services.AddHostedService<CobrancaAtrasoWorker>();
builder.Services.AddHostedService<CobrancaPagamentoWorker>();

var host = builder.Build();
host.Run();

static string ResolveSqlitePath(string connectionString, string contentRoot)
{
    var sqlite = new SqliteConnectionStringBuilder(connectionString);
    if (sqlite.DataSource != ":memory:" && !Path.IsPathRooted(sqlite.DataSource))
        sqlite.DataSource = Path.GetFullPath(Path.Combine(contentRoot, sqlite.DataSource));

    var directory = Path.GetDirectoryName(sqlite.DataSource);
    if (!string.IsNullOrEmpty(directory))
        Directory.CreateDirectory(directory);

    return sqlite.ToString();
}
