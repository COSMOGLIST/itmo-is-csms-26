namespace Lab3.Middleware;

public class ExceptionFormattingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            string message =
                $"Exception occured while processing request, type = {e.GetType().Name}, message = {e.Message}";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message });
        }
    }
}