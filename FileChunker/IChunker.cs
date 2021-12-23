using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static DAL.Models.LocationInfo;

namespace ChunkServiceHandler
{
    //todo
    //need to check capacity of each location before starting chunker
    //implement hangfire so tasks can run in background (when dowloading/uploading large files)

	interface IChunker
	{
        //string TempChunkDir { get; set; }
        abstract bool Initialize(LocationInfo location, MetaInfo metaInfo); //return if initialized properly
        abstract void ScatterChunk(string file);
        abstract string MergeChunks(List<ChunkInfo> chunks, MetaInfo file);
        abstract string MergeChunks(string directory, MetaInfo file);
        abstract void DeleteChunks(string[] paths, string fileName);

        //todo - maybe we can delete when we move them?
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
			/*else if (type == LocationType.gdrive)
				return new GDriveChunker();*/
			else
				throw new NotImplementedException(
					$"Location type {type.ToString()} is not supported.");
		}
	}
}
