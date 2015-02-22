using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace AutoDoc
{
	public class Parser
	{
		public Parser ()
		{
		}
		public string[] GetAllPdfsInDirectory(string directoryPath){
			var files = Directory.GetFiles (directoryPath, "*.pdf", SearchOption.AllDirectories);
			return files;
		}

		public IList<KeywordViewModel> ParsePdf(string filePath, IList<KeywordViewModel> allKeywords)
		{
			var reader = new PdfReader(filePath);
			var foundKeywords = new List<KeywordViewModel> ();
			for (int page = 1; page <= reader.NumberOfPages; page++)
			{
				var strategy = new SimpleTextExtractionStrategy();
				var currentText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

				currentText = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
				foreach (var keyword in allKeywords) 
				{
					foreach (var text in keyword.TextToMatch) {
						if (currentText.ToLowerInvariant().Contains (text.ToLowerInvariant())) {
							var existingKeyword = foundKeywords.FirstOrDefault (x => x.Name == keyword.Name && x.Category == keyword.Category);
							if (existingKeyword != null) {
								existingKeyword.Count++;
							} else {
								existingKeyword = new KeywordViewModel ();
								existingKeyword.Count = 1;
								existingKeyword.Name = keyword.Name;
								existingKeyword.Category = keyword.Category;
								existingKeyword.Id = keyword.Id;
								existingKeyword.Parent = keyword.Parent;
								existingKeyword.Tags = keyword.Tags;
								existingKeyword.TextToMatch = keyword.TextToMatch;
								foundKeywords.Add (existingKeyword);
							}
						}
					}
				}
			}

			reader.Close();
			return foundKeywords;
		}

		public void AddMetaDataToPdf(string filePath, IList<TagViewModel> tags, string title)
		{
			string inputFile = filePath;
			//string outputFile = Path.Combine(filePath, "Output.pdf");
			var tempOutput = Path.GetTempFileName ();

			PdfReader reader = new PdfReader(inputFile);
			using(FileStream fs = new FileStream(tempOutput, FileMode.Create, FileAccess.Write, FileShare.None)){
				using (PdfStamper stamper = new PdfStamper(reader, fs))
				{
					Dictionary<String, String> info = reader.Info;
					info["Title"] = title;
					var keywords = string.Empty;
					foreach (var tag in tags) {
						keywords += tag.Name;
						if (tag != tags.Last ()) 
						{
							keywords += ", ";
						}
					}

					info["Keywords"] = keywords;
					stamper.MoreInfo = info;
					stamper.Close();
				}
			}

			File.Delete (inputFile);
			File.Move (tempOutput, filePath);
		}

	}
}

