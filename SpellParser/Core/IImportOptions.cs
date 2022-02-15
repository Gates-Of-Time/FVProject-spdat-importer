namespace SpellParser.Core
{
    public interface IImportOptions
    {
        string EQCasterExportFilePath { get; }
        string SpellsUSFilePath { get; }
        int MaxSpellId { get; }
        IEnumerable<int> ExcludeSpellIds { get; }
        IEnumerable<string> ExcludateAutomaticUpdatesSpellNames { get; }
        IEnumerable<string> ExcludeSpellNames { get; }
    }
}