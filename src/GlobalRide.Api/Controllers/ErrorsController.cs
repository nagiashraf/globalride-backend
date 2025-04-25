#pragma warning disable S6931

using Microsoft.AspNetCore.Mvc;

namespace GlobalRide.Api.Controllers;

/// <summary>
/// Controller to handle errors globally.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    /// <summary>
    /// Handles errors globally and returns a ProblemDetails response.
    /// </summary>
    /// <returns>An IActionResult representing the ProblemDetails response.</returns>
    [Route("/error")]
    public IActionResult HandleError() => Problem();
}
