using GlobalRide.Domain.Common.Result;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GlobalRide.Api.Controllers;

/// <summary>
/// A base API controller that provides common functionality for handling errors and returning standardized problem details.
/// </summary>
[ApiController]
public class ApiController : ControllerBase
{
    /// <summary>
    /// Handles a list of errors and returns an appropriate <see cref="IActionResult"/> based on the error types.
    /// </summary>
    /// <param name="errors">A list of <see cref="AppError"/> objects representing the errors to handle.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the problem details response. If the list is empty, a generic problem response is returned.
    /// If all errors are validation errors, a validation problem response is returned. Otherwise, the first error in the list is used to generate the response.
    /// </returns>
    protected IActionResult Problem(List<AppError> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    /// <summary>
    /// Handles a single error and returns an appropriate <see cref="IActionResult"/> based on the error type.
    /// </summary>
    /// <param name="error">The <see cref="AppError"/> object representing the error to handle.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the problem details response. The status code is determined by the error type:
    /// <list type="bullet">
    ///   <item><description>Validation errors return a 400 Bad Request.</description></item>
    ///   <item><description>Not Found errors return a 404 Not Found.</description></item>
    ///   <item><description>Conflict errors return a 409 Conflict.</description></item>
    ///   <item><description>All other errors return a 500 Internal Server Error.</description></item>
    /// </list>
    /// </returns>
    protected IActionResult Problem(AppError error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode, detail: error.Message);
    }

    /// <summary>
    /// Handles a list of validation errors and returns a validation problem response.
    /// </summary>
    /// <param name="errors">A list of <see cref="AppError"/> objects representing the validation errors.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing a validation problem response.
    /// </returns>
    protected IActionResult ValidationProblem(List<AppError> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(
                error.Code,
                error.Message);
        }

        return ValidationProblem(modelStateDictionary);
    }
}
