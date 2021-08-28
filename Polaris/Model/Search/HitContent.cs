using System.IO;
using System.Diagnostics;

namespace Polaris.Models {

	/// <summary>
	/// 検索ヒットコンテンツ インターフェイス
	/// </summary>
	public interface IHitContent {
		string Text { get; }
		string FilePath { get; }
		string FileType { get; }
		string Directory { get; }
		string FileName { get; }
		int Sheet { get; }
		int Row { get; }
		int Column { get; }
		int Shape { get; }
	}

	/// <summary>
	/// 検索ヒットコンテンツ 実体
	/// </summary>
	public class HitContent : IHitContent {

		/// <summary>
		/// ctor
		/// </summary>
		public HitContent()
		#region
		{
		}
		#endregion

		/// <summary>
		/// セット
		/// </summary>
		public void set( string text, string filePath, string fileType, string sheet, string row, string column, string shape )
		#region
		{
			Text = text;
			FilePath = filePath;
			Directory = Path.GetDirectoryName( FilePath );
			FileName = Path.GetFileName( FilePath );
			FileType = fileType;

			int temp;
			if( int.TryParse( sheet, out temp ) ) {
				Sheet = temp;
			}
			if( int.TryParse( row, out temp ) ) {
				Row = temp;
			}
			if( int.TryParse( column, out temp ) ) {
				Column = temp;
			}
			if( int.TryParse( shape, out temp ) ) {
				Shape = temp;
			}
		}
		#endregion

		/// <summary>
		/// ファイルを開く
		/// </summary>
		public void OpenFile()
		#region
		{
			switch( FileType ) {
			case "excel":
				// その場所を指定して開きたい……
//				var uri = FilePath + "#'sheet" + Sheet.ToString() + "'!" + "R" + Row.ToString() + "C" + Column.ToString();
				Process.Start( FilePath );
				break;
			case "word":
				// その場所を指定して開きたい……
				Process.Start( FilePath );
				break;
			case "ppt":
				// その場所を指定して開きたい……
				Process.Start( FilePath );
				break;
			case "pdf":
				// その場所を指定して開きたい……
				Process.Start( FilePath );
				break;
			case "text":
				// これはどうでもいいや
				Process.Start( FilePath );
				break;
			}
		}
		#endregion

		/// <summary>
		/// ディレクトリを開く
		/// </summary>
		public void OpenDirectory()
		#region
		{
			string dir = Path.GetDirectoryName( FilePath );

			Process.Start( "EXPLORER.EXE", dir );
		}
		#endregion

		public string FilePath { get; private set; } = "";
		public string Directory { get; private set; } = "";
		public string FileName { get; private set; } = "";
		public string FileType { get; private set; } = "";
		public string Text { get; private set; } = "";
		public int Sheet { get; private set; } = 0;
		public int Row { get; private set; } = 0;
		public int Column { get; private set; } = 0;
		public int Shape { get; private set; } = 0;
	}
}
