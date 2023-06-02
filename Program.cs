using System;
using Microsoft.Data.SqlClient;
using Dapper;
using BaltaDataAccess.Models;
using System.Data;

namespace BaltaDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            //String de conexão ao Banco de dados
            const string connectionString
            = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True"; //ADD Microsoft.Data.SqlClient (Nuget) 


            //Conexão ao Banco 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //ListCategory(connection);
                //DeleteCategory(connection);             
                //UpdateCategori(connection);
                //CreateCategori(connection);
                //ExecuteProcedure(connection);
                //ExecuteReadProcedure(connection);
                //OneToOne(connection);
                //OneToMany(connection);
                //QueryMultiple(connection);
                //SelectIn(connection);
                Transaction(connection); 
                //Like(connection, "back");

            }
        }
        static void ListCategory(SqlConnection connection)
        {
            var categorys = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
            foreach (var item in categorys)
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

            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Description,
                category.Order,
                category.Summary,
                category.Featured

            });
            System.Console.WriteLine($"{rows} linhas inseridas");
        }

        static void UpdateCategori(SqlConnection connection)
        {
            var updateQuery = "UPDATE [Category] SET [Title]=@title WHERE [Id] = @id";
            var rows = connection.Execute(updateQuery, new
            {
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "Frontend 2021"
            });

            System.Console.WriteLine($"{rows}Registros atualizado");
        }

        static void ExecuteProcedure(SqlConnection connection)
        {
            var procedure = "spDeleteStudent";
            var pars = new { StudentId = "17fdec87-ca36-42f6-85ff-0bb9a077e4c8" };
            var affetctedRows = connection.Execute(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);

            System.Console.WriteLine($"{affetctedRows} Linhas afetadas");
        }

        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "spGetCoursesByCategory";
            var pars = new { CategoryId = "af3407aa-11ae-4621-a2ef-2028b85507c4" };
            var courses = connection.Query(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);

            foreach (var item in courses)
            {
                System.Console.WriteLine(item.Title);
            }
        }

        static void OneToOne(SqlConnection connection)
        {
            var sql = @"SELECT * FROM [CareerItem] 
                            INNER JOIN [Course] 
                                ON [CareerItem].[CourseId] = [Course].[Id] ";

            var items = connection.Query<CareerItem, Course, CareerItem>(sql,
            (careerItem, course) =>
            {
                careerItem.Course = course;
                return careerItem;
            }, splitOn: "Id");
            foreach (var item in items)
            {
                System.Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
            }
        }

        static void OneToMany(SqlConnection connection)
        {
            var sql = @"SELECT 
                        [Career].[Id],
                        [Career].[Title],
                        [CareerItem].[CareerId],
                        [CareerItem].[Title]
                        FROM [Career] 
                            INNER JOIN [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id] 
                            Order BY
                                [Career].[Title] ";

            var careers = new List<Career>();
            var items = connection.Query<Career, CareerItem, Career>
            (sql,
            (career, item) =>
            {
                var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                if (car == null)
                {
                    car = career;
                    car.Items.Add(item);
                    careers.Add(car);
                }
                else
                {
                    career.Items.Add(item);
                }

                return career;
            }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                System.Console.WriteLine($"{career.Title} ");
                foreach (var item in career.Items)
                {
                    System.Console.WriteLine($" - {item.Title} ");
                }
            }
        }

        static void QueryMultiple(SqlConnection connection)
        {
            var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

            using (var multi = connection.QueryMultiple(query))
            {
                var categories = multi.Read<Category>();
                var courses = multi.Read<Course>();

                foreach (var item in categories)
                {
                    System.Console.WriteLine(item.Title);
                }

                foreach (var item in courses)
                {
                    System.Console.WriteLine(item.Title);
                }
            }
        }

        static void SelectIn(SqlConnection connection)
        {
            connection.Open();
            var query = @"SELECT TOP 10 * FROM Career 
                            WHERE [Id] 
                               IN @Id)";

            var items = connection.Query<Career>(query, new
            {
                Id = new[]
                 {
                                        "4327ac7e-963b-4893-9f31-9a3b28a4e72b2",
                                        "92d7e864-bea5-4812-80cc-c2f4e94db1af"
                                 }
            });
            foreach (var item in items)
            {
                System.Console.WriteLine(item.Title);
            }
        }

        static void Like(SqlConnection connection, string term)
        {
            connection.Open();
            var query = @"SELECT * FROM Course 
                            WHERE [Title] 
                                Like @exp)";

            var items = connection.Query<Course>(query, new
            {
                exp = $"%{term}%"
            });

            foreach (var item in items)
            {
                System.Console.WriteLine(item.Title);
            }
        }

        static void Transaction(SqlConnection connection)
        {
            connection.Open();
            //adicionando uma nova categoria
            var category = new Category();

            category.Id = Guid.NewGuid();
            category.Title = "nao quero salvar";
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

            using (var transaction = connection.BeginTransaction())
            {
                var rows = connection.Execute(insertSql, new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Description,
                    category.Order,
                    category.Summary,
                    category.Featured

                }, transaction);

                //transaction.Commit(); 
                transaction.Rollback();

                System.Console.WriteLine($"{rows} linhas inseridas");
            }

        }
    }
}
