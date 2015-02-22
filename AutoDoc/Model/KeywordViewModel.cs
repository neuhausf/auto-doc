using System;
using System.Collections.Generic;

namespace AutoDoc
{
	public class KeywordViewModel
	{
		public KeywordViewModel ()
		{
		}

		public int Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public IList<string> TextToMatch {
			get;
			set;
		}
		public CategoryEnum Category {
			get;
			set;
		}
		public int Count {
			get;
			set;
		}

		public HashSet<TagViewModel> Tags {
			get;
			set;
		}

		public KeywordViewModel Parent {
			get;
			set;
		}
	}
}

