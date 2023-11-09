using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Helpers
{
    public class WebCursorParams
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortColum { get; set; }
        public string? SortDirection { get; set; }
    }
}
