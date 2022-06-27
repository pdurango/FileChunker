using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChunkServiceHandler
{
    internal class DropboxChunker : IChunker
    {
        public void DeleteChunks(string[] paths, string fileName)
        {
            throw new NotImplementedException();
        }

        public string GetChunk(ChunkInfo chunk)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(LocationInfo location, MetaInfo metaInfo)
        {
            throw new NotImplementedException();
        }

        public void ScatterChunk(string file)
        {
            throw new NotImplementedException();
        }
    }
}
