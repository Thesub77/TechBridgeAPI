using Microsoft.AnalysisServices.AdomdClient; // Para conectar con SSAS y trabajar con CellSet
using ApiCubeTB.Utils;


public static class EndpointsTabular
{
    public static void MapEndpoints(WebApplication app)
    {
        // Instancia de la clase que contiene los helpers
        var helper = new MyHelperClass();

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
        .WithTags("Tabular Queries")
        .WithOpenApi();



        // Endpoint que devuelve los 10 proyectos con mayor ganancia con su total de pagos
        app.MapGet("/proyectos-&con-mayor-ganancia", () =>
        {
            // Establecer conexion con el cubo
            using (AdomdConnection connection = helper.GetConnectionTabular())
            {
                // Establecer la consulta al cubo
                string myquery = "WITH MEMBER [Measures].[Sum of Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Sum of profit margin], 0) MEMBER [Measures].[Count of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Count of payment amount], 0) MEMBER [Measures].[Sum of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Sum of payment amount], 0) SELECT NON EMPTY { [Measures].[Sum of payment amount Without Nulls], [Measures].[Sum of Profit Margin Without Nulls], [Measures].[Count of Payment Amount Without Nulls] } ON COLUMNS, NON EMPTY { FILTER( TOPCOUNT([Dim_Project].[Project Name].MEMBERS, 10, [Measures].[Sum of Profit Margin Without Nulls]), [Dim_Project].[Project Name].CURRENTMEMBER.NAME <> 'All')} ON ROWS FROM [Model]";
                using (AdomdCommand command = new AdomdCommand(myquery, connection))
                {
                    var result = command.ExecuteCellSet();

                    // Transforma el resultado en JSON
                    var jsonResult = helper.TransformToJSON(result);
                    return Results.Ok(jsonResult);
                }
            }
        })
        .WithName("GetMaxProfitProjects")
        .WithTags("Tabular Queries")
        .WithOpenApi();



        // Endpoint que devuelve el total de dinero ganado por trimestre
        app.MapGet("/dinero-ganado-&trimestre", () =>
        {
            // Establecer conexion con el cubo
            using (AdomdConnection connection = helper.GetConnectionTabular())
            {
                // Establecer la consulta al cubo
                string myquery = "WITH MEMBER [Measures].[Sum of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Sum of payment amount], 0) MEMBER [Measures].[Year] AS [Dim_Date].[Year].CURRENTMEMBER.NAME SELECT NON EMPTY { [Measures].[Sum of Payment Amount Without Nulls], [Measures].[Year]} ON COLUMNS, NON EMPTY { FILTER([Dim_Date].[Calendar Hierarchy].[Quarter].MEMBERS, [Dim_Date].[Year].CURRENTMEMBER.NAME >= '2005')} ON ROWS FROM [Model]";
                using (AdomdCommand command = new AdomdCommand(myquery, connection))
                {
                    var result = command.ExecuteCellSet();

                    // Transforma el resultado en JSON
                    var jsonResult = helper.TransformToJSON(result);
                    return Results.Ok(jsonResult);
                }
            }
        })
        .WithName("GetPaymentAmountByQuarter")
        .WithTags("Tabular Queries")
        .WithOpenApi();



        // Endpoint que devuelve el total de ganancia y cantidad de pagos realizados
        app.MapGet("/ganancia-&pagos-anio", () =>
        {
            // Establecer conexion con el cubo
            using (AdomdConnection connection = helper.GetConnectionTabular())
            {
                // Establecer la consulta al cubo
                string myquery = "WITH MEMBER [Measures].[Sum of Profit Margin Without Nulls] AS CoalesceEmpty([Measures].[Sum of profit margin], 0) MEMBER [Measures].[Count of Payment Amount Without Nulls] AS CoalesceEmpty([Measures].[Count of payment amount], 0) MEMBER [Measures].[Year] AS [Dim_Date].[Year].CURRENTMEMBER.NAME SELECT NON EMPTY { [Measures].[Year], [Measures].[Sum of Profit Margin Without Nulls], [Measures].[Count of Payment Amount Without Nulls] } ON COLUMNS, NON EMPTY { FILTER([Dim_Date].[Calendar Hierarchy].[Month Name].MEMBERS, [Dim_Date].[Month Name].CURRENTMEMBER.NAME <> 'N/A' AND [Dim_Date].[Year].CURRENTMEMBER.NAME >= '2005')} ON ROWS FROM [Model]";
                using (AdomdCommand command = new AdomdCommand(myquery, connection))
                {
                    var result = command.ExecuteCellSet();

                    // Transforma el resultado en JSON
                    var jsonResult = helper.TransformToJSON(result);
                    return Results.Ok(jsonResult);
                }
            }
        })
        .WithName("GetPaymentCount&profit")
        .WithTags("Tabular Queries")
        .WithOpenApi();
    }
}
