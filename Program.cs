using Pacs008_Validator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<XsdValidator>();

var app = builder.Build();

app.MapControllers();
app.Run();