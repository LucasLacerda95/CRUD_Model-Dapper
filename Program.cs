using System;
using Microsoft.Data.SqlClient;
using Dapper;
using BaltaDataAccess.Models;
using System.Data;
using System.Collections.Generic;


//Dapper

namespace BaltaDataAccess
{
    internal class Program
    {
        static void Main(string[] args)


        {
            const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=zulu_2020#;Encrypt=false";
               
            using(var connection = new SqlConnection(connectionString)){
                
                //CreateCategory(connection);
                //CreateManyCategories(connection);
                //ListCategories(connection);
                //UpdateCategory(connection);
                //DeleteCategory(connection);
                //ExecuteProcedure(connection);
                //ExecuteReadProcedure(connection);
                //ExecuteScalar(connection);
                //ReadView(connection);
                //OneToOne(connection);
                //OneToMany(connection);
                //QueryMutiple(connection);
                //SelectIn(connection);
                //Like(connection, "api");
                Transaction(connection);

                //Evitar colocar muita coisa com a conexão aberta para não sobrecarregar a conexão  
            }
        }
   
        static void ListCategories(SqlConnection connection){
            
             var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");/* Query executa uma 
                query diretamente no banco de dados e retorna os dados e seus respectivos tipos */

                foreach(var item in categories){//Varre a lista retornada e exibe no console
                    System.Console.WriteLine($"{item.Id} - {item.Title}");
                }

        }
    
        static void CreateCategory(SqlConnection connection){

            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada aos serviços AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

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

            //A criação de parâmetros evita o SQL Injection que é um ataque que essa query pode sofrer.
                var rows = connection.Execute(insertSql, new {//Execute retorna somente um int com a quantidade de linhas alteradas
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured,
                });
                System.Console.WriteLine($"{rows} - Linhas inseridas.");
        }
   
        static void CreateManyCategories(SqlConnection connection){
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Categoria destinada aos serviços AWS";
            category.Featured = false;

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria Nova";
            category2.Url = "Categoria Nova";
            category2.Summary = "Categoria";
            category2.Order = 9;
            category2.Description = "Categoria Nova";
            category2.Featured = true;

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

            //A criação de parâmetros evita o SQL Injection que é um ataque que essa query pode sofrer.
                var rows = connection.Execute(insertSql, new[] {//Execute retorna somente um int com a quantidade de linhas alteradas
                    new{
                        category.Id,
                        category.Title,
                        category.Url,
                        category.Summary,
                        category.Order,
                        category.Description,
                        category.Featured
                    },
                    new{
                        category2.Id,
                        category2.Title,
                        category2.Url,
                        category2.Summary,
                        category2.Order,
                        category2.Description,
                        category2.Featured  
                    }
                });
                System.Console.WriteLine($"{rows} - Linhas inserida(s)");
        }

        static void UpdateCategory(SqlConnection connection){

            var updateQuery = "UPDATE [Category] SET [Title] = @title WHERE [Id]=@id";

            var rows = connection.Execute(updateQuery, new{
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "FronteEnd 2021"
            });

            System.Console.WriteLine($"{rows} - registros atualizado(s)");
        }

        static void DeleteCategory(SqlConnection connection){

            var deleteQuery = "DELETE FROM [Category] WHERE [id] = @id";

            var rows = connection.Execute(deleteQuery, new{
               id = new Guid("7faa3a73-ddcd-45b8-9d89-8a186711e8be") 
            });

            System.Console.WriteLine($"{rows} - registros deletado(s)");
        }
    
        static void ExecuteProcedure(SqlConnection connection){
            var procedure = "spDeleteStudent";
            var pars = new { StudentId = "30698791-3453-4e2a-b52c-d51da2d6a5a8"};

            var affectedRows = connection.Execute(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);

            System.Console.WriteLine($"{affectedRows} - linhas afetada(s)");
        }
    
        static void ExecuteReadProcedure(SqlConnection connection){
            var procedure = "[spGetCoursesByCategory]";
            var pars = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };
            var courses = connection.Query(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);
            
            foreach(var item in courses){
                System.Console.WriteLine(item.Title);
            }
        }

        static void ExecuteScalar(SqlConnection connection){
            
            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada aos serviços AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = @"
                INSERT INTO 
                    [Category]
                OUTPUT inserted.[Id] 
                VALUES (
                    NEWID(),
                    @Title,
                    @Url,
                    @Summary,
                    @Order,
                    @Description,
                    @Featured)";

                var id = connection.ExecuteScalar<Guid>(insertSql, new {
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured,
                });
                System.Console.WriteLine($"A categoria inserida foi: {id}");
        }
    
        static void ReadView(SqlConnection connection){
            var sql = "SELECT * FROM [vwCourses]";

            var courses = connection.Query(sql);

                foreach(var item in courses){
                    System.Console.WriteLine($"{item.Id} - {item.Title}");
                }
        }
    
        static void OneToOne(SqlConnection connection){
            var sql = @"SELECT
                * 
            FROM 
                [CareerItem] 
            INNER JOIN 
            [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

            var items = connection.Query<CareerItem, Course, CareerItem>(
                sql,
                (carrerItem, course)=>{
                    carrerItem.Course = course;
                    return carrerItem;  
                }, splitOn: "Id");

            foreach( var item in items){

                System.Console.WriteLine($"{item.Title} - Curso: {item.Course?.Title}");
            }
        }
   
        static void OneToMany(SqlConnection connection){
            var sql = @"
                SELECT
                    [Career].[Id],
                    [Career].[Title],
                    [CareerItem].[CareerId],
                    [CareerItem].[Title]
                FROM
                    [Career]
                INNER JOIN
                    [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
                ORDER BY
                    [Career].[Title]";

            var careers = new List<Career>();
            var items = connection.Query<Career, CareerItem, Career>(
                sql,
                (career, item)=>{
                    var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                    if(car == null){
                        car = career;
                        car.Items.Add(item);
                        careers.Add(car);
                    }else{
                        car.Items.Add(item);
                    }

                    return career;  
                }, splitOn: "CareerId");

            foreach( var career in careers){

                System.Console.WriteLine($"{career.Title}");

                foreach( var item in career.Items){

                System.Console.WriteLine($" - {item.Title}");
            }
            }
        }
    
        static void QueryMutiple(SqlConnection connection){
            var query = "SELECT * FROM [Category]; SELECT * FROM [Course];";

            using(var multi = connection.QueryMultiple(query)){
                var categories = multi.Read<Category>();
                var courses = multi.Read<Course>();

                foreach(var item in categories){
                    System.Console.WriteLine(item.Title);
                }

                
                foreach(var item in courses){
                    System.Console.WriteLine(item.Title);
                }
            }
        }

        static void SelectIn(SqlConnection connection){
            var query = @"
            SELECT * FROM 
                Career 
            WHERE [Id] 
            IN @Id";   

            var items = connection.Query<Career>(query, new{
                Id = new[]{
                    "01ae8a85-b4e8-4194-a0f1-1c6190af54cb",
                    "92d7e864-bea5-4812-80cc-c2f4e94db1af"
                }
            });

            foreach(var item in items){
                System.Console.WriteLine(item.Title);
            }
        }
  
        static void Like(SqlConnection connection, string term){
        
            var query = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";   

            var items = connection.Query<Course>(query, new{
                exp = $"%{term}%"
            });

            foreach(var item in items){
                System.Console.WriteLine(item.Title);
            }
        }
   
        static void Transaction(SqlConnection connection){
            
             var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Minha categoria que não";
            category.Url = "amazon";
            category.Description = "Categoria destinada aos serviços AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

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
            connection.Open();
            using(var transaction = connection.BeginTransaction()){
                    
                    var rows = connection.Execute(insertSql, new {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured,
                }, transaction);
                
            transaction.Commit();
            //transaction.Rollback();
            System.Console.WriteLine($"{rows} - Linhas inseridas.");

            }
            
        }
    }

}
