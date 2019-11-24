using System.Collections.Generic;
using System.IO;
using System.Text;

namespace thegame.Infrastructure.Common
{
    public static class Extensions
    {
        public static IEnumerable<string> ReadAllLines(this Stream stream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
            }

            stream.Dispose();
        }
    }
}