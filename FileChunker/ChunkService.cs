using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static DAL.Models.LocationInfo;

namespace ChunkServiceHandler
{
	public class ChunkService
	{
        const int BUFFER_SIZE = 20 * 1024; //20480B, 20.48KB
        const int CHUNK_SIZE = 4 * 1024;

        public List<ChunkInfo> UploadFile(string file, List<LocationInfo> locations, MetaInfo metaInfo)
        {
            var directory = SplitFile(file);

            //todo - inefficient - is there a better way? 
            var files = Directory.GetFiles(directory)
               .OrderBy(n => Convert.ToInt32(Path.GetFileNameWithoutExtension(n))).ToList();
            int fileCount = files.Count;
            int locationCount = locations.Count;

            if (fileCount <= 0 || locationCount <= 0)
                return null;

            var chunks = new List<ChunkInfo>();

            //Start off with first location type
            var locationChunker = ChunkerFactory.GetChunkerClass(locations[0].Type);
            if (!locationChunker.Initialize(locations[0], metaInfo))
                throw new Exception("Could not create location chunker instance");

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

                    //Once the bucket is full, move to next location to add chunks
                    locationChunker = ChunkerFactory.GetChunkerClass( 
                        locations[++locationIdx].Type);
                    if (!locationChunker.Initialize(locations[locationIdx], metaInfo))
                        throw new Exception("Could not create location chunker instance");
                }

                locationChunker.ScatterChunk(files[i]);

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

        public string DownloadFile(List<ChunkInfo> chunks, MetaInfo metaInfo)
        {
            LocationType currentLocType = LocationType.local; //default
            IChunker locationChunker = null;

            int percentage = 0;
            string fileName = $"{metaInfo.Name}.{metaInfo.Type}";
            string destFile = Path.Combine(Path.GetTempPath(), fileName);
            using var outputStream = File.Create(destFile);

            for (int i = 0, fileCount = chunks.Count; i < fileCount; i++)
            {
                ChunkInfo chunk = chunks[i];
                /*
                 * Either chunker has not been initalized, or the current chunk is at a different location
                 * so need to reset chunker.
                 * Note: chunk locations are created in batches, so Initialize should only be called for 
                 * X locations.
                 */
                if (locationChunker == null || currentLocType != chunk.LocationInfo.Type)
                {
                    locationChunker = ChunkerFactory.GetChunkerClass(chunk.LocationInfo.Type);
                    if (!locationChunker.Initialize(chunk.LocationInfo, metaInfo))
                        throw new Exception("Could not create location chunker instance");
                }

                //call chunker GetFile or something
                //handle the merging of files here

                using var inputStream = File.OpenRead(locationChunker.GetChunk(chunk));
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

        public bool DeleteFile(List<LocationInfo> locations, MetaInfo metaInfo)
        {
            throw new NotImplementedException();
        }

        private static string SplitFile(string file) //returns temp dir containing chunks
        {
            //where the file chunks will be stored temporarily
            var outputDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(file));
            Directory.CreateDirectory(outputDir);

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

            Console.WriteLine($"SplitFile: 100%");
            return outputDir;
        }
    }
}
