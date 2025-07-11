var builder = WebApplication.CreateBuilder(args);

// Add Clean Architecture Layers
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
