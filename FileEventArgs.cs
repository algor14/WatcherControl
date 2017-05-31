using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherControl
{
    class FileEventArgs:EventArgs
    {
        public string Message { get; set; }
        public FileEventArgs(string message)
        {
            Message = message;
        }
    }
}
