using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static DAL.Models.LocationInfo;

namespace ChunkServiceHandler
{
	interface IChunker
	{
		const int BUFFER_SIZE = 20 * 1024; //20480B, 20.48KB
		const int CHUNK_SIZE = 4 * 1024;

        //string TempChunkDir { get; set; }

		abstract List<ChunkInfo> ScatterChunks(string directory, List<LocationInfo> locations, MetaInfo metaInfo);
        abstract string MergeChunks(List<ChunkInfo> chunks, MetaInfo file);
        abstract string MergeChunks(string directory, MetaInfo file);
        abstract void DeleteChunks(string[] paths, string fileName);

        string SplitFile(string file) //returns temp dir containing chunks
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
        void DeleteTempChunks(string dir)
        {
            Directory.Delete(dir, true);
        }
    }

	internal class ChunkerFactory
	{
		public static IChunker GetChunkerClass(LocationType type)
		{
            if (type == LocationType.local)
                return new LocalChunker();
			else if (type == LocationType.gdrive)
				return new GDriveChunker();
			else
				throw new NotImplementedException(
					$"Location type {type.ToString()} is not supported.");
		}
	}
}
