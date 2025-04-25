using System.Globalization;

using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Common;

using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Payloads;
using Lucene.Net.Store;
using Lucene.Net.Util;

using Directory = Lucene.Net.Store.Directory;

namespace GlobalRide.Infrastructure.FullTextSearch.LuceneNet;

/// <summary>
/// Provides full-text search functionality for branches by name, city, or country using Lucene.NET for indexing and querying.
/// This service must be registered in DI Container as a singleton.
/// </summary>
internal sealed class LuceneBranchSearchingService : IBranchSearchingService, IDisposable
{
    private const LuceneVersion LuceneNetVersion = LuceneVersion.LUCENE_48;
    private readonly IndexWriter _indexWriter;
    private readonly Directory _directory;
    private readonly Analyzer _analyzer;

    /// <summary>
    /// Initializes a new instance of the <see cref="LuceneBranchSearchingService"/> class.
    /// </summary>
    /// <param name="indexesDirectoryPath">The directory path where the Lucene index will be stored.</param>
    public LuceneBranchSearchingService(string indexesDirectoryPath)
    {
        string indexPath = Path.Combine(indexesDirectoryPath, "BranchesIndex");
        _directory = FSDirectory.Open(indexPath);
        _analyzer = new NormalizationAndFoldingAnalyzer(LuceneNetVersion);
        var indexConfig = new IndexWriterConfig(LuceneNetVersion, _analyzer)
        {
            OpenMode = OpenMode.CREATE,
        };
        _indexWriter = new IndexWriter(_directory, indexConfig);
    }

    /// <summary>
    /// Represents the fields names in the Lucene index.
    /// </summary>
    public enum IndexField
    {
        /// <summary>
        /// The unique identifier of the branch.
        /// </summary>
        BranchId,

        /// <summary>
        /// The language code of the branch.
        /// </summary>
        LanguageCode,

        /// <summary>
        /// The name of the branch.
        /// </summary>
        Name,

        /// <summary>
        /// The city where the branch is located.
        /// </summary>
        City,

        /// <summary>
        /// The country where the branch is located.
        /// </summary>
        Country,

        /// <summary>
        /// The type of the branch.
        /// </summary>
        BranchType,

        /// <summary>
        /// The IANA time zone identifier for the branch city.
        /// </summary>
        TimeZone,
    }

    /// <summary>
    /// Adds a collection of branch search results to the Lucene index.
    /// </summary>
    /// <param name="branches">The collection of branch search results to add to the index.</param>
    public void AddRange(IEnumerable<BranchSearchResultResponse> branches)
    {
        var branchIdField = new StringField(
            nameof(IndexField.BranchId),
            string.Empty,
            Field.Store.YES);

        var languageCodeField = new StringField(
            nameof(IndexField.LanguageCode),
            string.Empty,
            Field.Store.YES);

        var nameField = new TextField(
            nameof(IndexField.Name),
            string.Empty,
            Field.Store.YES);

        var cityField = new TextField(
            nameof(IndexField.City),
            string.Empty,
            Field.Store.YES);

        var countryField = new TextField(
            nameof(IndexField.Country),
            string.Empty,
            Field.Store.YES);

        var branchTypeField = new StringField(
            nameof(IndexField.BranchType),
            string.Empty,
            Field.Store.YES);

        var timeZoneField = new StringField(
            nameof(IndexField.TimeZone),
            string.Empty,
            Field.Store.YES);

        var doc = new Document
        {
            branchIdField,
            languageCodeField,
            nameField,
            cityField,
            countryField,
            branchTypeField,
            timeZoneField,
        };

        foreach (BranchSearchResultResponse branch in branches)
        {
            branchIdField.SetStringValue(branch.Id.ToString());
            languageCodeField.SetStringValue(branch.LanguageCode.ToString());
            nameField.SetStringValue(branch.Name);
            cityField.SetStringValue(branch.City);
            countryField.SetStringValue(branch.Country);
            branchTypeField.SetStringValue(branch.Type.ToString());
            timeZoneField.SetStringValue(branch.TimeZone.ToString());
            _indexWriter.AddDocument(doc);
        }

        _indexWriter.Commit();
    }

    /// <summary>
    /// Searches for branches based on a search term and language code.
    /// </summary>
    /// <param name="searchTerm">The search term to query the index.</param>
    /// <param name="languageCode">The preferred language code for boosting search results.</param>
    /// <param name="maxResultsCount">The maximum number of results to return.</param>
    /// <returns>
    /// A list of <see cref="BranchSearchResultResponse"/> objects representing the search results.
    /// </returns>
    /// <remarks>
    /// This method performs a search using a combination of prefix, fuzzy, and payload queries on the branch name, city, and country fields.
    /// Results are boosted based on the preferred language code.
    /// </remarks>
    public IReadOnlyList<BranchSearchResultResponse> Search(string searchTerm, LanguageCode languageCode, int maxResultsCount)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        searchTerm = searchTerm.Trim();

        using var dirReader = DirectoryReader.Open(_directory);
        var searcher = new IndexSearcher(dirReader);

        var branchQuery = new BooleanQuery();
        AddFieldQuery(branchQuery, searchTerm, IndexField.Name, 4.0f);
        AddFieldQuery(branchQuery, searchTerm, IndexField.City, 10.0f);
        AddFieldQuery(branchQuery, searchTerm, IndexField.Country, 8.0f);

        var languageBoostQuery = new LanguageBoostQuery(branchQuery, languageCode);

        var topDocs = searcher.Search(languageBoostQuery, maxResultsCount).ScoreDocs;

        var results = topDocs.Select(scoreDoc =>
        {
            Document doc = searcher.Doc(scoreDoc.Doc);
            return new BranchSearchResultResponse(
                new Guid(doc.Get(nameof(IndexField.BranchId), CultureInfo.InvariantCulture)),
                Enum.Parse<LanguageCode>(doc.Get(nameof(IndexField.LanguageCode), CultureInfo.InvariantCulture)),
                doc.Get(nameof(IndexField.Name), CultureInfo.InvariantCulture),
                doc.Get(nameof(IndexField.City), CultureInfo.InvariantCulture),
                doc.Get(nameof(IndexField.Country), CultureInfo.InvariantCulture),
                Enum.Parse<BranchType>(doc.Get(nameof(IndexField.BranchType), CultureInfo.InvariantCulture)),
                doc.Get(nameof(IndexField.TimeZone), CultureInfo.InvariantCulture));
        }).ToList();

        return results;
    }

    /// <summary>
    /// Disposes of the Lucene index writer, analyzer, and directory.
    /// </summary>
    public void Dispose()
    {
        _indexWriter.Dispose();
        _analyzer.Dispose();
        _directory.Dispose();
    }

    private static void AddFieldQuery(BooleanQuery query, string searchTerm, IndexField field, float boost)
    {
        string fieldName = field.ToString();
        int fuzzyQueryDistance = 1;

        var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var phraseQuery = new PhraseQuery();
        foreach (var term in terms)
        {
            phraseQuery.Add(new Term(fieldName, term));

            query.Add(
                new PrefixQuery(
                    new Term(fieldName, term))
                {
                    Boost = boost,
                },
                Occur.SHOULD);

            query.Add(
                new FuzzyQuery(
                    new Term(fieldName, term),
                    fuzzyQueryDistance),
                Occur.SHOULD);

            query.Add(
                new PayloadTermQuery(
                    new Term(fieldName, term),
                    new AveragePayloadFunction()),
                Occur.SHOULD);
        }

        phraseQuery.Boost = boost;
        query.Add(phraseQuery, Occur.SHOULD);
    }
}
