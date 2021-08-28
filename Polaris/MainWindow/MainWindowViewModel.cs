using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;

using Polaris.Models;

namespace Polaris.ViewModels {

	public class MainWindowViewModel : INotifyPropertyChanged {

		/// <summary>
		/// ctor
		/// </summary>
		public MainWindowViewModel()
		#region
		{
			string titleName = "Polaris ver.";

			FileVersionInfo ver = FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location );

			titleName += ver.FileVersion;

			TitleName = titleName;

			MainModel.Initialize();
			MainModel.SearchSystem.PropertyChanged += SearchSystem_PropertyChanged;

			m_searchDirectory = @"C:\Users";
		}
		#endregion

		/// <summary>
		/// ロード完了時
		/// </summary>
		public void OnLoaded()
		#region
		{
			// 初回チェック
			MainModel.SearchSystem.SetDirectoryPath( SearchDirectory );
		}
		#endregion

		/// <summary>
		/// ファイルチェックして何がどれくらいあるか返す
		/// </summary>
		public string CheckFiles()
		#region
		{
			string retval = "";

			FileStateChecker fsChecker = new FileStateChecker();

			// チェック
			var fileList = fsChecker.Execute( SearchDirectory );

			retval += "指定フォルダ " + SearchDirectory + " のファイル総数は以下の通りです：\n";
			foreach( var fs in fileList ) {
				if( 0 < fs.Count ) {
					retval += fs.FileTypeStr + " が " + fs.Count.ToString() + " 個\n";
				}
			}

			return retval;
		}
		#endregion

		/// <summary>
		/// タイトルバー名
		/// </summary>
		private string m_titleName = "";
		public string TitleName
		#region
			{
			get {
				return m_titleName;
			}
			set {
				if( m_titleName != value ) {
					m_titleName = value;
					RaisePropertyChanged( nameof( TitleName ) );
				}
			}
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリ名
		/// </summary>
		private string m_searchDirectory = "";
		public string SearchDirectory
		#region
		{
			get {
				return m_searchDirectory;
			}
			set {
				if( m_searchDirectory != value ) {
					m_searchDirectory = value;
					RaisePropertyChanged( nameof( SearchDirectory ) );

					MainModel.SearchSystem.SetDirectoryPath( SearchDirectory );
					HitContents = null;
				}
			}
		}
		#endregion

		/// <summary>
		/// インデクス化されているかの状態
		/// </summary>
		private string m_indexStateText = "";
		public string IndexStateText
		#region
		{
			get {
				return m_indexStateText;
			}
			set {
				if( m_indexStateText != value ) {
					m_indexStateText = value;
					RaisePropertyChanged( nameof( IndexStateText ) );
				}
			}
		}
		#endregion

		/// <summary>
		/// インデクス化状態カラー
		/// </summary>
		private Brush m_indexStateColor;
		public Brush IndexStateColor
		#region
		{
			get {
				return m_indexStateColor;
			}
			set {
				if( m_indexStateColor != value ) {
					m_indexStateColor = value;
					RaisePropertyChanged( nameof( IndexStateColor ) );
				}
			}
		}
		#endregion

		/// <summary>
		/// ファイル開く
		/// </summary>
		public void OpenFile( object dataContext )
		#region
		{
			if( dataContext is HitContent hitCont ) {
				hitCont.OpenFile();
			}
		}
		#endregion

		/// <summary>
		/// フォルダ開く
		/// </summary>
		public void OpenDirectory( object dataContext )
		#region
		{
			if( dataContext is HitContent hitCont ) {
				hitCont.OpenDirectory();
			}
		}
		#endregion

		/// <summary>
		/// 検索単語
		/// </summary>
		private string m_searchTerm = "";
		public string SearchTerm
		#region
		{
			get {
				return m_searchTerm;
			}
			set {
				if( m_searchTerm != value ) {
					m_searchTerm = value;

					HitContents = null;

					// インデクス化がされていれば検索する
					var searchSystem = MainModel.SearchSystem;
					if( searchSystem.Indexed ) {
						if( !string.IsNullOrWhiteSpace( m_searchTerm ) ) {
							HitContents = searchSystem.Search( m_searchTerm );
						}
					}

					RaisePropertyChanged( nameof( SearchTerm ) );
				}
			}
		}
		#endregion

		/// <summary>
		/// 検索でヒットしたコンテンツリスト
		/// </summary>
		private IEnumerable<IHitContent> m_hitContents = null;
		public IEnumerable<IHitContent> HitContents
		#region
		{
			get {
				return m_hitContents;
			}
			set {
				if( m_hitContents != value ) {
					m_hitContents = value;
					RaisePropertyChanged( nameof( HitContents ) );
				}
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

		/// <summary>
		/// 検索システムのプロパティ変更通知
		/// </summary>
		private void SearchSystem_PropertyChanged( object sender, PropertyChangedEventArgs e )
		#region
		{
			var searchSystem = MainModel.SearchSystem;

			switch( e.PropertyName ) {
			case "LastIndexedDateTime":
				if( searchSystem.LastIndexedDateTime == DateTime.MinValue ) {
					IndexStateText = "指定されたフォルダはまだインデクス化されていません！";
				} else {
					var timeStr = searchSystem.LastIndexedDateTime.ToString( "yyyy/MM/dd hh:mm:ss" );
					IndexStateText = "指定されたフォルダは " + timeStr + " にインデクス化されています。";
				}
				break;
			case "Indexed":
				if( searchSystem.Indexed ) {
					IndexStateColor = Brushes.Black;
				} else {
					IndexStateColor = Brushes.Red;
				}
				break;
			default:	break;
			}
		}
		#endregion
	}
}
