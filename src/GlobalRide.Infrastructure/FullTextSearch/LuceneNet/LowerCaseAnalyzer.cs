using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Icu;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace GlobalRide.Infrastructure.FullTextSearch.LuceneNet;

/// <summary>
/// A custom Lucene.NET analyzer that performs text normalization and folding to lowercase for improved search consistency.
/// </summary>
internal sealed class NormalizationAndFoldingAnalyzer(LuceneVersion luceneVersion) : Analyzer
{
    private readonly LuceneVersion _luceneVersion = luceneVersion;

    /// <summary>
    /// Creates the components for tokenizing and filtering text.
    /// </summary>
    /// <param name="fieldName">The name of the field being analyzed.</param>
    /// <param name="reader">The <see cref="TextReader"/> providing the input text.</param>
    /// <returns>
    /// A <see cref="TokenStreamComponents"/> object containing the tokenizer and filter chain.
    /// </returns>
    /// <remarks>
    /// The components include:
    /// - A <see cref="StandardTokenizer"/> to split text into tokens.
    /// - An <see cref="ICUNormalizer2Filter"/> to normalize Unicode characters.
    /// - An <see cref="ICUFoldingFilter"/> to fold text to lowercase and handle case-insensitive matching.
    /// </remarks>
    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
    {
        Tokenizer tokenizer = new StandardTokenizer(_luceneVersion, reader);
        TokenStream filter = new ICUNormalizer2Filter(tokenizer);
        filter = new ICUFoldingFilter(filter);
        return new TokenStreamComponents(tokenizer, filter);
    }
}
