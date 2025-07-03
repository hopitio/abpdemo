# SQL Logging Configuration

This project has been configured to log SQL queries to a separate file.

## Configuration Details

### Files Modified:
1. **Program.cs** - Added SQL-specific logging configuration
2. **AbpAngular.HttpApi.Host.csproj** - Added required Serilog packages:
   - `Serilog.Sinks.File` - For file logging
   - `Serilog.Expressions` - For filtering expressions

### Log Files Created:
- **Logs/logs.txt** - General application logs
- **Logs/sql-[date].log** - SQL queries and Entity Framework logs only

### How It Works:
1. The application uses Serilog for structured logging
2. Entity Framework logging levels are set to Debug in development
3. A separate logger is configured to filter only Entity Framework logs
4. SQL logs are written to daily rolling files with the format: `sql-yyyy-MM-dd.log`

### SQL Log Format:
```
2025-07-03 10:30:45.123 [SQL] Executed DbCommand (2ms) [Parameters=[@p0='John' (Size = 4000)], CommandType='Text', CommandTimeout='30']
SELECT [u].[Id], [u].[Name] FROM [Users] AS [u] WHERE [u].[Name] = @p0
```

### Testing:
To test the SQL logging:
1. Run the application: `dotnet run`
2. Make some database operations through the API
3. Check the `Logs/sql-[date].log` file for SQL queries
4. Check the `Logs/logs.txt` file for general application logs

### Log Rotation:
- Files are rotated daily
- Each file has a size limit of 10MB
- Up to 10 old files are retained
- Files are named with date suffix (e.g., `sql-2025-07-03.log`)
