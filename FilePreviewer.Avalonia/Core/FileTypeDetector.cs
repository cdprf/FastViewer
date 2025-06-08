using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePreviewer.Avalonia.Core
{
    public static class FileTypeDetector
    {
        public enum FileType
        {
            Image,
            Text,
            Binary,
            Unknown
        }

        private static readonly string[] ImageExtensions = { ".jpeg", ".jpg", ".gif", ".bmp", ".png" };

        public static async Task<FileType> GetFileTypeAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return FileType.Unknown;
            }

            try
            {
                var extension = Path.GetExtension(filePath)?.ToLowerInvariant();
                if (extension != null && ImageExtensions.Contains(extension))
                {
                    return FileType.Image;
                }

                if (!File.Exists(filePath))
                {
                    return FileType.Unknown;
                }

                if (new FileInfo(filePath).Length == 0)
                {
                    return FileType.Text; // Empty files are considered text for this implementation
                }

                byte[] buffer = new byte[200];
                int bytesRead;

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                }

                if (bytesRead == 0) // Should be covered by FileInfo.Length == 0, but as a safeguard
                {
                    return FileType.Text;
                }

                // Check for text characters (CR or LF)
                // We only need to check the bytes actually read.
                for (int i = 0; i < bytesRead; i++)
                {
                    byte b = buffer[i];
                    if (b == (byte)'\r' || b == (byte)'\n')
                    {
                        return FileType.Text;
                    }
                }

                // If no CR/LF found in the initial bytes, classify as binary
                return FileType.Binary;

            }
            catch (FileNotFoundException)
            {
                // Log? For now, return Unknown
                return FileType.Unknown;
            }
            catch (IOException ex)
            {
                // Log error ex.Message?
                Console.WriteLine($"IO Exception in GetFileTypeAsync: {ex.Message}");
                return FileType.Unknown;
            }
            catch (Exception ex)
            {
                // Log error ex.Message?
                Console.WriteLine($"Generic Exception in GetFileTypeAsync: {ex.Message}");
                return FileType.Unknown;
            }
        }
    }
}
