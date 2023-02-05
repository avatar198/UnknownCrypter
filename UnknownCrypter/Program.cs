using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AESCrypter
{
    abstract class PolymorphicAES
    {
        public abstract byte[] EncryptBytes(byte[] input);
    }

    class AesEncryptor : PolymorphicAES
    {
        private Aes aes;

        public AesEncryptor(byte[] key)
        {
            aes = Aes.Create();
            aes.Key = key;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
        }

        public override byte[] EncryptBytes(byte[] input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(input, 0, input.Length);
                    cryptoStream.FlushFinalBlock();

                    return memoryStream.ToArray();
                }
            }
        }

        private void UnusedMethod()
        {
            Console.WriteLine("This is unused code");
        }

        private int AnotherUnusedMethod(int a, int b, int c)
        {
            int result = a + b + c;
            Console.WriteLine("This is another unused code");
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter input file path: ");
            string inputFile = Console.ReadLine();
            Console.Write("Enter output file path: ");
            string outputFile = Console.ReadLine();
            if (!outputFile.EndsWith(".encrypted"))
            {
                outputFile += ".encrypted";
            }
            Console.Write("Enter number of iterations (1-1000): ");
            int iterations = int.Parse(Console.ReadLine());
            Console.Write("Enter secret key (32 characters): ");
            string secretKey = Console.ReadLine();

            if (iterations < 1 || iterations > 1000)
            {
                Console.WriteLine("Error: Invalid number of iterations");
                return;
            }
            if (secretKey.Length != 32)
            {
                Console.WriteLine("Error: Secret key must be 32 characters long");
                return;
            }

            try
            {
                byte[] key = System.Text.Encoding.UTF8.GetBytes(secretKey);
                byte[] input = File.ReadAllBytes(inputFile);

                PolymorphicAES aesEncryptor = new AesEncryptor(key);

                byte[] output = input;
                for (int i = 0; i < iterations; i++)
                {
                    output = aesEncryptor.EncryptBytes(output);
                }
                File.WriteAllBytes(outputFile, output);

                Console.WriteLine("File successfully encrypted");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
