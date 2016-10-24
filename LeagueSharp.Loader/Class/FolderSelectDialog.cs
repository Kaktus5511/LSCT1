using System;
using System.Windows.Forms;

namespace LeagueSharp.Loader.Class
{
	public class FolderSelectDialog
	{
		private OpenFileDialog ofd;

		public string FileName
		{
			get
			{
				return this.ofd.FileName;
			}
		}

		public string InitialDirectory
		{
			get
			{
				return this.ofd.InitialDirectory;
			}
			set
			{
				this.ofd.InitialDirectory = (value == null || value.Length == 0 ? Environment.CurrentDirectory : value);
			}
		}

		public string Title
		{
			get
			{
				return this.ofd.Title;
			}
			set
			{
				this.ofd.Title = (value == null ? "Select a folder" : value);
			}
		}

		public FolderSelectDialog()
		{
			this.ofd = new OpenFileDialog()
			{
				Filter = "Folders|\n",
				AddExtension = false,
				CheckFileExists = false,
				DereferenceLinks = true,
				Multiselect = false
			};
		}

		public bool ShowDialog()
		{
			return this.ShowDialog(IntPtr.Zero);
		}

		public bool ShowDialog(IntPtr hWndOwner)
		{
			bool flag = false;
			if (Environment.OSVersion.Version.Major < 6)
			{
				FolderBrowserDialog fbd = new FolderBrowserDialog()
				{
					Description = this.Title,
					SelectedPath = this.InitialDirectory,
					ShowNewFolderButton = false
				};
				if (fbd.ShowDialog(new WindowWrapper(hWndOwner)) != DialogResult.OK)
				{
					return false;
				}
				this.ofd.FileName = fbd.SelectedPath;
				flag = true;
			}
			else
			{
				Reflector r = new Reflector("System.Windows.Forms");
				uint num = 0;
				Type typeIFileDialog = r.GetType("FileDialogNative.IFileDialog");
				object dialog = r.Call(this.ofd, "CreateVistaDialog", new object[0]);
				r.Call(this.ofd, "OnBeforeVistaDialog", new object[] { dialog });
				uint options = (uint)r.CallAs(typeof(FileDialog), this.ofd, "GetOptions", new object[0]);
				options = options | (uint)r.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
				r.CallAs(typeIFileDialog, dialog, "SetOptions", new object[] { options });
				object pfde = r.New("FileDialog.VistaDialogEvents", new object[] { this.ofd });
				object[] parameters = new object[] { pfde, num };
				r.CallAs2(typeIFileDialog, dialog, "Advise", parameters);
				num = (uint)parameters[1];
				try
				{
					int num2 = (int)r.CallAs(typeIFileDialog, dialog, "Show", new object[] { hWndOwner });
					flag = num2 == 0;
				}
				finally
				{
					r.CallAs(typeIFileDialog, dialog, "Unadvise", new object[] { num });
					GC.KeepAlive(pfde);
				}
			}
			return flag;
		}
	}
}