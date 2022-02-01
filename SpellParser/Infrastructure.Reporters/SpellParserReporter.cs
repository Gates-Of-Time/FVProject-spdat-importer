using SpellParser.Core;
using SpellParser.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellParser.Infrastructure.Reporters
{
    public class SpellParserReporter : IDisposable
    {
        private bool disposedValue;
        private readonly StringBuilder stringBuilder;
        private readonly IExportOptions options;

        public SpellParserReporter(IExportOptions options)
        {
            stringBuilder = new StringBuilder();
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Write(string reportText)
        {
            stringBuilder.Append(reportText);
        }

        public void Write(IEnumerable<ChangeTracker> updates)
        {
            stringBuilder.AppendLine(@"## Manual Spell Updates");
            stringBuilder.AppendLine($@"Number of spells: {updates.Count()}");
            stringBuilder.AppendLine($@"");
            var reportText = string.Join("\n\n", updates.Select(x => ChangeTrackerReporter.ToMarkdown(x)));
            stringBuilder.Append(reportText);
        }

        private static void EnsureDirectoryExists(string configFilePath)
        {
            var path = System.IO.Path.GetFullPath(configFilePath);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var filePath = options.ExportLocation;
                    EnsureDirectoryExists(filePath);
                    File.WriteAllText($@"{filePath}\ParseReport.md", stringBuilder.ToString());
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
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
