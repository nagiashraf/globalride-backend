#pragma warning disable CA1305

using GlobalRide.Domain.Common;

using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Queries;
using Lucene.Net.Search;

namespace GlobalRide.Infrastructure.FullTextSearch.LuceneNet;

/// <summary>
/// A custom Lucene.NET query scorer that boosts search results based on the preferred language and English as a fallback.
/// </summary>
public class LanguageBoostQuery(Query subQuery, LanguageCode preferredLanguageCode)
    : CustomScoreQuery(subQuery)
{
    private const float PreferredLanguageBoostFactor = 100;
    private const float EnglishLanguageBoostFactor = 10;
    private const LanguageCode EnglishLanguageCode = LanguageCode.en;
    private readonly LanguageCode _preferredLanguageCode = preferredLanguageCode;

    /// <summary>
    /// Creates a custom score provider to apply language-specific boosts to search results.
    /// </summary>
    /// <param name="context">The atomic reader context for the query execution.</param>
    /// <returns>
    /// A <see cref="CustomScoreProvider"/> instance that applies the language boost logic.
    /// </returns>
    protected override CustomScoreProvider GetCustomScoreProvider(AtomicReaderContext context)
        => new LanguageBoostScoreProvider(context, _preferredLanguageCode);

    private sealed class LanguageBoostScoreProvider(AtomicReaderContext context, LanguageCode preferredLanguageCode)
        : CustomScoreProvider(context)
    {
        private readonly LanguageCode _preferredLanguageCode = preferredLanguageCode;

        public override float CustomScore(int doc, float subQueryScore, float valSrcScore)
        {
            Document document = m_context.AtomicReader.Document(doc);
            LanguageCode documentLanguageCode = Enum.Parse<LanguageCode>(
                document.Get(nameof(LuceneBranchSearchingService.IndexField.LanguageCode)));

            if (documentLanguageCode == _preferredLanguageCode)
            {
                return subQueryScore * PreferredLanguageBoostFactor;
            }

            if (_preferredLanguageCode != EnglishLanguageCode && documentLanguageCode == EnglishLanguageCode)
            {
                return subQueryScore * EnglishLanguageBoostFactor;
            }

            return subQueryScore;
        }
    }
}
