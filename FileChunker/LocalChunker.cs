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
        private string m_destination;

        public bool Initialize(LocationInfo location, MetaInfo metaInfo)
        {
            //Add chunks to location+fileName directory
            m_destination = Path.Combine(location.Path, metaInfo.Name);

            if (Directory.Exists(m_destination))
                Directory.Delete(m_destination, true);

            //Need to create directory once for each location
            Directory.CreateDirectory(m_destination);

            return true;
        }


        public void ScatterChunk(string file)
		{
            File.Move(file, Path.Combine(m_destination, Path.GetFileName(file)));
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
