
namespace PDFRider
{
    //[System.Windows.Markup.DictionaryKeyProperty("Key")] this doesn't work so use "classic" x:Key property
    [System.Windows.Markup.ContentProperty("Info")]
    public class LocInfo
    {
        public LocInfo()
        { }

        public string Key { get; set; }
        public string TargetProject { get; set; }
        public string TargetProjectVersion { get; set; }
        public string Info { get; set; }
    }
}
