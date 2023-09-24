namespace ConTest.ApiKey
{
    public class ApiKey
    {
        private readonly RequestDelegate _next;
        private
        const string APIKEY = "XApiKey";
        public ApiKey(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out
                    var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("No se ha enviado ApiKey");
                return;
            }
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Sin Autorizacion");
                return;
            }
            await _next(context);
        }
    }
}
