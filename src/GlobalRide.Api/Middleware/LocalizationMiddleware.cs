using System.Globalization;

namespace GlobalRide.Api.Middleware;

public class LocalizationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var cultureQuery = context.Request.Query["languagecode"].ToString();

        cultureQuery = cultureQuery switch
        {
            "en" => "en-US",
            "fr" => "fr-FR",
            "es" => "es-ES",
            "ar" => "ar-SA",
            _ => cultureQuery,
        };

        var culture = !string.IsNullOrEmpty(cultureQuery)
            ? cultureQuery
            : context.Request.Headers.AcceptLanguage.ToString().Substring(0, 2)
                ?? "en-US";

        CultureInfo.CurrentUICulture = new CultureInfo(culture);
        await _next(context);
    }
}
