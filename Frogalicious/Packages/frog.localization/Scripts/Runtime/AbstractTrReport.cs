#if UNITY_EDITOR
namespace Frog.Localization
{
    /// <summary>
    /// An inheritor of this class is generated in every assembly that refrences the `Frog.Localization` assembly.
    /// The implementation returns info about all calls to `Tr.Msg` and `Tr.Plu` in the assembly.
    /// For details see <see cref="TrReportEntry"/>.
    /// </summary>
    public abstract class AbstractTrReport
    {
        public abstract TrReportEntry[] Entries { get; }
    }

    public struct TrReportEntry
    {
        public string FullPath;
        public int LineNumber;
        public string MsgId;
        public bool IsPlural;

        public TrReportEntry(string fullPath, int lineNumber, string msgId, bool isPlural)
        {
            FullPath = fullPath;
            LineNumber = lineNumber;
            MsgId = msgId;
            IsPlural = isPlural;
        }
    }
}
#endif
