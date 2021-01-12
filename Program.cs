using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UTF8SecureKeyGenerator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("-= Cryptographically Secure UTF-8 Key Generator");
            Console.WriteLine("Built for the sake of JWT signing keys");

            using var secureRng = new RNGCryptoServiceProvider();

            var keyByteSize = -1;

            var encoding = Encoding.Default;

            var fileName = "secretKey.txt";
            var fileNameChosen = false;

            var invalidFileNameChars = Path.GetInvalidPathChars();
            string fullFilePath;

            do
            {
                Console.WriteLine("Enter output filename (leave blank for \"secretKey.txt\" > ");
                var inputFileName = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(inputFileName))
                {
                    fileNameChosen = true;
                }
                else if (invalidFileNameChars.Any(x => inputFileName.Contains(x)))
                {
                    Console.WriteLine("Input a valid file name.");
                }
                else
                {
                    fileName = inputFileName;
                    fileNameChosen = true;
                }

                // Finally check if file exists
                fullFilePath = Path.Join(Environment.CurrentDirectory, fileName);
                if (File.Exists(fullFilePath))
                {
                    Console.WriteLine("That file already exists, choose a new file name.");
                    fileNameChosen = false;
                }
            } while (!fileNameChosen);

            do
            {
                Console.Write("Enter number of bytes > ");

                int.TryParse(Console.ReadLine(), out keyByteSize);
            } while (keyByteSize <= 0);

            Console.WriteLine($"Generating a {keyByteSize * 8} bit UTF-8 encoded security key");

            var keyBytes = new byte[keyByteSize];

            secureRng.GetBytes(keyBytes);

            var base64 = Convert.ToBase64String(keyBytes);
            var base64UrlKey = base64.TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            Console.WriteLine("-= Secure Key - Do not share this with anyone =-");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(base64UrlKey);
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine($"Saving to \"{fullFilePath}\"");

            File.WriteAllText(fullFilePath, base64UrlKey, encoding);
        }
    }
}