using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.WindowsAPICodePack.Dialogs;

using Polaris.ViewModels;

namespace Polaris.Views {

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {

		/// <summary>
		/// ctor
		/// </summary>
		public MainWindow()
		#region
		{
			InitializeComponent();

			TextCompositionManager.AddPreviewTextInputHandler( m_searchTermTextBox, OnPreviewTextInput );
			TextCompositionManager.AddPreviewTextInputUpdateHandler( m_searchTermTextBox, OnPreviewTextInputUpdate );
		}
		#endregion

		/// <summary>
		/// ロード時
		/// </summary>
		private void OnLoaded( object sender, RoutedEventArgs e )
		#region
		{
			ViewModel.OnLoaded();
		}
		#endregion

		/// <summary>
		/// ファイルオープンダイアログボタン押した
		/// </summary>
		private void OnDirOpenDlgClick( object sender, RoutedEventArgs e )
		#region
		{
			var comnOpenFileDlg = new CommonOpenFileDialog();

			comnOpenFileDlg.Title = "検索するフォルダを選択してください";
			comnOpenFileDlg.InitialDirectory = ViewModel.SearchDirectory;
			// フォルダ選択モードにする
			comnOpenFileDlg.IsFolderPicker = true;

			// フォルダが選択されたら変更をかける
			if( CommonFileDialogResult.Ok == comnOpenFileDlg.ShowDialog() ) {
				ViewModel.SearchDirectory = comnOpenFileDlg.FileName;
			}
		}
		#endregion

		/// <summary>
		/// インデクスダイアログボタン押した
		/// </summary>
		private void OnIndexingDlgClick( object sender, RoutedEventArgs e )
		#region
		{
			// ここで予測時間とか出しておきたい
			var fileCountStr = ViewModel.CheckFiles();

			if( MessageBoxResult.OK != MessageBox.Show( fileCountStr, "確認", MessageBoxButton.OKCancel ) ) {
				return;
			}

			var idxDlg = new IndexingDialog();

			// 検索ディレクトリを設定
			idxDlg.SearchDirectory = ViewModel.SearchDirectory;

			// 表示するとインデクス化を開始する。完了するまで待機
			idxDlg.ShowDialog();
		}
		#endregion

		private bool m_isImeOnConv = false;
		private int m_enterKeyBuffer { get; set; }

		/// <summary>
		/// テキスト入力前
		/// </summary>
		private void OnPreviewTextInput( object sender, TextCompositionEventArgs e )
		#region
		{
			m_enterKeyBuffer = m_isImeOnConv ? 1 : 0;
			m_isImeOnConv = false;
		}
		#endregion

		/// <summary>
		/// テキスト入力更新
		/// </summary>
		private void OnPreviewTextInputUpdate( object sender, TextCompositionEventArgs e )
		#region
		{
			m_isImeOnConv = (e.TextComposition.CompositionText.Length != 0);
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリにフォーカスが入ったとき
		/// </summary>
		private void SearchDirectoryTextBox_GotFocus( object sender, RoutedEventArgs e )
		#region
		{
			Dispatcher.InvokeAsync( () => {
				Task.Delay( 0 );
				var tb = sender as TextBox;
				tb?.SelectAll();
			} );
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリ名変更時
		/// </summary>
		private void SearchedDirectoryTextChanged( object sender, TextChangedEventArgs e )
		#region
		{
			ViewModel.SearchDirectory = m_searchDirectoryTextBox.Text;
		}
		#endregion

		/// <summary>
		/// 検索文言
		/// </summary>
		private void searchTermTextBox_KeyUp( object sender, KeyEventArgs e )
		#region
		{
			if( m_isImeOnConv == false && e.Key == Key.Enter && m_enterKeyBuffer == 1 ) {
				m_enterKeyBuffer = 0;
			} else if( m_isImeOnConv == false && e.Key == Key.Enter && m_enterKeyBuffer == 0 ) {
				ViewModel.SearchTerm = m_searchTermTextBox.Text;
			}
		}
		#endregion

		/// <summary>
		/// リストビューのファイル開く
		/// </summary>
		private void ListViewItem_OpenFile( object sender, RoutedEventArgs e )
		#region
		{
			if( sender is MenuItem menuItem ) {
				ViewModel.OpenFile( menuItem.DataContext );
			}
		}
		#endregion

		/// <summary>
		/// リストビューのファイル開く（ダブルクリックイベント）
		/// </summary>
		private void ListViewItem_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		#region
		{
			if( sender is ListViewItem item ) {
				ViewModel.OpenFile( item.DataContext );
			}
		}
		#endregion

		/// <summary>
		/// リストビューのフォルダ開く
		/// </summary>
		private void ListViewItem_OpenFolder( object sender, RoutedEventArgs e )
		#region
		{
			if( sender is MenuItem menuItem ) {
				ViewModel.OpenDirectory( menuItem.DataContext );
			}
		}
		#endregion

		/// <summary>
		/// ビューモデル
		/// </summary>
		public MainWindowViewModel ViewModel
		#region
		{
			get {
				return DataContext as MainWindowViewModel;
			}
		}
		#endregion
	}
}
