using Microsoft.OpenApi.Models;
using Microsoft.Owin.Security.OAuth;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.Controllers;
using PaymentGatewayApi.PaymentModels;
using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AddJWTTokenServicesExtensions.AddJWTTokenServices(builder.Services,builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                    }
                },
                new string[] {}
        }
    });
});
builder.Services.AddCors();
builder.Services.AddTransient<PaymentGatewayContext>((a) => new LibraryContextFactory().Create());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGatewayApi v1"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials());

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});