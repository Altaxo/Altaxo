using System;
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;


namespace Altaxo.Main.Commands
{
	public class CreateNewWorksheet : AbstractMenuCommand
	{
		public override void Run()
		{
			App.Current.CreateNewWorksheet();
		}
	}
	public class CreateNewGraph : AbstractMenuCommand
	{
		public override void Run()
		{
			App.Current.CreateNewGraph();
		}
	}
}
