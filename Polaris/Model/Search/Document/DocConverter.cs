using System;
using System.Collections.Generic;
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

namespace Polaris.Models {

	/// <summary>
	/// Lucene の Document を生成する
	/// </summary>
	public interface IDocConverter {

		/// <summary>
		/// ドキュメント取得
		/// </summary>
		IEnumerable<Document> GetDocs( string filePath );
	}
}
