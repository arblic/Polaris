using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Lucene.Net.Documents;

using ClosedXML.Excel;

namespace Polaris.Models {

	/// <summary>
	/// .xls の Document を生成する
	/// </summary>
	public class DocConverterXLSX : IDocConverter {

		/// <summary>
		/// Doc 取得
		/// </summary>
		public IEnumerable<Document> GetDocs( string filePath )
		{
			var retval = new List<Document>();
			int sheetNo;

			try {

				using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) ) {

					using( var workbook = new XLWorkbook( fs ) ) {

						sheetNo = 1;

						foreach( var sheet in workbook.Worksheets ) {

							var usedRange = sheet.RangeUsed();
							if( null == usedRange ) {
								continue;
							}

							for( int r = 1; r <= usedRange.RowCount(); ++r ) {
								for( int c = 1; c <= usedRange.ColumnCount(); ++c ) {

									var cell = usedRange.Cell( r, c );

									var str = cell.Value.ToString();

									// 文字列が存在するセルのみピックアップする
									if( !String.IsNullOrWhiteSpace( str ) ) {

										var fieldFileName   = new StringField( "fileName", filePath, Field.Store.YES );
										var fieldFileType   = new StringField( "fileType", "excel", Field.Store.YES );
										var fieldSheet      = new StringField( "sheet", sheetNo.ToString(), Field.Store.YES );
										var fieldRow        = new StringField( "row", cell.Address.RowNumber.ToString(), Field.Store.YES );
										var fieldColumn     = new StringField( "column", cell.Address.ColumnNumber.ToString(), Field.Store.YES );
										var fieldShape      = new StringField( "shape", "0", Field.Store.YES );
										var fieldText       = new TextField( "text", str, Field.Store.YES );

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
								}
							}

							++sheetNo;
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
