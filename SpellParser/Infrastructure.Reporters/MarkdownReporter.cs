using SpellParser.Core;
using System.Text;

namespace SpellParser.Infrastructure.Reporters
{
    public class MarkdownReporter : IDisposable
    {
        private bool disposedValue;
        private readonly StringBuilder stringBuilder;
        private readonly IExportOptions options;

        public MarkdownReporter(IExportOptions options)
        {
            stringBuilder = new StringBuilder();
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Append(string reportText)
        {
            stringBuilder.Append(reportText);
        }

        public void AppendSection<T>(string header, IEnumerable<T> changes)
        {
            stringBuilder.AppendLine($@"## {header}");
            stringBuilder.AppendLine($@"Occurences: {changes.Count()}");
            stringBuilder.AppendLine($@"");
        }

        public void AppendSection<T>(string header, IEnumerable<T> changes, Func<T, string> reporter)
        {
            AppendSection(header, changes);
            foreach (var change in changes)
            {
                stringBuilder.AppendLine($@"{reporter(change)}");
            }
            stringBuilder.AppendLine($@"");
        }

        public void AppendBulletsSection<T>(string header, IEnumerable<T> changes, Func<T, string> reporter)
        {
            AppendSection(header, changes);
            foreach (var change in changes)
            {
                stringBuilder.AppendLine($@"+ {reporter(change)}");
            }
            stringBuilder.AppendLine($@"");
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
                    File.WriteAllText($@"{filePath}\Report.md", stringBuilder.ToString());
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