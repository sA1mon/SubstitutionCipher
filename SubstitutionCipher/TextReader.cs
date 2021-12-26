using System.IO;

namespace SubstitutionCipher
{
    public static class TextReader
    {
        public static string ReadWithoutSpecials(string fromPath)
        {
            var text = File.ReadAllText(fromPath).ToLower();

            return text;
        }
    }
}