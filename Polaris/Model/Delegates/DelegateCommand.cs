using System;
using System.Windows.Input;

namespace Polaris.ViewModels {

	/// <summary>
	/// デリゲートコマンド
	/// </summary>
	public class DelegateCommand : ICommand {

		/// <summary>
		/// ctor
		/// </summary>
		public DelegateCommand( Action execute ) : this( execute, () => true ) { }

		/// <summary>
		/// ctor
		/// </summary>
		public DelegateCommand(Action execute, Func<bool> canExecute)
		#region
		{
			this.execute = execute;
            this.canExecute = canExecute;
        }
		#endregion

		/// <summary>
		/// 実行可能か？
		/// </summary>
		public bool CanExecute(object parameter)
		#region
		{
			return canExecute();
        }
		#endregion

		/// <summary>
		/// 実行
		/// </summary>
		public void Execute(object parameter)
		#region
		{
			execute();
        }
		#endregion

		/// <summary>
		/// 実行可能確認イベントハンドラ
		/// </summary>
		public event EventHandler CanExecuteChanged
		#region
		{
			add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
		#endregion

		Action execute;
        Func<bool> canExecute;
    }


	public class DelegateCommand<T> : ICommand {

		private readonly Action<T> _execute;
		private readonly Func<bool> _canExecute;

		public event EventHandler CanExecuteChanged
		#region
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
        }
		#endregion

		public DelegateCommand( Action<T> execute ) : this( execute, () => true ) { }

		public DelegateCommand( Action<T> execute, Func<bool> canExecute )
		#region
		{
			this._execute = execute;
			this._canExecute = canExecute;
		}
		#endregion

		public void Execute( object parameter )
		#region
		{
			this._execute( (T)parameter );
		}
		#endregion

		public bool CanExecute( object parameter )
		#region
		{
			return this._canExecute();
		}
		#endregion
	}
}
