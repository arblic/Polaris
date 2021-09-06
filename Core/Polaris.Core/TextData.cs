namespace Polaris.Core {

	/// <summary>
	/// テキストデータ
	/// </summary>
	public struct TextData {
		public string fileName;	//!	ファイル名
		public string fileType;	//!	ファイルタイプ
		public uint sheetNo;    //!	シート番号
		public uint row;        //!	行番号
		public uint column;     //!	列番号
		public uint shape;      //!	シェイプ番号
		public string text;     //!	テキスト
	}
}
