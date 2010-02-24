using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFRider
{
    public class PDFFile
    {
        public PDFFile(string path)
        {
            this.FullName = path;
            this.PageRanges = new List<string>();
        }

        public string FullName { get; set; }
        public List<string> PageRanges { get; private set; }
    }
}
