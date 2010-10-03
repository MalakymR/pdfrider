using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFRider
{
    /// <summary>
    /// Stores basic informations about a file (path, number of pages, ...)
    /// </summary>
    public class PDFFileInfo
    {
        public PDFFileInfo()
        {
            this.PageRanges = new List<string>();
        }

        public string FullName { get; set; }
        public short NumberOfPages { get; set; }
        public short PageLabelStart { get; set; }
        public List<string> PageRanges { get; private set; }
    }
}
