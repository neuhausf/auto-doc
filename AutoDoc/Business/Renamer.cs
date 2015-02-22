using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Foundation;

namespace AutoDoc
{
	public class Renamer
	{
		public Renamer ()
		{
		}

		public void RenameFile(string filePath, IList<KeywordViewModel> keywords, string pattern)
		{

		}

		public HashSet<TagViewModel> GetTags(string filePath){
			var tags = new HashSet<TagViewModel> ();
			var url = NSUrl.CreateFileUrl(new string[]{filePath});

			NSError error;
			var values = url.GetResourceValues (new NSString[]{ new NSString("NSURLTagNamesKey") }, out error);
			foreach (var val in values) {
				var tagArray = val.Value as NSArray;
				if (tagArray == null) {
					continue;
				}
				for(nuint i = 0; i < tagArray.Count; i++) {
					tags.Add (new TagViewModel { Name = tagArray.GetItem<NSString>(i).ToString() });
				}
			}

			return tags;
		}

		public HashSet<TagViewModel> AddOrUpdateTags(string filePath, HashSet<TagViewModel> tags){
			tags.UnionWith(this.GetTags(filePath));
			var tagListStrings = tags.SelectMany (x => x.Name).Distinct ();
			var url = NSUrl.CreateFileUrl(new string[]{filePath});
			var tagList = new NSMutableArray ();
			foreach (var tag in tagListStrings) {
				tagList.Add(new NSString(tag));
			}

			url.SetResource(new NSString("NSURLTagNamesKey"), tagList);
			return tags;
		}

		public string GetFileName(string filePath, IList<KeywordViewModel> keywords, string pattern){
			// {date}-{category:documenttype}-{category:sender}-{category:topic}
			var date = File.GetCreationTime (filePath);
			var fileName = pattern.Replace("{date}", date.ToString ("yyyy-MM-dd"));
			var fileExt = Path.GetExtension (filePath);
			var firstKeywordsOfCategory = keywords.OrderBy (x => x.Category)
											 .ThenByDescending (x => x.Count)
											 .GroupBy (x => x.Category)
				.Select(x => x.First()).ToList();

			foreach (CategoryEnum cat in Enum.GetValues(typeof(CategoryEnum)))
			{
				var catKeyword = firstKeywordsOfCategory.FirstOrDefault (x => x.Category == cat);
				if (catKeyword != null) {
					fileName = fileName.Replace ("{category:" + cat.ToString ().ToLowerInvariant () + "}", catKeyword.Name);
				} else {
					fileName = fileName.Replace ("{category:" + cat.ToString ().ToLowerInvariant () + "}", string.Empty);
				}
			}

			return fileName + fileExt;
		}
	}
}

