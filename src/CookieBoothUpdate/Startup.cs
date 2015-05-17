using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Http;
using System.Data.SqlClient;
using System.Data;
using CookieBoothUpdate.Models;
using Serilog;


namespace CookieBoothUpdate
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public readonly ILogger _logger;

        public Startup()
        {
            //Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            //then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            Configuration = new Configuration()
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            _logger = new LoggerConfiguration()
                        .WriteTo.RollingFile("c:\\rollingfile.txt")  
                        .WriteTo.File("c:\\test.txt")  
                        .WriteTo.Console()
                        .CreateLogger();
            _logger.Information("Serilog created");
        }


        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.Information("Starting");

            /*
            // Add EF services to the services container.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<BoothDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            */

            services.AddMvc();
            services.AddSingleton<ICBURepository, CBURepository>();

            services.Configure<MvcOptions>(options =>
            {
#if PRODUCTION  // apply only for production
                _logger.LogInformation("Detecting and forcing SSL");

                // forces https when api is accessed
                // note that the home or root is still accessible non securely
                options.Filters.Add(new RequireHttpsAttribute());
#endif
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            _logger.Information("Configuring App");

            //app.UseWelcomePage();

            app.UseMvc();

            app.Run(async (context) =>
            {
                _logger.Information("Response out");
                await context.Response.WriteAsync("Hello World! 66");

                var parameters = new[]
                        {
                        new SqlParameter("@Id", 3),
                        new SqlParameter("@Name", "Pear")
                    };

                using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    _logger.Information("Inserting Pear using Query");
                    SqlHelper.ExecuteNonQuery(
                       conn,
                       CommandType.Text,
                       @" INSERT Products ([Id],[Name])
                            SELECT @Id, @Name"
                         ,
                       parameters);

                }


                var parameters2 = new[]
                        {
                        new SqlParameter("@Id", 4),
                        new SqlParameter("@Name", "Grape")
                    };

                using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    _logger.Information("Inserting Pear using StoredProcedure");
                    SqlHelper.ExecuteNonQuery(
                       conn,
                       CommandType.StoredProcedure,
                       @"InsertProduct",
                       parameters2);

                }


                var parameters3 = new[]
                        {
                        new SqlParameter("@Id", 4)
                    };

                using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    _logger.Information("Selecting all Pears");
                    SqlDataReader dr = SqlHelper.ExecuteReader(
                                           conn,
                                           CommandType.StoredProcedure,
                                           @"GetProductInfo",
                                           parameters3);

                    _logger.Information("Outputting all Pears");
                    while (dr.Read())
                    {
                        string name = dr["Name"].ToString();
                        await context.Response.WriteAsync("\nProduct : " + name);
                    }
                }


                _logger.Information("Done!");
                await context.Response.WriteAsync("\nDB done!");
            });

        }

    }
}
