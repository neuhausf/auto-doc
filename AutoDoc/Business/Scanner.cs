using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoDoc
{
	public class Scanner
	{
		private Parser parser;
		private Renamer renamer;

		public Scanner ()
		{
			this.parser = new Parser ();
			this.renamer = new Renamer ();
		}

		public void DoScan(string inputPath, string outputPath, string pattern, IList<KeywordViewModel> allKeywords, bool deleteInputFiles){
			var files = this.parser.GetAllPdfsInDirectory(inputPath);
			if (!Directory.Exists (outputPath)) {
				Directory.CreateDirectory (outputPath);
			}
			foreach (var file in files) {
				var foundKeywords = this.parser.ParsePdf (file, allKeywords);
				var newFileName = this.renamer.GetFileName (file, foundKeywords, pattern);
				var newFilePath = Path.Combine (outputPath, newFileName);
				if (deleteInputFiles) {
					File.Move (file, newFilePath);
				} else {
					var counter = 1;
					while (File.Exists (newFilePath)) {
						var duplicateFileName = Path.GetFileNameWithoutExtension (newFileName) + "-" + counter++ + Path.GetExtension(newFilePath);
						newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath), duplicateFileName);
					}

					File.Copy (file, newFilePath);
				}

				parser.AddMetaDataToPdf (newFilePath, foundKeywords.SelectMany (x => x.Tags).ToList(), newFileName);
				renamer.GetTags (newFilePath);
				renamer.AddOrUpdateTags (newFilePath, new HashSet<TagViewModel>(foundKeywords.SelectMany (x => x.Tags).ToList()));
			}
		}
	}
}

