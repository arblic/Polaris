using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

using Polaris.Models;

namespace Polaris.ViewModels {

	/// <summary>
	/// インデクス作成ダイアログ ビューモデル
	/// </summary>
	public class IndexingDialogViewModel : INotifyPropertyChanged {

		#region for Binding Commands
		public ICommand CloseOnViewModel { get; private set; }
		#endregion

		/// <summary>
		/// ctor
		/// </summary>
		public IndexingDialogViewModel()
		#region
		{
			CloseOnViewModel = new DelegateCommand( () => { Closing = true; } );
			RaisePropertyChanged( nameof( CloseOnViewModel ) );
		}
		#endregion

		/// <summary>
		/// ロード時
		/// </summary>
		public void OnLoaded()
		#region
		{
			// インデクス化開始
			Task.Run( () => Indexing() );
		}
		#endregion

		/// <summary>
		/// インデクス
		/// </summary>
		private async Task Indexing()
		#region
		{
			var filePicker = MainModel.FilePicker;
			var searchSystem = MainModel.SearchSystem;
			var exts = MainModel.FileTypeTable.Values.ToList();
			int nowCount = 0;
			int maxCount = 0;

			// 検索用にアスタリスクを全部に付与
			for( int i = 0; i < exts.Count; ++i ) {
				exts[i] = "*" + exts[i];
			}

			// ドキュメントを消しておく
			searchSystem.DeleteDocuments();

			ProgressStateStr = "ファイルをリストアップしています……";
			ProgressValue = 0.0;

			await Task.Run( () => {
				filePicker.Reload( SearchDirectory, exts.ToArray() );
			} );

			nowCount = 0;
			maxCount = filePicker.Containers.Count;
			ProgressStateStr = "ファイルのドキュメント化を行っています……";
			ProgressValue = 5.0;

			await Task.Run( () => {
				filePicker.Containers.ForEach( x => {
					x.ConstructLuceneDocument();
					++nowCount;
					ProgressValue = 5.0 + (double)nowCount / (double)maxCount * 50.0;
				} );
			} );

			nowCount = 0;
			maxCount = filePicker.Containers.Count;
			ProgressStateStr = "ファイルのドキュメントを転地インデクス化しています……";
			ProgressValue = 55.0;

			await Task.Run( () => {
				filePicker.Containers.ForEach( x => {
					searchSystem.Entry( x.LuceneDocuments );
					++nowCount;
					ProgressValue = 55.0 + (double)nowCount / (double)maxCount * 45.0;
				} );
			} );

			searchSystem.FlushCommit();

			ProgressStateStr = "完了";
			ProgressValue = 100.0;

			await Task.Run( () => Thread.Sleep( 1000 ) );

			Closing = true;
		}
		#endregion

		/// <summary>
		/// 閉じる
		/// </summary>
		private bool m_closing = false;
		public bool Closing
		#region
		{
			get {
				return m_closing;
			}
			set {
				if( m_closing != value ) {
					m_closing = value;
					RaisePropertyChanged( nameof( Closing ) );
				}
			}
		}
		#endregion

		/// <summary>
		/// 実行状態
		/// </summary>
		private string m_progressStateStr = "";
		public string ProgressStateStr
		#region
		{
			get {
				return m_progressStateStr;
			}
			set {
				if( m_progressStateStr != value ) {
					m_progressStateStr = value;
					RaisePropertyChanged( nameof( ProgressStateStr ) );
				}
			}
		}
		#endregion

		/// <summary>
		/// 進捗度合い
		/// </summary>
		private double m_progressValue = 0.0;
		public double ProgressValue
		#region
		{
			get {
				return m_progressValue;
			}
			set {
				m_progressValue = value;
				RaisePropertyChanged( nameof( ProgressValue ) );
			}
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリ
		/// </summary>
		private string m_searchDirectory = "";
		public string SearchDirectory
		#region
		{
			get {
				return m_searchDirectory;
			}
			set {
				m_searchDirectory = value;
				RaisePropertyChanged( nameof( SearchDirectory ) );
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
	}
}
