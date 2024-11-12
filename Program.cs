using Microsoft.AnalysisServices.AdomdClient; // Para conectar con SSAS y trabajar con CellSet
using System.Collections.Generic; // Para usar List y Dictionary
using Microsoft.AspNetCore.Builder; // Para la configuración del WebApplication
using Microsoft.Extensions.DependencyInjection; // Para el servicio del API explorer
using Microsoft.Extensions.Hosting;
using Microsoft.AnalysisServices;
using ApiCubeTB.Utils;

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

// Instancia de la clase que contiene los helpers
var helper = new MyHelperClass();


// * Endpoints que realizan consultas al modelo multidimensional de TechBridge * \\

// Endpoint root
app.MapGet("/", () =>
{
    var mensaje = new { message = "Bienvenido a la APICube de TechBridge" };
    return Results.Json(mensaje);
})
.WithName("RootEndpoint")
.WithOpenApi();



// Endpoint de de prueba de la conexion al cubo
app.MapGet("/cubedatatest", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = " SELECT NON EMPTY { [Measures].[Profit Margin] } ON COLUMNS, NON EMPTY { ([Project].[Project Name].[Project Name].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM [Soft Developers DW] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetCubeData")
.WithOpenApi();



// Endpoint que retorna la ganancia de 15 proyectos ordenados alfabeticamente
app.MapGet("/ganancia_15_proyectos_o_alfabet", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) MEMBER [Measures].[Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Payment Amount], 0) SELECT {[Measures].[Profit Margin Without Nulls], [Measures].[Payment Amount Without Nulls]} ON COLUMNS, TOPCOUNT( {[Project].[Project Name].MEMBERS}, 16) ON ROWS FROM [Soft Developers DW] WHERE [Date].[Year].[2010]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetProfitProjectsSO")
.WithOpenApi();



// Endpoint que retorna el nombre y la ganancia de los clientes que han dejado mas de 50k de ganancia
app.MapGet("/ganancia_clientes_mayor_50000", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) MEMBER [Measures].[Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Payment Amount], 0) SELECT {[Measures].[Profit Margin Without Nulls], [Measures].[Payment Amount Without Nulls]} ON COLUMNS, FILTER( [Customer].[Company Name].MEMBERS, [Measures].[Profit Margin Without Nulls] > 50000) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetProfitClients")
.WithOpenApi();



// Endpoint que retorna la ganancia de los ultimos 4 anios [2020, 2021, 2022, 2023]
app.MapGet("/ganancia_ultimos_4_anios", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) SELECT {[Measures].[Profit Margin Without Nulls]} ON COLUMNS, Tail( FILTER( [Date].[Year].MEMBERS, [Date].[Year].CURRENTMEMBER.NAME <= '2023'), 4) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetFourYearsProfit")
.WithOpenApi();



// Endpoint que retorna los 10 proyectos con mayor ganancia
app.MapGet("/proyectos_mayor_ganancia", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) MEMBER [Measures].[Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Payment Amount], 0) SELECT {[Measures].[Profit Margin Without Nulls], [Measures].[Payment Amount Without Nulls]} ON COLUMNS, TOPCOUNT( {[Project].[Project Name].MEMBERS}, 11, [Measures].[Profit Margin Without Nulls]) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetProjectsWHigherProfit")
.WithOpenApi();



// Endpoint que retorna los 10 proyectos con menor ganancia
app.MapGet("/proyectos_menor_ganancia", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) MEMBER [Measures].[Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Payment Amount], 0) SELECT {[Measures].[Profit Margin Without Nulls], [Measures].[Payment Amount Without Nulls]} ON COLUMNS, BOTTOMCOUNT( FILTER( {[Project].[Project Name].MEMBERS}, NOT IsEmpty([Measures].[Profit Margin Without Nulls])), 11, [Measures].[Profit Margin Without Nulls]) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetProjectsWLessProfit")
.WithOpenApi();



// Endpoint que retorna la ganancia total de los proyectos que tienen un valor total mayor a 1M
app.MapGet("/ganancia_proyectosCostos_mayor_1000000", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) MEMBER [Measures].[Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Payment Amount], 0) SELECT {[Measures].[Profit Margin Without Nulls], [Measures].[Payment Amount Without Nulls]} ON COLUMNS, FILTER( [Project].[Project Name].MEMBERS, [Measures].[Payment Amount Without Nulls] > 1000000) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetProjectProfitWhTValue1000000")
.WithOpenApi();



// Endpoint que retorna los proyectos con un valor total superior a 1.5M
app.MapGet("/proyectosCostos_mayor_1500000", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "SELECT {[Measures].[Payment Amount]} ON COLUMNS, NON EMPTY FILTER( [Project].[Project Name].MEMBERS, [Measures].[Payment Amount] > 1500000) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetProjectTValue1500000")
.WithOpenApi();



// Endpoint que retorna el total de ganancia de los ultimos 10 proyectos entregados
app.MapGet("/ganancia_ultimos10_proyectos_entregados", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnection())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Profit Margin], 0) MEMBER [Measures].[Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Payment Amount], 0) SELECT {[Measures].[Profit Margin Without Nulls], [Measures].[Payment Amount Without Nulls]} ON COLUMNS, TOPCOUNT( ORDER( [Project].[Project Name].MEMBERS, [Project].[Project Deadline], DESC), 11) ON ROWS FROM [Soft Developers DW]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetLastProjectsProfit")
.WithOpenApi();



// * Endpoints que realizan consultas al modelo tabular de TechBridge * \\

// Endpoint de de prueba de la conexion al modelo tabular
app.MapGet("/tabulardatatest", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnectionTabular())
    {
        // Establecer la consulta al cubo
        string myquery = " SELECT NON EMPTY { [Measures].[Sum of profit margin] } ON COLUMNS, NON EMPTY { ([Dim_Project].[project name].[project name].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM [Model] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetTabularData")
.WithOpenApi();



// Endpoint que devuelve la ganancia promedio por proyecto
app.MapGet("/ganancia-promedio-proyectos", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnectionTabular())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Average of Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Average of profit margin], 0) SELECT NON EMPTY { [Measures].[Average of Profit Margin Without Nulls] } ON COLUMNS, NON EMPTY { FILTER( [Dim_Project].[project name].[project name].ALLMEMBERS, [Dim_Project].[project name].CURRENTMEMBER.MEMBER_CAPTION <> 'N/A') } ON ROWS FROM [Model]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetAverageProjectProfit")
.WithOpenApi();



// Endpoint que devuelve el total de dinero desembolsado y cantidad de pagos realizados por cliente
app.MapGet("/dinero-desembolsado-&pagos-cliente", () =>
{
    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnectionTabular())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Sum of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Sum of payment amount], 0) MEMBER [Measures].[Count of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Count of payment amount], 0) SELECT NON EMPTY { [Measures].[Sum of Payment Amount Without Nulls], [Measures].[Count of Payment Amount Without Nulls] } ON COLUMNS, NON EMPTY { [Dim_Customer].[company name].[company name].ALLMEMBERS } ON ROWS FROM [Model]";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetPaymentAmountPerClient")
.WithOpenApi();



// Endpoint que devuelve el total de ganancia y cantidad de pagos realizados en un anio especifico
app.MapGet("/ganancia-&pagos-anio", (int year) =>
{
    // Validar el anio recibido
    if (year < 2000 || year > DateTime.Now.Year)
    {
        return Results.BadRequest("El año proporcionado no es válido.");
    }

    // Establecer conexion con el cubo
    using (AdomdConnection connection = helper.GetConnectionTabular())
    {
        // Establecer la consulta al cubo
        string myquery = "WITH MEMBER [Measures].[Sum of Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Sum of profit margin], 0) MEMBER [Measures].[Count of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Count of payment amount], 0) SELECT NON EMPTY { [Measures].[Sum of Profit Margin Without Nulls], [Measures].[Count of Payment Amount Without Nulls] } ON COLUMNS, NON EMPTY { [Dim_Date].[Calendar Hierarchy].[Month Name].MEMBERS } ON ROWS FROM [Model] WHERE ([Dim_Date].[Year].&[" + year + "])";
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();

            // Transforma el resultado en JSON
            var jsonResult = helper.TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }
})
.WithName("GetPayment&profit")
.WithOpenApi();



// Ejecutar la API
app.Run();
