using FastEndpoints;

using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Extensions;
using GamingApp.ApiService.Validation;

using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Endpoints;

public class GetUserProfileEndpoint(AppDbContext dbContext, ILogger<GetUserProfileEndpoint> logger)
    : EndpointWithoutRequest<UserProfileResponse>
{
    public override void Configure()
    {
        Get("/userProfile");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var user = await UserHelper.EnsureUserExistsAsync(HttpContext, dbContext, logger, ct);
                if (user == null)
                {
                    await SendUnauthorizedAsync(ct);
                    return;
                }

                var response = user.ToProfileResponse();
                await SendOkAsync(response, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching user profile");
                await SendErrorsAsync(500, ct);
            }
        }
    }
}

public class CreateUserProfileEndpoint(AppDbContext dbContext, ILogger<CreateUserProfileEndpoint> logger)
    : Endpoint<CreateUserProfileRequest, UserProfileResponse>
{
    public override void Configure()
    {
        Put("/userProfile");
    }

    public override async Task HandleAsync(CreateUserProfileRequest req, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                // Check if username is unique
                var user = await UserHelper.EnsureUserExistsAsync(HttpContext, dbContext, logger, ct);
                if (user == null)
                {
                    await SendUnauthorizedAsync(ct);
                    return;
                }

                // Check if username is unique excluding the current user
                if (await dbContext.Users.AnyAsync(u => u.InGameUserName == req.InGameUserName && u.Id != user.Id, ct))
                {
                    AddError("InGameUserName is already taken");
                    await SendErrorsAsync(cancellation: ct);
                    return;
                }

                user.InGameUserName = req.InGameUserName;
                if (!string.IsNullOrEmpty(req.Bio)) user.Bio = req.Bio;
                if (!string.IsNullOrEmpty(req.Status)) user.Status = req.Status;

                await dbContext.SaveChangesAsync(ct);

                var response = user.ToProfileResponse();
                await SendOkAsync(response, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating user profile");
                await SendErrorsAsync(500, ct);
            }
        }
    }
}

public class CheckUsernameUniquenessEndpoint(AppDbContext dbContext, ILogger<CheckUsernameUniquenessEndpoint> logger)
    : EndpointWithoutRequest<bool>
{
    public override void Configure()
    {
        Get("/userProfile/checkUsername");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var username = HttpContext.Request.Query["username"].ToString();
                var exists = await dbContext.Users.AnyAsync(u => u.InGameUserName == username, ct);
                await SendOkAsync(!exists, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking username uniqueness");
                await SendErrorsAsync(500, ct);
            }
        }
    }
}
