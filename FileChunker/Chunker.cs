using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileChunker
{
	public static class Chunker
	{
        private const int BUFFER_SIZE = 20 * 1024; //20480B, 20.48KB
        private const int CHUNK_SIZE = 4 * 1024;

        public static void SplitFile(string file, string outputDir)
        {
            var buffer = new byte[BUFFER_SIZE];

            using var inputStream = File.OpenRead(file);
            int idx = 0, percentage = 0;

            while (inputStream.Position < inputStream.Length)
            {
                int remaining = CHUNK_SIZE;
                int bytesRead = 0;
                using var outputStream = File.Create(Path.Combine(outputDir, idx.ToString()));

                while (remaining > 0 && (bytesRead =
                    inputStream.Read(buffer, 0, Math.Min(remaining, BUFFER_SIZE))) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                    remaining -= bytesRead;
                }

                int currentPercentage = (int)((inputStream.Position * 100) / inputStream.Length);
                if (currentPercentage > percentage)
                {
                    Console.WriteLine(
                        $"{(inputStream.Position * 100) / inputStream.Length}%");
                    percentage = currentPercentage;
                }

                idx++;
            }
        }

        public static void MergeChunks(string directory)
        {
            //Need to fetch files in order - this seems ineffieicnet so find a better solution
            var inputFiles = Directory.GetFiles(directory)
                .OrderBy(n => Convert.ToInt32(Path.GetFileNameWithoutExtension(n)));

            using var outputStream = File.Create(Path.Combine(directory, "output.pdf"));
            foreach (var inputFilePath in inputFiles)
            {
                using var inputStream = File.OpenRead(inputFilePath);
                // Buffer size can be passed as the second argument.
                inputStream.CopyTo(outputStream);

                Console.WriteLine("The file {0} has been processed.", inputFilePath);
            }

            /*const int BUFFER_SIZE = 20 * 1024; //20480B, 20.48KB

            var inputFiles = Directory.GetFiles(directory);
            using var output = File.Create(Path.Combine(directory, "output.pdf"));
            foreach (var file in inputFiles)
            {
                using var input = File.OpenRead(file);
                var buffer = new byte[BUFFER_SIZE];
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
            }*/
        }
    }
}
