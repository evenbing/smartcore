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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartCore.Infrastructure;
using SmartCore.Infrastructure.Json;
using SmartCore.Middleware;
using SmartCore.Middleware.MiddlewareExtension;
using SmartCore.Repository.Base;
using SmartCore.Repository.Base.Impl;
using SmartCore.Services;

namespace SmartCore.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        #region 构造函数 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion
        #region 定义依赖注入
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        //Log记录接口
        //private readonly ILoggerFactory _loggerFactory;
        #endregion
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. 这个方法为应用程序添加服务
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
                //options.Filters.Add(typeof(CustomExceptionAttribute));
                options.RespectBrowserAcceptHeader = true;
            }).AddNewtonsoftJson(options =>
            {
                //默认 JSON 格式化程序基于 System.Text.Json
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;                //忽略循环引用
                // Use the default property convert to lower
                options.SerializerSettings.ContractResolver = new ToLowerPropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            #region Authentication
            services.AddTokenAuthentication();
            #endregion
            //#region HttpClientFactory
            services.AddHttpClient();
            //#endregion
            services.AddHttpContextAccessor();

            //API版本控制DI
            //services.AddApiVersioning(option => {
            //    option.ReportApiVersions = true;
            //    option.AssumeDefaultVersionWhenUnspecified = true;
            //    option.DefaultApiVersion = new ApiVersion(1, 0);
            //});


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
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.配置HTTP请求管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory">日志工厂</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use((context, next) =>
            {
                //Do some work here System.Environment.MachineName
                context.Response.Headers.Add("X-SererName", Common.MachineNameWithHide);
                context.Response.Headers.Add("X-Correlation-ID", context?.TraceIdentifier);
                //Pass the request on down to the next pipeline (Which is the MVC middleware)
                return next();
            });
            //app.UseHttpsRedirection(); 
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            //});
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }
            else if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }
            app.UseHsts();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseErrorHandling();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            ServiceProviderInstance.Instance = app.ApplicationServices;
            NLog.LogManager.LoadConfiguration("nlog.config");
            //NLog.Web.NLogBuilder.ConfigureNLog("LogConfig/nlog.config");//读取Nlog配置文件 
        }
        /// <summary>
        ///Autofac IOC设置
        /// </summary>
        /// <param name="container"></param>
        /// <remarks>
        /// 1.依赖注入的目的是为了解耦。
        ///2.不依赖于具体类，而依赖抽象类或者接口，这叫依赖倒置。
        ///3.控制反转即IoC(Inversion of Control)，它把传统上由程序代码直接操控的对象的调用权交给容器，通过容器来实现对象组件的装配和管理。所谓的“控制反转”概念就是对组件对象控制权的转移，从程序代码本身转移到了外部容器。
        ///Autofac创建类的生命周期
        /// 1、InstancePerDependency 
        ///对每一个依赖或每一次调用创建一个新的唯一的实例。这也是默认的创建实例的方式。
        ///官方文档解释：Configure the component so that every dependent component or call to Resolve() gets a new, unique instance(default.)
        ///2、InstancePerLifetimeScope
        ///在一个生命周期域中，每一个依赖或调用创建一个单一的共享的实例，且每一个不同的生命周期域，实例是唯一的，不共享的。
        ///官方文档解释：Configure the component so that every dependent component or call to Resolve() within a single ILifetimeScope gets the same, shared instance.Dependent components in different lifetime scopes will get different instances.
        ///3、InstancePerMatchingLifetimeScope
        ///在一个做标识的生命周期域中，每一个依赖或调用创建一个单一的共享的实例。打了标识了的生命周期域中的子标识域中可以共享父级域中的实例。若在整个继承层次中没有找到打标识的生命周期域，则会抛出异常：DependencyResolutionException。
        ///官方文档解释：Configure the component so that every dependent component or call to Resolve() within a ILifetimeScope tagged with any of the provided tags value gets the same, shared instance. Dependent components in lifetime scopes that are children of the tagged scope will share the parent's instance. If no appropriately tagged scope can be found in the hierarchy an DependencyResolutionException is thrown. 
        ///4、InstancePerOwned 
        ///在一个生命周期域中所拥有的实例创建的生命周期中，每一个依赖组件或调用Resolve() 方法创建一个单一的共享的实例，并且子生命周期域共享父生命周期域中的实例。若在继承层级中没有发现合适的拥有子实例的生命周期域，则抛出异常：DependencyResolutionException。
        ///官方文档解释：Configure the component so that every dependent component or call to Resolve() within a ILifetimeScope created by an owned instance gets the same, shared instance. Dependent components in lifetime scopes that are children of the owned instance scope will share the parent's instance. If no appropriate owned instance scope can be found in the hierarchy an DependencyResolutionException is thrown. 
        ///5、SingleInstance
        ///每一次依赖组件或调用Resolve() 方法都会得到一个相同的共享的实例。其实就是单例模式。
        ///官方文档解释：Configure the component so that every dependent component or call to Resolve() gets the same, shared instance. 
        /// </remarks>
        public void ConfigureContainer(ContainerBuilder container)
        {
            Assembly assemblyRepository = Assembly.Load("SmartCore.Repository");
            Assembly assemblyServices = Assembly.Load("SmartCore.Services");
            //以接口方式进行注入,注入这些类的所有的公共接口作为服务 InstancePerRequest 每次请求共享同一个实例,例如：使用ef时，使不同的操作使用同一个数据上下文
            container.RegisterAssemblyTypes(assemblyRepository).AsImplementedInterfaces();
            container.RegisterAssemblyTypes(assemblyServices).AsImplementedInterfaces();
            //属性进行注册
            container.RegisterAssemblyTypes(typeof(Startup).Assembly).PropertiesAutowired();
            //RegisterGeneric 对泛型类进行注册 InstancePerDependency为每个依赖或者调用(Resolve())都创建一个新的对象,唯一的实例 InstancePerMatchingLifetimeScope 在一个做标识的生命周期域中，每一个依赖或调用创建一个单一的共享的实例。打了标识了的生命周期域中的子标识域中可以共享父级域中的实例
            container.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            //container.RegisterType<JwtServices>().As<IJwtServices>().AsImplementedInterfaces();
        }
    }
}
