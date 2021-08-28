using System;
using System.Collections.Generic;
using System.Diagnostics;

using Lucene.Net.Documents;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Polaris.Models {

	/// <summary>
	/// .pdf の Document を生成する
	/// </summary>
	public class DocConverterPDF : IDocConverter {

		/// <summary>
		/// Doc 取得
		/// </summary>
		public IEnumerable<Document> GetDocs( string filePath )
		{
			var retval = new List<Document>();
			int rowNo;

			try {

				var reader = new PdfReader( filePath );
				var texts = new List<string>();

				for( int i = 1; i <= reader.NumberOfPages; ++i ) {

					var innerTexts = PdfTextExtractor.GetTextFromPage( reader, i, new SimpleTextExtractionStrategy() );

					// 改行文字で区切る
					innerTexts = innerTexts.Replace( "\r\n", "\n" );
					var lineTexts = new List<string>(innerTexts.Split('\n'));

					rowNo = 1;

					foreach( var lineText in lineTexts ) {

						if( !String.IsNullOrWhiteSpace( lineText ) ) {

							var fieldFileName   = new StringField( "fileName", filePath, Field.Store.YES );
							var fieldFileType   = new StringField( "fileType", "pdf", Field.Store.YES );
							var fieldSheet      = new StringField( "sheet", i.ToString(), Field.Store.YES );
							var fieldRow        = new StringField( "row", rowNo.ToString(), Field.Store.YES );
							var fieldColumn     = new StringField( "column", "0", Field.Store.YES );
							var fieldShape      = new StringField( "shape", "0", Field.Store.YES );
							var fieldText       = new Lucene.Net.Documents.TextField( "text", lineText, Field.Store.YES );

							var doc = new Document {
								fieldFileName,
								fieldFileType,
								fieldSheet,
								fieldRow,
								fieldColumn,
								fieldShape,
								fieldText
							};

							retval.Add( doc );
						}
					}
				}

			} catch( Exception e ) {
				Debug.WriteLine( e.ToString() );
			}

			return retval;
		}
	}
}
