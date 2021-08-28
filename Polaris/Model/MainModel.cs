using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Polaris.Models {

	/// <summary>
	/// メインモデル
	/// </summary>
	public static class MainModel {

		/// <summary>
		/// 初期化
		/// </summary>
		public static bool Initialize()
		#region
		{
			SearchSystem = new SearchSystem();
			FilePicker = new FilePicker();

			FileTypeTable = new Dictionary<FileTypes, string>();
			FileTypeTable.Add( FileTypes.XLSX, ".xlsx" );
			FileTypeTable.Add( FileTypes.XLSM, ".xlsm" );
			FileTypeTable.Add( FileTypes.PPTX, ".pptx" );
			FileTypeTable.Add( FileTypes.DOCX, ".docx" );
			FileTypeTable.Add( FileTypes.PDF,  ".pdf" );
			FileTypeTable.Add( FileTypes.TXT,  ".txt" );
			FileTypeTable.Add( FileTypes.CPP,  ".cpp" );
			FileTypeTable.Add( FileTypes.HPP,  ".h" );
			FileTypeTable.Add( FileTypes.INL,  ".inl" );

			return true;
		}
		#endregion

		/// <summary>
		/// 検索システム
		/// </summary>
		public static SearchSystem SearchSystem
		#region
		{
			get; private set;
		}
		#endregion

		/// <summary>
		/// ファイルピッカー
		/// </summary>
		public static FilePicker FilePicker
		#region
		{
			get; private set;
		}
		#endregion

		/// <summary>
		/// ファイルタイプテーブル
		/// </summary>
		public static Dictionary<FileTypes, string> FileTypeTable
		#region
		{
			get; private set;
		}
		#endregion
	}
}
