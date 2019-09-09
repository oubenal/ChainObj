using System;

namespace ChainObj.UnitTests
{
    class TempDirectory : IDisposable
    {
        internal string Path { get; }
        internal TempDirectory()
        {
            Path = Utilities.TempDirPath;
            if (System.IO.Directory.Exists(Path))
                System.IO.Directory.Delete(Path, true);
            System.IO.Directory.CreateDirectory(Path);
        }
        public void Dispose()
        {
            System.IO.Directory.Delete(Path, true);
        }
    }
}
