namespace SpellParser.Core
{
    public interface IImportOptions
    {
        string EQCasterExportFilePath { get; }
        string SpellsUSFilePath { get; }
        int MaxSpellId { get; }
    }
}