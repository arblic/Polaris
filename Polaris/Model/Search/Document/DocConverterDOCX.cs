using DocumentFormat.OpenXml.Packaging;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Polaris.Models {

	/// <summary>
	/// .docx の Document を生成する
	/// </summary>
	public class DocConverterDOCX : IDocConverter {

		/// <summary>
		/// Doc 取得
		/// </summary>
		public IEnumerable<Document> GetDocs( string filePath )
		#region
		{
			var retval = new List<Document>();

			const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
			StringBuilder textBuilder = new StringBuilder();

			using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) ) {
				using( var wordDocument = WordprocessingDocument.Open( fs, false ) ) {

					NameTable nt = new NameTable();
					XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
					nsManager.AddNamespace( "w", wordmlNamespace );

					XmlDocument xdoc = new XmlDocument(nt);
					xdoc.Load( wordDocument.MainDocumentPart.GetStream() );

					XmlNodeList paragraphNodes = xdoc.SelectNodes("//w:p", nsManager);
					foreach( XmlNode paragraphNode in paragraphNodes ) {
						XmlNodeList textNodes = paragraphNode.SelectNodes(".//w:t", nsManager);
						foreach( XmlNode textNode in textNodes ) {
							textBuilder.Append( textNode.InnerText );
						}
						textBuilder.Append( Environment.NewLine );
					}
				}
			}

			// 改行文字で区切る
			var innerTexts = textBuilder.ToString().Replace( "\r\n", "\n" );
			var lineTexts = new List<string>(innerTexts.Split('\n'));
			int rowNo = 1;

			foreach( var lineText in lineTexts ) {

				if( !String.IsNullOrWhiteSpace( lineText ) ) {

					var fieldFileName   = new StringField( "fileName", filePath, Field.Store.YES );
					var fieldFileType   = new StringField( "fileType", "word", Field.Store.YES );
					var fieldSheet      = new StringField( "sheet", "0", Field.Store.YES );
					var fieldRow        = new StringField( "row", rowNo.ToString(), Field.Store.YES );
					var fieldColumn     = new StringField( "column", "0", Field.Store.YES );
					var fieldShape      = new StringField( "shape", "0", Field.Store.YES );
					var fieldText       = new TextField( "text", lineText, Field.Store.YES );

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

				++rowNo;
			}

			return retval;
		}
		#endregion
	}
}
