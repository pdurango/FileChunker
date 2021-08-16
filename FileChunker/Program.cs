using System;
using System.IO;
using System.Linq;

namespace FileChunker
{
	class Program
	{
		static void Main(string[] args)
		{
            /*Console.WriteLine("Enter file path...");
			string file = Console.ReadLine();
            Console.WriteLine("Enter destination for chunks...");
            string outputDir = Console.ReadLine();
            SplitFile(file, outputDir);*/

            string file = @"C:\Users\silve\Downloads\10840.pdf";
            string outputDir = @"C:\Users\silve\Downloads\dum";
            outputDir = Path.Combine(outputDir, Path.GetFileName(file));
            
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            outputDir = Directory.CreateDirectory(outputDir).FullName;

            //SplitFile(file, outputDir);
            Chunker.SplitFile(file, outputDir);
            Chunker.MergeChunks(outputDir, Path.GetFileName(file));
        }

        
	}
}
