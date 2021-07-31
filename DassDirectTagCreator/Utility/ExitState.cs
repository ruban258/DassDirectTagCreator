namespace AGTIAADDIN.Utility
{
    public enum ExitState
    {
        IsChangeable,
        CouldNotExport,
        CouldNotCompile,
        Successful,
        CouldNotImport,
        BlockIsKnowHowProtected,
        ProgrammingLanguageNotSupported,
        XmlEditingError,
        IsLibraryType,
        IsDataBlock
    }
    public class TagRecord
    {
        public string TagName { get; set; }
        public string Address { get; set; }
    }
}
