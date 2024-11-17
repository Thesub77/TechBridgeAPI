using Microsoft.AnalysisServices.AdomdClient; // Para conectar con SSAS y trabajar con CellSet
using ApiCubeTB.Utils;


public static class EndpointsMultidimensional
{
    public static void MapEndpoints(WebApplication app)
    {
        // Instancia de la clase que contiene los helpers
        var helper = new MyHelperClass();

        //.WithTags("Multidimensional Queries"); // Agrupa en Swagger
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
        .WithOpenApi()
        .WithTags("Multidimensional Queries");



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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
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
        .WithTags("Multidimensional Queries")
        .WithOpenApi();

    }
}