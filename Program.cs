var builder = WebApplication.CreateBuilder(args);

// Agrega CORS para permitir la conexion desde cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Usa CORS
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}



// * Endpoint root * \\
app.MapGet("/", () =>
{
    var mensaje = new { message = "Bienvenido a la APICube de TechBridge" };
    return Results.Json(mensaje);
})
.WithName("RootEndpoint")
.WithOpenApi();


// * Endpoints que realizan consultas al modelo multidimensional de TechBridge * \\
EndpointsMultidimensional.MapEndpoints(app);


// * Endpoints que realizan consultas al modelo tabular de TechBridge * \\
EndpointsTabular.MapEndpoints(app);


// Ejecutar la API
app.Run();
