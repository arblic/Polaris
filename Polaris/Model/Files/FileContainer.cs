using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lucene.Net.Documents;

namespace Polaris.Models {

	/// <summary>
	/// ファイルコンテナ
	/// </summary>
	public class FileContainer {

		/// <summary>
		/// ctor
		/// </summary>
		public FileContainer( string filePath )
		#region
		{
			var ft = MainModel.FileTypeTable;

			FilePath = filePath;

			var ext = Path.GetExtension( FilePath ).ToLower();

			foreach( var table in ft ) {
				if( table.Value == ext ) {
					Type = table.Key;
					break;
				}
			}

			m_luceneDocuments = new List<Document>();
		}
		#endregion

		/// <summary>
		/// Lucene のドキュメントを構築する
		/// </summary>
		public void ConstructLuceneDocument()
		#region
		{
			IDocConverter  iDocConv = null;

			m_luceneDocuments.Clear();

			switch( Type ) {
			case FileTypes.XLSX:
			case FileTypes.XLSM:
				iDocConv = new DocConverterXLSX();
				break;
			case FileTypes.PPTX:
				iDocConv = new DocConverterPPTX();
				break;
			case FileTypes.DOCX:
				iDocConv = new DocConverterDOCX();
				break;
			case FileTypes.PDF:
				iDocConv = new DocConverterPDF();
				break;
			case FileTypes.TXT:
			case FileTypes.CPP:
			case FileTypes.HPP:
			case FileTypes.INL:
				iDocConv = new DocConverterTXT();
				break;
			}

			if( null != iDocConv ) {
				m_luceneDocuments.AddRange( iDocConv.GetDocs( FilePath ) );
			}
		}
		#endregion

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// ファイルタイプ
		/// </summary>
		public FileTypes Type { get; private set; }

		/// <summary>
		/// Lucene Document リスト
		/// </summary>
		private List<Document> m_luceneDocuments;
		public IEnumerable<Document> LuceneDocuments
		#region
		{
			get {
				return m_luceneDocuments;
			}
		}
		#endregion
	}
}
