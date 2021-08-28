using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

using Polaris.ViewModels;

namespace Polaris.Views {

	/// <summary>
	/// IndexingDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class IndexingDialog : Window {

		#region Dependency Properties
		public static readonly DependencyProperty SearchDirectoryProperty =
		DependencyProperty.Register("SearchDirectory",
									typeof(string),
									typeof(IndexingDialog),
									new FrameworkPropertyMetadata("SearchDirectory", new PropertyChangedCallback(OnSearchDirectoryChanged)));
		#endregion

		public IndexingDialog()
		#region
		{
			InitializeComponent();

			SearchDirectory = "";

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
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
		/// ビューモデル側で何かが変更された
		/// </summary>
		private void ViewModel_PropertyChanged( object sender, PropertyChangedEventArgs e )
		#region
		{
			switch( e.PropertyName ) {
			case "Closing":
				// 別スレッドから呼ばれても問題なく閉じる
				Dispatcher.Invoke( () => Close() );
				break;
			}
		}
		#endregion

		#region "最大化・最小化・閉じるボタンの非表示設定"
		[DllImport( "user32.dll" )]
		private static extern int GetWindowLong( IntPtr hWnd, int nIndex );
		[DllImport( "user32.dll" )]
		private static extern int SetWindowLong( IntPtr hWnd, int nIndex, int dwNewLong );

		const int GWL_STYLE = -16;
		const int WS_SYSMENU = 0x80000;

		protected override void OnSourceInitialized( EventArgs e )
		{
			base.OnSourceInitialized( e );
			IntPtr handle = new WindowInteropHelper(this).Handle;
			int style = GetWindowLong(handle, GWL_STYLE);
			style = style & (~WS_SYSMENU);
			SetWindowLong( handle, GWL_STYLE, style );
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリプロパティ
		/// </summary>
		public string SearchDirectory
		#region
		{
			get { return (string)GetValue( SearchDirectoryProperty ); }
			set { SetValue( SearchDirectoryProperty, value ); }
		}
		#endregion

		/// <summary>
		/// 検索ディレクトリ変更時
		/// </summary>
		private static void OnSearchDirectoryChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e )
		#region
		{
			var indexDlg = obj as IndexingDialog;
			if( indexDlg != null ) {
				indexDlg.ViewModel.SearchDirectory = indexDlg.SearchDirectory;
			}
		}
		#endregion

		/// <summary>
		/// ビューモデル
		/// </summary>
		private IndexingDialogViewModel ViewModel
		#region
		{
			get {
				return DataContext as IndexingDialogViewModel;
			}
		}
		#endregion
	}
}
