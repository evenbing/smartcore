using System;
using System.IO;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartCore.Infrastructure;
using SmartCore.Infrastructure.Json;
using SmartCore.Middleware;
using SmartCore.Middleware.MiddlewareExtension;
using SmartCore.Repository.Base;
using SmartCore.Repository.Base.Impl;
namespace SmartCore.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param> 
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(options =>
            {
                // requires using Microsoft.AspNetCore.Mvc.Formatters;
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                options.Filters.Add(typeof(ValidateModelAttribute));
                options.Filters.Add(typeof(WebApiResultMiddleware));
                options.Filters.Add(typeof(CustomExceptionAttribute));
                options.RespectBrowserAcceptHeader = true;
            }).AddNewtonsoftJson(options =>
            {
                //默认 JSON 格式化程序基于 System.Text.Json
                // Use the default property convert to lower
                options.SerializerSettings.ContractResolver = new ToLowerPropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            #region Authentication
            services.AddTokenAuthentication(Configuration);
            #endregion
            //#region HttpClientFactory
            services.AddHttpClient();
            //#endregion
            services.AddHttpContextAccessor();

            #region Configure Swagger
            services.AddSwaggerGen(c =>
            {
                //Swashbuckle.AspNetCore.Swagger.SwaggerOptions
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
                c.OrderActionsBy(a => a.RelativePath);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";//文件名来源于项目属性==》生成==》输出==》XML文档文件
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 启用xml注释. 该方法第二个参数启用控制器的注释，默认为false.
                c.IncludeXmlComments(xmlPath, true);
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    //JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//Jwt default param name
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //Add authentication type
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            });
            #endregion 
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                //Do some work here System.Environment.MachineName
                context.Response.Headers.Add("X-SererName",Common.MachineNameWithHide);
                context.Response.Headers.Add("X-TraceId", context?.TraceIdentifier);
                //Pass the request on down to the next pipeline (Which is the MVC middleware)
                return next();
            });
            //app.UseHttpsRedirection(); 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            else if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseErrorHandling();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        /// <summary>
        /// IOC设置
        /// </summary>
        /// <param name="container"></param>
        public void ConfigureContainer(ContainerBuilder container)
        {
            Assembly assemblyRepository = Assembly.Load("SmartCore.Repository");
            Assembly assemblyServices = Assembly.Load("SmartCore.Services");
            container.RegisterAssemblyTypes(assemblyRepository).AsImplementedInterfaces();//.Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();
            container.RegisterAssemblyTypes(assemblyServices).AsImplementedInterfaces();
            //属性注入
            container.RegisterAssemblyTypes(typeof(Startup).Assembly).PropertiesAutowired();
            container.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            //container.RegisterType<JwtServices>().As<IJwtServices>().AsImplementedInterfaces();
        }
    }
}
