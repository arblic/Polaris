using System.Collections.Generic;

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
