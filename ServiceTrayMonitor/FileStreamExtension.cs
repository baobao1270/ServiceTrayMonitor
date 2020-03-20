using System.IO;
using System.Text;

namespace Joseph.ServiceTrayMonitor
{
    public static class FileStreamExtension
    {
        public static byte[] ReadAllBytes(this FileStream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ReadAllString(this FileStream fileStream, Encoding encoding)
        {
            return encoding.GetString(fileStream.ReadAllBytes());
        }

        public static string ReadAllString(this FileStream fileStream)
        {
            return fileStream.ReadAllString(Encoding.UTF8);
        }

        public static void WriteAllBytes(this FileStream fileStream, byte[] bytes)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.SetLength(bytes.Length);
            fileStream.Flush();
        }

        public static void WriteAllString(this FileStream fileStream, string content, Encoding encoding)
        {
            fileStream.WriteAllBytes(encoding.GetBytes(content));
        }

        public static void WriteAllString(this FileStream fileStream, string content)
        {
            fileStream.WriteAllString(content, Encoding.UTF8);
        }
    }
}