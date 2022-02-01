using SpellParser.Core;

namespace SpellParser
{
    public enum Expansion
    {
        Original,
        Kunark,
        Velious
    }

    public class Settings
    {
        public Expansion Expansion { get; set; }
        public ImportOptions Import { get; set; }
        public ExportOptions Export { get; set; }

        public class ImportOptions : IImportOptions
        {
            public string EQCasterExportFilePath { get; set; }
            public string SpellsUSFilePath { get; set; }
            public int MaxSpellId { get; set; }
        }

        public class ExportOptions : IExportOptions
        {
            public string ExportLocation { get; set; }
        }
    }
}