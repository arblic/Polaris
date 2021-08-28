using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Ja;
using Lucene.Net.Analysis.Ja.Dict;
using Lucene.Net.Analysis.Ja.TokenAttributes;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Util;

using Hnx8.ReadJEnc;

namespace Polaris.Models {

	/// <summary>
	/// 全文検索テスト
	/// </summary>
	public class LuceneTest {

		/// <summary>
		/// ctor
		/// </summary>
		public LuceneTest()
		#region
		{
			m_version = LuceneVersion.LUCENE_48;
			m_analyzer = new JapaneseAnalyzer( m_version );
			//m_analyzer = new StandardAnalyzer( m_version );
			m_workingDir = Path.Combine( Path.GetTempPath(), "enlil" );
		}
		#endregion

		~LuceneTest()
		#region
		{
			m_indexWriter.DeleteAll();
			m_indexWriter.Commit();
		}
		#endregion

		public void standby( string filePath )
		#region
		{
			FilePicker fp = new FilePicker();
			fp.Reload( filePath, "*.txt" );

			m_fsDir = FSDirectory.Open( m_workingDir );
			var indexConfig = new IndexWriterConfig(m_version, m_analyzer);
			m_indexWriter = new IndexWriter(m_fsDir, indexConfig );

			//m_indexWriter.DeleteAll();
			//m_indexWriter.DeleteUnusedFiles();
			//m_indexWriter.Commit();

			//foreach( var fileContainer in fp.Containers ) {

			//	int row = 1;

			//	FileInfo fi = new FileInfo( fileContainer.FilePath );
			//	Encoding enc = Encoding.Unicode;

			//	// エンコード判別
			//	using( FileReader reader = new FileReader( fi ) ) {
			//		enc = reader.Read(fi).GetEncoding();
			//	}

			//	using( StreamReader sr = new StreamReader( fileContainer.FilePath, enc ) ) {

			//		while( !sr.EndOfStream ) {

			//			var fieldFileName = new StringField( "fileName", fileContainer.FilePath, Field.Store.YES );
			//			var fieldRow = new StringField( "row", row.ToString(), Field.Store.YES );
			//			var fieldText = new TextField( "text", sr.ReadLine(), Field.Store.YES );

			//			Document doc = new Document();
			//			doc.Add( fieldFileName );
			//			doc.Add( fieldRow );
			//			doc.Add( fieldText );

			//			m_indexWriter.AddDocument( doc );

			//			++row;
			//		}
			//	}
			//}

			//m_indexWriter.Flush( true, true );
			//m_indexWriter.Commit();

		}
		#endregion

		public void search( string searchTerm )
		#region
		{
			var reader = m_indexWriter.GetReader( true );
			IndexSearcher searcher = new IndexSearcher( reader );

			var qb = new QueryBuilder( m_analyzer );
			//var query = qb.CreateMinShouldMatchQuery( "text", searchTerm, 0.0f );
			var query = qb.CreatePhraseQuery( "text", searchTerm );
			//var query = new MultiPhraseQuery();
			//query.Add( new Term( "text", searchTerm ) );

			//QueryParser parser = new QueryParser( m_version, "text", m_analyzer);
			//parser.AllowLeadingWildcard = true;
			//Query query = parser.Parse(searchTerm);

			var topDocs = searcher.Search(query, 20);

			foreach( var score in topDocs.ScoreDocs ) {
				var foundDoc = searcher.Doc( score.Doc );
				var fieldFile = foundDoc.GetField( "fileName" );
				var fieldRow = foundDoc.GetField( "row" );
				var fieldText = foundDoc.GetField( "text" );
				Debug.WriteLine( fieldFile.GetStringValue() + "(" + fieldRow.GetStringValue() + "): " + fieldText.GetStringValue() );
			}
		}
		#endregion

		private LuceneVersion m_version;
		private Analyzer m_analyzer;
		private string m_workingDir;
		private FSDirectory m_fsDir;
		private IndexWriter m_indexWriter;
	}
}
