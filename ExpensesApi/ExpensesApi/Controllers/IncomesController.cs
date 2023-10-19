using System.Diagnostics;
using ExpensesApi.Models;
using ExpensesApi.Registries;
using ExpensesApi.Utility;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesApi.Controllers;

[ApiController]
[Route("api/incomes", Name = "incomes")]
public class IncomesController : ControllerBase
{
    #region Private fields

    private readonly IQueryParametersValidator _validator;
    private readonly IIncomesRegistry _registry;
    private readonly ILogger<IncomesController> _logger;

    #endregion

    #region Initialization

    public IncomesController(ILoggerFactory loggerFactory, IIncomesRegistry registry, IQueryParametersValidator validator)
    {
        _logger = loggerFactory is not null ? loggerFactory.CreateLogger<IncomesController>() : throw new ArgumentNullException(nameof(loggerFactory));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    #endregion

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Income>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(499, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<ActionResult> GetAllAsync([FromQuery] IncomesGetAllQueryParameters? queryParameters, [FromHeader(Name = "Username")] string? username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(GetAllAsync)} invoked. Username: {username}");
        var sw = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException();
            }

            var filterParameters = _validator.Validate(queryParameters);

            var incomes = await _registry.GetAllAsync(username, filterParameters, cancellationToken);

            _logger.LogInformation($"{nameof(GetAllAsync)} completed. Username: {username}; Incomes count: {incomes.Count}. Elapsed time: {sw.Elapsed}");

            return Ok(incomes);
        }
        catch (InvalidOperationException)
        {
            _logger.LogError($"Invalid username: {username}. Elapsed time: {sw.Elapsed}");
            return Unauthorized(new Error { ErrorMessage = $"Invalid username: {username}" });
        }
        catch (FormatException e)
        {
            _logger.LogError(e, $"Invalid query parameter. Username: {username}. Elapsed time: {sw.Elapsed}");
            return BadRequest(new Error { ErrorMessage = $"Invalid query parameter: {e.Message}" });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"Operation cancelled by client. Username: {username}. Elapsed time: {sw.Elapsed}");
            return Cancelled(new Error { ErrorMessage = "Operation cancelled by client" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while retrieving incomes. Username: {username}. Elapsed time: {sw.Elapsed}");
            return InternalServerError(new Error { ErrorMessage = $"An error occurred: {e.Message}" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Income))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(499, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<ActionResult> GetAsync([FromRoute(Name = "id")] Guid id, [FromHeader(Name = "Username")] string? username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(GetAsync)} invoked. Username: {username}");
        var sw = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(nameof(username));
            }

            var income = await _registry.GetAsync(username, id, cancellationToken);
            if (income is null)
            {
                var errorMessage = $"No income found with given ID: {id}. Username: {username}";
                _logger.LogError($"{errorMessage}. Elapsed time: {sw.Elapsed}");
                return NotFound(new Error { ErrorMessage = errorMessage });
            }

            _logger.LogInformation($"{nameof(GetAsync)} completed. Username: {username}; Income: {income}. Elapsed time: {sw.Elapsed}");

            return Ok(income);
        }
        catch (InvalidOperationException)
        {
            _logger.LogError($"Invalid username: {username}. Elapsed time: {sw.Elapsed}");
            return Unauthorized(new Error { ErrorMessage = $"Invalid username: {username}" });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"Operation cancelled by client. Username: {username}. Elapsed time: {sw.Elapsed}");
            return Cancelled(new Error { ErrorMessage = "Operation cancelled by client" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while retrieving income. Username: {username}. Elapsed time: {sw.Elapsed}");
            return InternalServerError(new Error { ErrorMessage = $"An error occurred: {e.Message}" });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Income))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(499, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<ActionResult> CreateAsync([FromBody] IncomeDetails incomeDetails, [FromHeader(Name = "Username")] string? username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(CreateAsync)} invoked. Username: {username}");
        var sw = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var createdIncome = await _registry.InsertAsync(username, incomeDetails, cancellationToken);

            _logger.LogInformation($"{nameof(CreateAsync)} completed. Username: {username}; Expense: {createdIncome}. Elapsed time: {sw.Elapsed}");

            return CreatedAtRoute("incomes", createdIncome);
        }
        catch (InvalidOperationException)
        {
            _logger.LogError($"Invalid username: {username}. Elapsed time: {sw.Elapsed}");
            return Unauthorized(new Error { ErrorMessage = $"Invalid username: {username}" });
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, $"Invalid input argument. Username: {username} Elapsed time: {sw.Elapsed}");
            return BadRequest(new Error { ErrorMessage = e.Message });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"Operation cancelled by client. Username: {username}. Elapsed time: {sw.Elapsed}");
            return Cancelled(new Error { ErrorMessage = "Operation cancelled by client" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while retrieving incomes. Username: {username}. Elapsed time: {sw.Elapsed}");
            return InternalServerError(new Error { ErrorMessage = $"An error occurred: {e.Message}" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Income))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(499, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<ActionResult> UpdateAsync([FromRoute(Name = "id")] Guid id, [FromBody] IncomeDetails incomeDetails, [FromHeader(Name = "Username")] string? username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(UpdateAsync)} invoked. Username: {username}");
        var sw = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var updatedRecord = await _registry.UpdateAsync(username, id, incomeDetails, cancellationToken);

            _logger.LogInformation($"{nameof(UpdateAsync)} completed. Username: {username}; Updated record: {updatedRecord}. Elapsed time: {sw.Elapsed}");

            return Ok(updatedRecord);
        }
        catch (InvalidOperationException)
        {
            _logger.LogError($"Invalid username: {username}. Elapsed time: {sw.Elapsed}");
            return Unauthorized(new Error { ErrorMessage = $"Invalid username: {username}" });
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, $"Invalid input argument. Elapsed time: {sw.Elapsed}");
            return BadRequest(new Error { ErrorMessage = e.Message });
        }
        catch (NotFoundException e)
        {
            var errorMessage = $"No income found with given ID: {id}. Username: {username}";
            _logger.LogError(e, $"{errorMessage}. Elapsed time: {sw.Elapsed}");
            return NotFound(new Error { ErrorMessage = errorMessage });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"Operation cancelled by client. Username: {username}. Elapsed time: {sw.Elapsed}");
            return Cancelled(new Error { ErrorMessage = "Operation cancelled by client" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while updating income with ID: {id}. Username: {username}. Elapsed time: {sw.Elapsed}");
            return InternalServerError(new Error { ErrorMessage = $"An error occurred: {e.Message}" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(499, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<ActionResult> DeleteAsync([FromRoute(Name = "id")] Guid id, [FromHeader(Name = "Username")] string? username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(DeleteAsync)} invoked");
        var sw = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            await _registry.DeleteAsync(username, id, cancellationToken);

            _logger.LogInformation($"{nameof(DeleteAsync)} completed. Elapsed time: {sw.Elapsed}");

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            _logger.LogError($"Invalid username: {username}. Elapsed time: {sw.Elapsed}");
            return Unauthorized(new Error { ErrorMessage = $"Invalid username: {username}" });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"Operation cancelled by client. Elapsed time: {sw.Elapsed}");
            return Cancelled(new Error { ErrorMessage = "Operation cancelled by client" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while deleting income with ID: {id}. Elapsed time: {sw.Elapsed}");
            return InternalServerError(new Error { ErrorMessage = $"An error occurred: {e.Message}" });
        }
    }

    #region Utility Methods

    private ObjectResult Cancelled(object value)
    {
        return StatusCode(499, value);
    }

    private ObjectResult InternalServerError(object value)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, value);
    }

    #endregion
}