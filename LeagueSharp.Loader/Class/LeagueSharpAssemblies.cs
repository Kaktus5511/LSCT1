using LeagueSharp.Loader.Data;
using PlaySharp.Service.WebService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LeagueSharp.Loader.Class
{
	public static class LeagueSharpAssemblies
	{
		public static List<LeagueSharpAssembly> GetAssemblies(string directory, string url = "")
		{
			// 
			// Current member / type: System.Collections.Generic.List`1<LeagueSharp.Loader.Class.LeagueSharpAssembly> LeagueSharp.Loader.Class.LeagueSharpAssemblies::GetAssemblies(System.String,System.String)
			// File path: G:\LeagueSharp\loader.exe
			// 
			// Product version: 2016.3.1003.0
			// Exception in: System.Collections.Generic.List<LeagueSharp.Loader.Class.LeagueSharpAssembly> GetAssemblies(System.String,System.String)
			// 
			// Der Objektverweis wurde nicht auf eine Objektinstanz festgelegt.
			//    bei ..(ICollection`1 ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:Zeile 515.
			//    bei ..() in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:Zeile 445.
			//    bei ..() in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:Zeile 363.
			//    bei ..() in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:Zeile 307.
			//    bei ..( ,  ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:Zeile 88.
			//    bei ..(MethodBody ,  , ILanguage ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:Zeile 88.
			//    bei ..(MethodBody , ILanguage ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:Zeile 70.
			//    bei ..( , ILanguage , MethodBody , & ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:Zeile 95.
			//    bei ..(MethodBody , ILanguage , & ,  ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:Zeile 58.
			//    bei ..(ILanguage , MethodDefinition ,  ) in c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:Zeile 117.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public static LeagueSharpAssembly GetAssembly(string projectFile, string url = "")
		{
			LeagueSharpAssembly foundAssembly = null;
			try
			{
				foundAssembly = new LeagueSharpAssembly(Path.GetFileNameWithoutExtension(projectFile), projectFile, url);
			}
			catch (Exception exception)
			{
				Exception e = exception;
				Utility.Log(LogStatus.Error, "Updater GetAssembly", e.ToString(), Logs.MainLog);
			}
			return foundAssembly;
		}
	}
}