using AppointmetScheduler.Data;
using AppointmetScheduler.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
//Scalar integration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Appointment Scheduler API", Version = "v1" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token"))),
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<ISchedulingServices, SchedulingServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 2. MIDDLEWARE TO SERVE SWAGGER SPEC AND SCALAR UI
app.UseSwagger();
// 3. Custom endpoint to serve the Scalar UI page.
// Access the documentation at /scalar
app.MapGet("/scalar", async context =>
{
    // HTML template that loads Scalar from CDN and points it to the local swagger.json file
    var html = @"
    <!DOCTYPE html>
    <html>
    <head>
        <title>API Documentation (Scalar)</title>
        <meta charset='utf-8'/>
        <meta name='viewport' content='width=device-width, initial-scale=1'>
        <style>
            /* Ensure the UI takes up the full page */
            body { margin: 0; padding: 0; height: 100vh; width: 100vw; overflow-x: hidden; }
        </style>
    </head>
    <body>
        <script id='scalar-cdn' 
            data-url='/swagger/v1/swagger.json'
            data-theme='purple' 
            data-layout='aside'
        ></script>
        <script async id='scalar-script' 
            src='https://cdn.jsdelivr.net/npm/@scalar/api-reference'>
        </script>
    </body>
    </html>";

    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(html);
});
// ******************************************************
app.UseCors("AllowAngularApp");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
