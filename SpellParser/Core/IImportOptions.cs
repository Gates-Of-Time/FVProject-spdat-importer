namespace SpellParser.Core
{
    public interface IImportOptions
    {
        string EQCasterExportFilePath { get; }
        string SpellsUSFilePath { get; }
        IEnumerable<int> ExcludeSpellIds { get; }
        IEnumerable<string> ExcludateAutomaticUpdatesSpellNames { get; }
        IEnumerable<string> ExcludeSpellNames { get; }
    }
}