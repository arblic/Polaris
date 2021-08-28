using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polaris.Models {

	/// <summary>
	/// ファイル状態チェッカ
	/// </summary>
	public class FileStateChecker {

		/// <summary>
		/// ctor
		/// </summary>
		public FileStateChecker()
		#region
		{
		}
		#endregion

		/// <summary>
		/// 実行
		/// </summary>
		public IEnumerable<FileState> Execute( string directoryPath )
		#region
		{
			var ft = MainModel.FileTypeTable;
			FilePicker filePicker = new FilePicker();
			List<FileState> fileStates = new List<FileState>();
			string searchPattern = "";

			foreach( var pair in ft ) {

				searchPattern = "*" + pair.Value;

				if( filePicker.Reload( directoryPath, searchPattern ) ) {
					var fs = new FileState();
					fs.Set( pair.Key, pair.Value, filePicker.Containers.Count );
					fileStates.Add( fs );
				}
			}

			return fileStates;
		}
		#endregion
	}
}
