using DAL.Models;
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
                    percentage = currentPercentage;
                    Console.WriteLine($"SplitFile: {percentage}%");
                }

                idx++;
            }
        }

        public static List<ChunkInfo> ScatterChunks(
            string directory, List<LocationInfo> locations, MetaInfo metaInfo)
        {
            var files = Directory.GetFiles(directory)
                .OrderBy(n => Convert.ToInt32(Path.GetFileNameWithoutExtension(n))).ToList();
            int fileCount = files.Count;
            int locationCount = locations.Count;

            if (fileCount <= 0 || locationCount <= 0)
                return null;

            var chunks = new List<ChunkInfo>();

            int locationIdx = 0, curBucketCount = 0, percentage = 0;
            //Number of total buckets to create
            int bucketSize = fileCount / locationCount;

            for(int i = 0; i < fileCount; i++)
            {
                /*
                 * 1. Iterate through files
                 * 2. Add chunks to current location
                 * 3. Once a location gets all its allocated chunks (total chunks / total locations), 
                 *    need to reset the chunk counter and increment destination location
                 * 4. Repeat
                 */

                //Second condtion - If chunks aren't evenly divisble by locations, get remainder and add to last grouping
                if (curBucketCount == bucketSize && locationIdx != locationCount - 1)
                {
                        curBucketCount = 0;
                        locationIdx++; //Once the bucket is full, move to next location to add chunks
                }

                //Add chunks to location+fileName directory
                var destination = Path.Combine(locations[locationIdx].Path, metaInfo.Name);
                if (curBucketCount == 0)
                {
                    if (Directory.Exists(destination))
                        Directory.Delete(destination, true);

                    Directory.CreateDirectory(destination); //Need to create directory once for each location
                }

                File.Move(files[i], Path.Combine(destination, Path.GetFileName(files[i])));

                chunks.Add(new ChunkInfo
                {
                    MetaInfo = metaInfo,
                    LocationInfo = locations[locationIdx],
                    Name = Path.GetFileNameWithoutExtension(files[i])
                });

                curBucketCount++;

                int currentPercentage = ((i * 100) / fileCount);
                if (currentPercentage > percentage)
                {
                    percentage = currentPercentage;
                    Console.WriteLine($"ScatterChunks: {percentage}%");
                }
            }

            return chunks;
        }

        public static string MergeChunks(List<ChunkInfo> chunks, string fileName)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var tmpFolder = Path.Combine(Path.GetTempPath(), fileNameWithoutExt);

            if (Directory.Exists(tmpFolder))
                Directory.Delete(tmpFolder, true);

            Directory.CreateDirectory(tmpFolder);

            foreach (var chunk in chunks)
                File.Copy(Path.Combine(chunk.LocationInfo.Path, fileNameWithoutExt, chunk.Name),
                    Path.Combine(tmpFolder, chunk.Name));

            return MergeChunks(tmpFolder, fileName);
        }

        public static string MergeChunks(string directory, string fileName)
        {
            //Need to fetch files in order - this seems ineffieicnet so find a better solution
            var inputFiles = Directory.GetFiles(directory)
                .OrderBy(n => Convert.ToInt32(Path.GetFileNameWithoutExtension(n))).ToList();

            int percentage = 0;
            string destFile = Path.Combine(directory, fileName);
            using var outputStream = File.Create(Path.Combine(directory, fileName));
            for(int i = 0, fileCount = inputFiles.Count; i < fileCount; i++)
            {
                using var inputStream = File.OpenRead(inputFiles[i]);
                // Buffer size can be passed as the second argument.
                inputStream.CopyTo(outputStream);

                int currentPercentage = ((i * 100) / fileCount);
                if (currentPercentage > percentage)
                {
                    percentage = currentPercentage;
                    Console.WriteLine($"MergeChunks: {percentage}%");
                }

                //Console.WriteLine("The file {0} has been processed.", inputFilePath);
            }

            return destFile;
        }
    }
}
