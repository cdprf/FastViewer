using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilePreviewer.Avalonia.Core;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FilePreviewer.Tests
{
    [TestClass]
    public class FileTypeDetectorTests
    {
        private const string TestFileNameImageJpg = "test.jpg";
        private const string TestFileNameImagePng = "test.png";
        private const string TestFileNameBinary = "test.bin";
        private const string TestFileNameText = "test.txt";
        private const string TestFileNameEmpty = "empty.txt";
        private const string TestFileNonExistent = "nonexistent.dat";

        [TestInitialize]
        public void Setup()
        {
            // Clean up any existing test files before each test
            CleanUpTestFiles();

            // Create dummy files for binary and text tests
            // Binary file (first 200 bytes without CR/LF)
            byte[] binaryData = new byte[250];
            for (int i = 0; i < binaryData.Length; i++)
            {
                binaryData[i] = (byte)(i % 250 + 1); // Ensure no 0, CR, or LF for binary part
                if (binaryData[i] == (byte)'\r' || binaryData[i] == (byte)'\n')
                {
                    binaryData[i] = (byte)'_'; // Replace CR/LF if accidentally generated
                }
            }
            File.WriteAllBytes(TestFileNameBinary, binaryData);

            // Text file (with CR/LF)
            File.WriteAllText(TestFileNameText, "This is a test file.\r\nIt has newlines.");

            // Empty file
            File.WriteAllBytes(TestFileNameEmpty, new byte[0]);

            // Create dummy image files (just by extension, content doesn't matter for this part of detector)
            File.WriteAllText(TestFileNameImageJpg, "dummy image content");
            File.WriteAllText(TestFileNameImagePng, "dummy image content");
        }

        [TestCleanup]
        public void CleanUpTestFiles()
        {
            DeleteTestFile(TestFileNameImageJpg);
            DeleteTestFile(TestFileNameImagePng);
            DeleteTestFile(TestFileNameBinary);
            DeleteTestFile(TestFileNameText);
            DeleteTestFile(TestFileNameEmpty);
            // No need to delete TestFileNonExistent as it shouldn't be created
        }

        private void DeleteTestFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public async Task GetFileTypeAsync_RecognizesJpgImageByExtension()
        {
            var fileType = await FileTypeDetector.GetFileTypeAsync(TestFileNameImageJpg);
            Assert.AreEqual(FileTypeDetector.FileType.Image, fileType);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_RecognizesPngImageByExtension()
        {
            var fileType = await FileTypeDetector.GetFileTypeAsync(TestFileNameImagePng);
            Assert.AreEqual(FileTypeDetector.FileType.Image, fileType);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_RecognizesOtherImageExtensions()
        {
            File.WriteAllText("test.gif", "dummy");
            var fileTypeGif = await FileTypeDetector.GetFileTypeAsync("test.gif");
            Assert.AreEqual(FileTypeDetector.FileType.Image, fileTypeGif);
            DeleteTestFile("test.gif");

            File.WriteAllText("test.bmp", "dummy");
            var fileTypeBmp = await FileTypeDetector.GetFileTypeAsync("test.bmp");
            Assert.AreEqual(FileTypeDetector.FileType.Image, fileTypeBmp);
            DeleteTestFile("test.bmp");

            File.WriteAllText("test.jpeg", "dummy");
            var fileTypeJpeg = await FileTypeDetector.GetFileTypeAsync("test.jpeg");
            Assert.AreEqual(FileTypeDetector.FileType.Image, fileTypeJpeg);
            DeleteTestFile("test.jpeg");
        }

        [TestMethod]
        public async Task GetFileTypeAsync_RecognizesBinaryFile()
        {
            var fileType = await FileTypeDetector.GetFileTypeAsync(TestFileNameBinary);
            Assert.AreEqual(FileTypeDetector.FileType.Binary, fileType);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_RecognizesTextFile()
        {
            var fileType = await FileTypeDetector.GetFileTypeAsync(TestFileNameText);
            Assert.AreEqual(FileTypeDetector.FileType.Text, fileType);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_HandlesNonExistentFile()
        {
            var fileType = await FileTypeDetector.GetFileTypeAsync(TestFileNonExistent);
            Assert.AreEqual(FileTypeDetector.FileType.Unknown, fileType);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_HandlesEmptyFileAsText()
        {
            var fileType = await FileTypeDetector.GetFileTypeAsync(TestFileNameEmpty);
            Assert.AreEqual(FileTypeDetector.FileType.Text, fileType);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_HandlesNullOrEmptyPath()
        {
            var fileTypeNull = await FileTypeDetector.GetFileTypeAsync(null);
            Assert.AreEqual(FileTypeDetector.FileType.Unknown, fileTypeNull);

            var fileTypeEmpty = await FileTypeDetector.GetFileTypeAsync("");
            Assert.AreEqual(FileTypeDetector.FileType.Unknown, fileTypeEmpty);

            var fileTypeWhitespace = await FileTypeDetector.GetFileTypeAsync("   ");
            Assert.AreEqual(FileTypeDetector.FileType.Unknown, fileTypeWhitespace);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_FileWithOnlyCRIsText()
        {
            string fileName = "cr_only.txt";
            File.WriteAllBytes(fileName, new byte[] { (byte)'a', (byte)'\r', (byte)'b' });
            var fileType = await FileTypeDetector.GetFileTypeAsync(fileName);
            Assert.AreEqual(FileTypeDetector.FileType.Text, fileType);
            DeleteTestFile(fileName);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_FileWithOnlyLFIsText()
        {
            string fileName = "lf_only.txt";
            File.WriteAllBytes(fileName, new byte[] { (byte)'a', (byte)'\n', (byte)'b' });
            var fileType = await FileTypeDetector.GetFileTypeAsync(fileName);
            Assert.AreEqual(FileTypeDetector.FileType.Text, fileType);
            DeleteTestFile(fileName);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_SmallBinaryFile() // Smaller than buffer
        {
            string fileName = "small.bin";
            byte[] binaryData = new byte[50];
            for (int i = 0; i < binaryData.Length; i++)
            {
                byte val = (byte)(i + 1);
                if (val == (byte)'\r' || val == (byte)'\n')
                {
                    val = (byte)'_'; // Ensure no CR/LF
                }
                binaryData[i] = val;
            }
            File.WriteAllBytes(fileName, binaryData);
            var fileType = await FileTypeDetector.GetFileTypeAsync(fileName);
            Assert.AreEqual(FileTypeDetector.FileType.Binary, fileType);
            DeleteTestFile(fileName);
        }

        [TestMethod]
        public async Task GetFileTypeAsync_SmallTextFile() // Smaller than buffer
        {
            string fileName = "small.txt";
            File.WriteAllText(fileName, "Hi\n");
            var fileType = await FileTypeDetector.GetFileTypeAsync(fileName);
            Assert.AreEqual(FileTypeDetector.FileType.Text, fileType);
            DeleteTestFile(fileName);
        }
    }
}
