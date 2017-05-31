using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherControl
{

    class OnFileChanged
    {
        public EventHandler<FileEventArgs> Chandged;
        private DateTime currentLastWriteTime;
        private string path;
        private object syncObject;
        public OnFileChanged(string path, object syncObject)
        {
            this.path = path;
            this.syncObject = syncObject;
            currentLastWriteTime = File.GetLastWriteTime(path);
            Thread dataThread = new Thread(RunFollowing);
            dataThread.Start();
        }

        private void RunFollowing()
        {
            while (true)
            {
                if (currentLastWriteTime != File.GetLastWriteTime(path) && checkContentFile())
                {
                    Chandged?.Invoke(this, new FileEventArgs("File has been changed"));
                    currentLastWriteTime = File.GetLastWriteTime(path);
                }
            }
        }

        private bool checkContentFile()
        {
            lock (syncObject)
            {
                string line = "";
                using (StreamReader sr = new StreamReader(path))
                    line = sr.ReadLine();
                if (line == "1")
                    return true;
                else
                    return false;
            }
        }
    }
    class Program
    {
        private static FileInfo file;
        private static OnFileChanged fileFollower;
        private static object syncObject = new object();

        static void Main(string[] args)
        {
            file = new FileInfo("text.txt");
            fileFollower = new OnFileChanged("text.txt", syncObject);
            fileFollower.Chandged += FileWasChanged;
            while (true)
            {
                Console.WriteLine("Do you want to write a 1 again? (y/n)");
                if (Console.ReadLine() == "y")
                {
                    lock (syncObject)
                    {
                        using (StreamWriter sw = new StreamWriter("text.txt"))
                            sw.WriteLine("1");
                    }
                }
            }
        }

        private static void FileWasChanged(object sender, FileEventArgs e)
        {
            Console.WriteLine(e.Message);
            ThreadPool.QueueUserWorkItem(o => SomeBigMethod());
        }
        private static void SomeBigMethod()
        {
            lock (syncObject)
            {
                using (StreamWriter sw = new StreamWriter("text.txt"))
                    sw.WriteLine("0");
                Thread.Sleep(10000);
                Console.WriteLine("File content changed to 0 ten seconds ago");
            }
        }

    }
}
