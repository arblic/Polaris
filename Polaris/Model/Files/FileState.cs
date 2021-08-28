using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polaris.Models {

	/// <summary>
	/// ファイルの状態
	/// </summary>
	public class FileState {

		/// <summary>
		/// ctor
		/// </summary>
		public FileState()
		#region
		{
		}
		#endregion

		/// <summary>
		/// 設定
		/// </summary>
		public void Set( FileTypes fileType, string fileTypeStr, int count )
		#region
		{
			FileType = fileType;
			FileTypeStr = fileTypeStr;
			Count = count;
		}
		#endregion

		public FileTypes FileType { get; private set; } = FileTypes.INVALID;
		public string FileTypeStr { get; private set; } = "";
		public int Count { get; private set; } = 0;
	}
}
