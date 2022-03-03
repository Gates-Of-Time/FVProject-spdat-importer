using SpellParser.Core;
using System.Text;

namespace SpellParser.Infrastructure.Reporters
{
    public class SQLReporter: IDisposable
    {
        public SQLReporter(IExportOptions options)
        {
            Options = options;
        }

        private bool DisposedValue { get; set; }
        private StringBuilder Updates { get; } = new StringBuilder();
        private StringBuilder Rollbacks { get; } = new StringBuilder();
        
        public IExportOptions Options { get; }

        public void Write(IEnumerable<ChangeTracker> updates)
        {
            Updates.AppendJoin("\n\n", updates.Select(SQL));
            Rollbacks.AppendJoin("\n\n", updates.Select(UndoSQL));
        }

        private static void EnsureDirectoryExists(string configFilePath)
        {
            var path = System.IO.Path.GetFullPath(configFilePath);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        private static Func<ChangeTracker, string> SQL = (ChangeTracker changeTracker) =>
            {
                var sql = $@"-- {changeTracker.Name}
UPDATE spells_new SET
{string.Join("\n,", changeTracker.Changes.Select(x => $"`{x.Name}` = {GetSqlValue(x.Name, x.NewValue)}"))}
WHERE id = {changeTracker.Id};";
                return sql;
            };

        public static Func<ChangeTracker, string> UndoSQL = (ChangeTracker changeTracker) =>
            {
                var sql = $@"-- {changeTracker.Name}
UPDATE spells_new SET
{string.Join("\n,", changeTracker.Changes.Select(x => $"`{x.Name}` = {GetSqlValue(x.Name, x.OldValue)}"))}
WHERE id = {changeTracker.Id};";
                return sql;
            };

        private static string GetSqlValue(string columnName, string value)
        {
            if (columnName == nameof(PEQSpell.name))
            {
                return $"'{value.Replace("'", "''")}'";
            }

            return value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    var filePath = Options.ExportLocation;
                    EnsureDirectoryExists(filePath);
                    File.WriteAllText($@"{filePath}\SqlUpdates.sql", $"{Updates}");
                    File.WriteAllText($@"{filePath}\SqlRollback.sql", $"{Rollbacks}");
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                DisposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}