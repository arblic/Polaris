using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Lucene.Net.Documents;

using Hnx8.ReadJEnc;

namespace Polaris.Models {

	/// <summary>
	/// .txt の Document を生成する
	/// </summary>
	public class DocConverterTXT : IDocConverter {

		/// <summary>
		/// Doc 取得
		/// </summary>
		public IEnumerable<Document> GetDocs( string filePath )
		{
			var retval = new List<Document>();
			int row = 1;

			FileInfo fi = new FileInfo( filePath );
			Encoding enc = Encoding.Unicode;

			// エンコード判別
			using( FileReader reader = new FileReader( fi ) ) {
				enc = reader.Read( fi ).GetEncoding();
			}

			using( StreamReader sr = new StreamReader( filePath, enc ) ) {

				while( !sr.EndOfStream ) {

					var txt = sr.ReadLine();

					// 文字列が存在する行のみピックアップする
					if( !string.IsNullOrWhiteSpace( txt ) ) {

						var fieldFileName   = new StringField( "fileName", filePath, Field.Store.YES );
						var fieldFileType   = new StringField( "fileType", "text", Field.Store.YES );
						var fieldSheet      = new StringField( "sheet", "0", Field.Store.YES );
						var fieldRow        = new StringField( "row", row.ToString(), Field.Store.YES );
						var fieldColumn     = new StringField( "column", "0", Field.Store.YES );
						var fieldShape      = new StringField( "shape", "0", Field.Store.YES );
						var fieldText       = new TextField( "text", txt, Field.Store.YES );

						Document doc = new Document();

						doc.Add( fieldFileName );
						doc.Add( fieldFileType );
						doc.Add( fieldSheet );
						doc.Add( fieldRow );
						doc.Add( fieldColumn );
						doc.Add( fieldShape );
						doc.Add( fieldText );

						retval.Add( doc );
					}

					++row;
				}
			}

			return retval;
		}
	}
}
