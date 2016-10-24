using LeagueSharp.Loader.Data;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace LeagueSharp.Loader.Class
{
	[Serializable]
	internal class ProjectFile
	{
		public readonly Microsoft.Build.Evaluation.Project Project;

		private readonly Log log;

		public string Configuration
		{
			get;
			set;
		}

		public string PlatformTarget
		{
			get;
			set;
		}

		public string ReferencesPath
		{
			get;
			set;
		}

		public ProjectFile(string file, Log log)
		{
			try
			{
				this.log = log;
				if (File.Exists(file))
				{
					ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
					this.Project = new Microsoft.Build.Evaluation.Project(file);
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				Utility.Log(LogStatus.Error, "ProjectFile", string.Format("Error - {0}", ex.Message), this.log);
			}
		}

		public void Change()
		{
			ProjectMetadata metadata;
			string evaluatedValue;
			try
			{
				if (this.Project != null)
				{
					this.Project.SetGlobalProperty("Configuration", this.Configuration);
					this.Project.SetGlobalProperty("Platform", this.PlatformTarget);
					this.Project.SetGlobalProperty("PlatformTarget", this.PlatformTarget);
					this.Project.SetGlobalProperty("PreBuildEvent", string.Empty);
					this.Project.SetGlobalProperty("PostBuildEvent", string.Empty);
					this.Project.SetGlobalProperty("PreLinkEvent", string.Empty);
					this.Project.SetGlobalProperty("DebugSymbols", (this.Configuration == "Release" ? "false" : "true"));
					this.Project.SetGlobalProperty("DebugType", (this.Configuration == "Release" ? "None" : "full"));
					this.Project.SetGlobalProperty("Optimize", (this.Configuration == "Release" ? "true" : "false"));
					this.Project.SetGlobalProperty("DefineConstants", (this.Configuration == "Release" ? "TRACE" : "DEBUG;TRACE"));
					this.Project.SetGlobalProperty("OutputPath", string.Concat("bin\\", this.Configuration, "\\"));
					foreach (ProjectItem item in this.Project.GetItems("Reference"))
					{
						if (item != null)
						{
							metadata = item.GetMetadata("HintPath");
						}
						else
						{
							metadata = null;
						}
						ProjectMetadata hintPath = metadata;
						if (hintPath != null)
						{
							evaluatedValue = hintPath.EvaluatedValue;
						}
						else
						{
							evaluatedValue = null;
						}
						if (string.IsNullOrWhiteSpace(evaluatedValue))
						{
							continue;
						}
						item.SetMetadataValue("HintPath", Path.Combine(this.ReferencesPath, Path.GetFileName(hintPath.EvaluatedValue)));
					}
					this.Project.Save();
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				Utility.Log(LogStatus.Error, "ProjectFile", ex.Message, this.log);
			}
		}
	}
}