using SpellParser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpellParser.Infrastructure.Reporters
{
    public class SQLReporter
    {
        public static void WriteToDisk(IExportOptions options, IEnumerable<ChangeTracker> updates)
        {
            var filePath = options.ExportLocation;
            EnsureDirectoryExists(filePath);

            var sql = string.Join("\n\n", updates.Select(SQL));
            File.WriteAllText($@"{filePath}\SqlUpdates.sql", sql);

            var undoAql = string.Join("\n\n", updates.Select(UndoSQL));
            File.WriteAllText($@"{filePath}\SqlRollback.sql", undoAql);
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
{string.Join("\n,", changeTracker.Changes.Select(x => $"{x.Name} = {GetSqlValue(x.Name, x.NewValue)}"))}
WHERE id = {changeTracker.Id};";
              return sql;
          };

        public static Func<ChangeTracker, string> UndoSQL = (ChangeTracker changeTracker) =>
          {
              var sql = $@"-- {changeTracker.Name}
UPDATE spells_new SET
{string.Join("\n,", changeTracker.Changes.Select(x => $"{x.Name} = {GetSqlValue(x.Name, x.OldValue)}"))}
WHERE id = {changeTracker.Id};";
              return sql;
          };

        private static string GetSqlValue(string columnName, string value)
        {
            if (columnName == nameof(PEQSpell.name))
            {
                return $"'{value}'";
            }

            return value;
        }
    }
}