using System;

using Foundation;
using AppKit;
using System.Collections.Generic;

namespace AutoDoc
{
	public partial class MainWindowController : NSWindowController
	{
		private Scanner scanner = new Scanner();
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.ScanButton.Activated += ScanButtonClicked;
		}

		void ScanButtonClicked (object sender, EventArgs e)
		{
			var allKeywords = new List<KeywordViewModel>();
			allKeywords.Add (new KeywordViewModel { 
				Category = CategoryEnum.Sender, 
				Name = "PostFinance" /* name to put in the filename */, 
				TextToMatch = new List<string> { "PostFinance", "Post Finance" } /* one of these words must be present */,
				Tags = new HashSet<TagViewModel> { new TagViewModel { Name = "Finanzen" } } /* apply these to OSX tags AND PDF keywords */
			});

			allKeywords.Add (new KeywordViewModel { 
				Category = CategoryEnum.DocumentType, 
				Name = "Kontoauszug" /* name to put in the filename */, 
				TextToMatch = new List<string> { "Kontoauszug" } /* one of these words must be present */,
				Tags = new HashSet<TagViewModel> { new TagViewModel { Name = "Finanzen" } } /* apply these to OSX tags AND PDF keywords */
			});

			allKeywords.Add (new KeywordViewModel { 
				Category = CategoryEnum.Topic, 
				Name = "Zinsabschluss" /* name to put in the filename */, 
				TextToMatch = new List<string> { "Zinsabschluss" } /* one of these words must be present */,
				Tags = new HashSet<TagViewModel> { new TagViewModel { Name = "Finanzen" }, new TagViewModel { Name = "Steuern" } } /* apply these to OSX tags AND PDF keywords */
			});

			scanner.DoScan(
				"/Users/flo/Dropbox/scan me" /* source path */, 
				"/Users/flo/Dropbox/scan me output" /* dest path */, 
				"{date}-{category:documenttype}-{category:sender}-{category:topic}" /* filename pattern */, 
				allKeywords /* all possible keywords to look for */, 
				false /* delete in source */);
		}

		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
	}
}
