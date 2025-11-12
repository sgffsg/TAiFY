using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.UnitTests
{
    /// <summary>
    ///  Представляет временный файл, создаваемый на время работы теста.
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        private TempFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static TempFile Create(string contents)
        {
            string path = System.IO.Path.GetTempFileName();
            File.WriteAllText(path, contents, Encoding.UTF8);
            return new TempFile(path);
        }

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}
