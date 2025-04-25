namespace GlobalRide.Api.Controllers.Branches;

public sealed record class SearchBranchesRequest(
    string SearchTerm,
    string LanguageCode,
    int ResultsCount);
