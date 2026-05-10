using Pacs008_Validator.Application.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<XsdValidator>();   // singleton — schema compiled once

var app = builder.Build();

app.MapControllers();
app.Run();