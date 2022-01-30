using SpellParser.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpellParser.Infrastructure.Data
{
    public class OutputWriter
    {
        private const string FilePath = @"output\";

        public static void WriteToDisk(IEnumerable<SpellUpdater> updates)
        {
            EnsureDirectoryExists(FilePath);

            var changes = string.Join("\n", updates.Select(x => x.ChangeTracker.ToString()));
            var sql = string.Join("\n\n", updates.Select(x => x.ChangeTracker.SQL));
            var undoAql = string.Join("\n\n", updates.Select(x => x.ChangeTracker.UndoSQL));

            File.WriteAllText($"{FilePath}SqlUpdates.sql", sql);
            File.WriteAllText($"{FilePath}SqlRollback.sql", undoAql);
            File.WriteAllText($"{FilePath}ChangeLog.txt", changes);
        }

        private static void EnsureDirectoryExists(string configFilePath)
        {
            var path = System.IO.Path.GetFullPath(configFilePath);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}