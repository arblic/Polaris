using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polaris.Models {

	/// <summary>
	/// ファイルピッカー
	/// </summary>
	public class FilePicker {

		/// <summary>
		/// ctor
		/// </summary>
		public FilePicker()
		#region
		{
			Containers = new List<FileContainer>();
			CheckPath = "";
		}
		#endregion

		/// <summary>
		/// リロード
		/// </summary>
		public bool Reload( string checkPath, string ext = "*.*" )
		#region
		{
			Containers.Clear();

			// 引数パスが無効ではない
			if( !String.IsNullOrWhiteSpace( checkPath ) ) {
				CheckPath = checkPath;
			}

			// 現在設定されているパスが有効
			if( String.IsNullOrWhiteSpace( CheckPath ) ) {
				return false;
			}

			// 全ピックアップ
			var files = Directory.EnumerateFiles( CheckPath, ext, SearchOption.AllDirectories );
			foreach( var filePath in files ) {
				var fc = new FileContainer( filePath );
				Containers.Add( fc );
			}

			return true;
		}
		#endregion

		/// <summary>
		/// リロード
		/// </summary>
		public bool Reload( string checkPath, string[] exts )
		#region
		{
			Containers.Clear();

			// 引数パスが無効ではない
			if( !String.IsNullOrWhiteSpace( checkPath ) ) {
				CheckPath = checkPath;
			}

			// 現在設定されているパスが有効
			if( String.IsNullOrWhiteSpace( CheckPath ) ) {
				return false;
			}

			// 
			foreach( var ext in exts ) {
				var files = Directory.EnumerateFiles( CheckPath, ext, SearchOption.AllDirectories );
				foreach( var filePath in files ) {
					var fc = new FileContainer( filePath );
					Containers.Add( fc );
				}
			}

			return true;
		}
		#endregion

		/// <summary>
		/// チェックパス
		/// </summary>
		public string CheckPath { get; set; }

		/// <summary>
		/// パスリスト
		/// </summary>
		public List<FileContainer> Containers { get; private set; }
	}
}
