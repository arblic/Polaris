using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polaris.Core {

	/// <summary>
	/// テキスト抽出
	/// </summary>
	public interface ITextExtractor {

		/// <summary>
		/// 抽出
		/// </summary>
		IEnumerable<TextData> Extract( string filePath );
	}
}
