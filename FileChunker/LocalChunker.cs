using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChunkServiceHandler
{
	public class LocalChunker : IChunker
	{
        public List<ChunkInfo> ScatterChunks(string directory, List<LocationInfo> locations, MetaInfo metaInfo)
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

            for (int i = 0; i < fileCount; i++)
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

                    DirectoryInfo x = Directory.CreateDirectory(destination); //Need to create directory once for each location
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

            Console.WriteLine($"ScatterChunks: 100%");
            return chunks;
        }
        public string MergeChunks(List<ChunkInfo> chunks, MetaInfo file)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.Name);
            var tmpFolder = Path.Combine(Path.GetTempPath(), fileNameWithoutExt);

            if (Directory.Exists(tmpFolder))
                Directory.Delete(tmpFolder, true);

            Directory.CreateDirectory(tmpFolder);

            //todo - why do i do this?
            int percentage = 0;
            for (int i = 0, count = chunks.Count; i < count; i++)
            {
                ChunkInfo chunk = chunks[i];
                File.Copy(
                    Path.Combine(chunk.LocationInfo.Path, fileNameWithoutExt, chunk.Name),
                    Path.Combine(tmpFolder, chunk.Name));


                int currentPercentage = ((i * 100) / count);
                if (currentPercentage > percentage)
                {
                    percentage = currentPercentage;
                    Console.WriteLine($"MergeChunks preparing files: {percentage}%");
                }
            }

            return MergeChunks(tmpFolder, file);
        }
        public string MergeChunks(string directory, MetaInfo file)
        {
            // Need to fetch files in order - this seems inefficient so find a better solution
             var inputFiles = Directory.GetFiles(directory)
                 .OrderBy(n => Convert.ToInt32(Path.GetFileNameWithoutExtension(n))).ToList();

            int percentage = 0;
            string fileName = $"{file.Name}.{file.Type}";
            string destFile = Path.Combine(directory, fileName);
            using var outputStream = File.Create(Path.Combine(directory, fileName));
            for (int i = 0, fileCount = inputFiles.Count; i < fileCount; i++)
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

            Console.WriteLine($"MergeChunks: 100%");
            return destFile;
        }

        public void DeleteChunks(string[] paths, string fileName)
        {
            foreach (var path in paths)
                Directory.Delete(Path.Combine(path, fileName), true);
        }
	}
}
