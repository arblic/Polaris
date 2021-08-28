using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Presentation;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Polaris.Models {

	/// <summary>
	/// .pptx の Document を生成する
	/// </summary>
	public class DocConverterPPTX : IDocConverter {

		/// <summary>
		/// Doc 取得
		/// </summary>
		public IEnumerable<Document> GetDocs( string filePath )
		#region
		{
			var retval = new List<Document>();
			int rowNo, slideNo;

			slideNo = 1;

			using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) ) {

				using( var presentationDoc = PresentationDocument.Open( fs, false ) ) {

					foreach( var slide in presentationDoc.PresentationPart.SlideParts ) {

						if( slide.Slide != null ) {

							rowNo = 1;

							foreach( var paragraph in slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>() ) {

								StringBuilder paragraphText = new StringBuilder();

								foreach( var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>() ) {
									paragraphText.Append( text.Text );
								}

								if( 0 < paragraphText.Length ) {

									var fieldFileName   = new StringField( "fileName", filePath, Field.Store.YES );
									var fieldFileType   = new StringField( "fileType", "ppt", Field.Store.YES );
									var fieldSheet      = new StringField( "sheet", slideNo.ToString(), Field.Store.YES );
									var fieldRow        = new StringField( "row", rowNo.ToString(), Field.Store.YES );
									var fieldColumn     = new StringField( "column", "0", Field.Store.YES );
									var fieldShape      = new StringField( "shape", "0", Field.Store.YES );
									var fieldText       = new TextField( "text", paragraphText.ToString(), Field.Store.YES );

									var doc = new Document {	fieldFileName,
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
						}

						++slideNo;
					}
				}
			}

			return retval;
		}
		#endregion
	}
}
