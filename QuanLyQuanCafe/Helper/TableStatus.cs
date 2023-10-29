using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.Helper
{
    internal class TableStatus
    {
        public TableStatus(bool status, string statusText)
        {
            Status = status;
            StatusText = statusText;
        }
        public bool Status { get; set; }
        public string StatusText
        {
            get;
            set;
        }
    }
}
