using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;


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
	/// 検索システム
	/// </summary>
	public class SearchSystem : INotifyPropertyChanged {

		/// <summary>
		/// ctor
		/// </summary>
		public SearchSystem()
		#region
		{
			m_version = LuceneVersion.LUCENE_48;
			m_analyzer = new JapaneseAnalyzer( m_version );
			m_workingDir = Path.Combine( Path.GetTempPath(), "enlil" );
		}
		#endregion

		private void Cleanup()
		#region
		{
			if( null != m_indexWriter ) {
				m_indexWriter.Dispose();
			}
			if( null != m_fsDir ) {
				m_fsDir.Dispose();
			}

			m_indexWriter = null;
			m_fsDir = null;
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリ設定
		/// </summary>
		public bool SetDirectoryPath( string directoryPath )
		#region
		{
			Cleanup();

			if( !System.IO.Directory.Exists( directoryPath ) ) {
				LastIndexedDateTime = DateTime.MinValue;
				Indexed = false;
				return false;
			}

			// ディレクトリパスをハッシュ値化して作業ディレクトリを作成しておく
			string directoryHash = getDirectoryHash( directoryPath );
			string workingDir = Path.Combine( m_workingDir, directoryHash );

			// ワーキングディレクトリオープン（オープンした段階で、存在しなければ作成される）
			m_fsDir = FSDirectory.Open( workingDir );

			// インデクスライター作成
			var indexConfig = new IndexWriterConfig( m_version, m_analyzer );
			m_indexWriter = new IndexWriter( m_fsDir, indexConfig );

			// 最終インデクス化時間が記録されているか？
			m_lastTimeFile = Path.Combine( workingDir, "LastIndexdTime.txt" );
			if( File.Exists( m_lastTimeFile ) ) {
				using( StreamReader sr = new StreamReader( m_lastTimeFile ) ) {
					LastIndexedDateTime = DateTime.Parse( sr.ReadToEnd() );
					Indexed = true;
				}
			} else {
				LastIndexedDateTime = DateTime.MinValue;
				Indexed = false;
			}

			return true;
		}
		#endregion

		/// <summary>
		/// 削除
		/// </summary>
		public void DeleteDocuments()
		#region
		{
			m_indexWriter.DeleteAll();
			m_indexWriter.Flush( true, true );
			m_indexWriter.Commit();
		}
		#endregion

		/// <summary>
		/// 検索要素の登録
		/// </summary>
		public void Entry( IEnumerable<Document> documents )
		#region
		{
			m_indexWriter.AddDocuments( documents );
		}
		#endregion

		/// <summary>
		/// 検索インデクスのコミット、全 Entry 後に呼ぶ
		/// </summary>
		public void FlushCommit()
		#region
		{
			m_indexWriter.Flush( true, true );
			m_indexWriter.Commit();

			LastIndexedDateTime = DateTime.Now;
			Indexed = true;

			// 最後に更新した時間帯を記録
			using( var sw = new StreamWriter( m_lastTimeFile, false ) ) {
				sw.Write( LastIndexedDateTime.ToString() );
			}
		}
		#endregion

		/// <summary>
		/// 検索
		/// </summary>
		public IEnumerable<IHitContent> Search( string searchTerm )
		#region
		{
			List<HitContent> retval = new List<HitContent>();

			var reader = m_indexWriter.GetReader( true );
			IndexSearcher searcher = new IndexSearcher( reader );

			// クエリ作成
			var qb = new QueryBuilder( m_analyzer );
			var query = qb.CreatePhraseQuery( "text", searchTerm );

			if( null == query ) {
				return null;
			}

			// 検索
			var topDocs = searcher.Search(query, 1000);

			// 結果を格納
			foreach( var score in topDocs.ScoreDocs ) {

				var foundDoc = searcher.Doc( score.Doc );
				var hitContent = new HitContent();

				hitContent.set( foundDoc.GetField( "text" ).GetStringValue(),
								foundDoc.GetField( "fileName" ).GetStringValue(),
								foundDoc.GetField( "fileType" ).GetStringValue(),
								foundDoc.GetField( "sheet" ).GetStringValue(),
								foundDoc.GetField( "row" ).GetStringValue(),
								foundDoc.GetField( "column" ).GetStringValue(),
								foundDoc.GetField( "shape" ).GetStringValue() );

				retval.Add( hitContent );
			}

			return retval;
		}
		#endregion

		/// <summary>
		/// インデクス化がされているかどうか
		/// </summary>
		private bool m_indexed = false;
		public bool Indexed
		#region
		{
			get {
				return m_indexed;
			}
			set {
				m_indexed = value;
				RaisePropertyChanged( nameof( Indexed ) );
			}
		}
		#endregion

		/// <summary>
		/// ディレクトリのハッシュ値を返す
		/// </summary>
		public string getDirectoryHash( string directoryPath )
		#region
		{
			var targetBytes = Encoding.UTF8.GetBytes( directoryPath );
			var csp = new MD5CryptoServiceProvider();
			var hashBytes = csp.ComputeHash(targetBytes);

			var hashStr = new StringBuilder();
			foreach( var hashByte in hashBytes ) {
				hashStr.Append( hashByte.ToString( "x2" ) );
			}

			return hashStr.ToString();
		}
		#endregion

		/// <summary>
		/// 最後にインデクスを更新した時間
		/// </summary>
		public DateTime LastIndexedDateTime
		#region
		{
			get {
				return m_lastDate;
			}
			set {
				m_lastDate = value;
				RaisePropertyChanged( nameof( LastIndexedDateTime ) );
			}
		}
		#endregion

		/// <summary>
		/// プロパティ変更通知
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged( string propertyname )
		#region
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyname ) );
		}
		#endregion

		private LuceneVersion	m_version;		//!	バージョン
		private Analyzer		m_analyzer;		//!	アナライザ
		private string			m_workingDir;	//!	ワーキングディレクトリ名
		private FSDirectory		m_fsDir;		//!	ディレクトリ
		private IndexWriter		m_indexWriter;  //!	インデックスライター
		private string          m_lastTimeFile;	//!	最後にインデクス更新した日を記録するファイル名
		private DateTime        m_lastDate;		//!	最後にインデクスを更新した日
	}
}
