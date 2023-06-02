using System;
using Microsoft.Data.SqlClient;
using Dapper;
using BaltaDataAccess.Models;
using System.Data;

namespace BaltaDataAccess
{
    class Program
    {
        static void Main(string[]args)
        {
            //String de conexão ao Banco de dados
            const string connectionString
            = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True"; //ADD Microsoft.Data.SqlClient (Nuget) 
           

            //Conexão ao Banco 
            using(SqlConnection connection = new SqlConnection(connectionString))
            {     
                //ListCategory(connection);
                //DeleteCategory(connection);             
                //UpdateCategori(connection);
                //CreateCategori(connection);
                //ExecuteProcedure(connection);
                //ExecuteReadProcedure(connection);
           
            }
        }
        static void ListCategory(SqlConnection connection)
        {
              var categorys = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
                foreach(var item in categorys)
                {
                    System.Console.WriteLine($"{item.Id} - {item.Title}");
                }
        }
   
        static void CreateCategori(SqlConnection connection)
        {
             //adicionando uma nova categoria
            var category = new Category();

            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "Amazon";
            category.Description = "Categoria destinada a serviço do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            //inserindo a nova categoria usando parametros
             var insertSql = @"INSERT INTO
                    [Category]
                VALUES (
                    @Id,
                    @Title,
                    @Url,
                    @Summary,
                    @Order,
                    @Description,
                    @Featured)";

            var rows= connection.Execute(insertSql, new{
                category.Id ,
                category.Title ,
                category.Url ,
                category.Description ,
                category.Order ,
                category.Summary ,
                category.Featured 

                });
                System.Console.WriteLine($"{rows} linhas inseridas");
        }
    
        static void UpdateCategori(SqlConnection connection)
        {
            var updateQuery = "UPDATE [Category] SET [Title]=@title WHERE [Id] = @id";
            var rows = connection.Execute(updateQuery, new
            {
                id= new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "Frontend 2021"
            });

            System.Console.WriteLine($"{rows}Registros atualizado");
        }

    static void ExecuteProcedure(SqlConnection connection)
    {
        var procedure ="spDeleteStudent";
        var pars = new {StudentId = "17fdec87-ca36-42f6-85ff-0bb9a077e4c8" };
        var affetctedRows = connection.Execute(
            procedure,
            pars, 
            commandType: CommandType.StoredProcedure);

        System.Console.WriteLine($"{affetctedRows} Linhas afetadas");
    }    

        static void ExecuteReadProcedure(SqlConnection connection)
    {
        var procedure ="spGetCoursesByCategory";
        var pars = new {CategoryId = "af3407aa-11ae-4621-a2ef-2028b85507c4" };
        var courses = connection.Query(
            procedure,
            pars, 
            commandType: CommandType.StoredProcedure);

        foreach(var item in courses)
        {
            System.Console.WriteLine(item.Title);
        }
    }  

    }
}
