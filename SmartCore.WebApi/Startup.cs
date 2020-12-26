using System;
using System.IO;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Authorization;
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
using SmartCore.Middleware.Providers;
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
        #region ���캯�� 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion
        #region ��������ע��
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        //Log��¼�ӿ�
        //private readonly ILoggerFactory _loggerFactory;
        #endregion
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. �������ΪӦ�ó�����ӷ���
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
                //Ĭ�� JSON ��ʽ��������� System.Text.Json
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;                //����ѭ������
                // Use the default property convert to lower
                options.SerializerSettings.ContractResolver = new ToLowerPropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            #region Authentication
            services.AddTokenAuthentication();
            #endregion
            //#region HttpClientFactory
            services.AddHttpClient();
            //#endregion
            services.AddHttpContextAccessor();

            //API�汾����DI
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
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";//�ļ�����Դ����Ŀ����==������==�����==��XML�ĵ��ļ�
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ����xmlע��. �÷����ڶ����������ÿ�������ע�ͣ�Ĭ��Ϊfalse.
                c.IncludeXmlComments(xmlPath, true);
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    //JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�
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
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.����HTTP����ܵ�
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory">��־����</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use((context, next) =>
            {
                //Do some work here System.Environment.MachineName
                context.Response.Headers.Add("X-ServerName", Common.MachineNameWithHide);
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
            //NLog.Web.NLogBuilder.ConfigureNLog("LogConfig/nlog.config");//��ȡNlog�����ļ� 
        }
        /// <summary>
        ///Autofac IOC����
        /// </summary>
        /// <param name="container"></param>
        /// <remarks>
        /// 1.����ע���Ŀ����Ϊ�˽��
        ///2.�������ھ����࣬��������������߽ӿڣ�����������á�
        ///3.���Ʒ�ת��IoC(Inversion of Control)�����Ѵ�ͳ���ɳ������ֱ�ӲٿصĶ���ĵ���Ȩ����������ͨ��������ʵ�ֶ��������װ��͹�����ν�ġ����Ʒ�ת��������Ƕ�����������Ȩ��ת�ƣ��ӳ�����뱾��ת�Ƶ����ⲿ������
        ///Autofac���������������
        /// 1��InstancePerDependency 
        ///��ÿһ��������ÿһ�ε��ô���һ���µ�Ψһ��ʵ������Ҳ��Ĭ�ϵĴ���ʵ���ķ�ʽ��
        ///�ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() gets a new, unique instance(default.)
        ///2��InstancePerLifetimeScope
        ///��һ�������������У�ÿһ����������ô���һ����һ�Ĺ����ʵ������ÿһ����ͬ������������ʵ����Ψһ�ģ�������ġ�
        ///�ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() within a single ILifetimeScope gets the same, shared instance.Dependent components in different lifetime scopes will get different instances.
        ///3��InstancePerMatchingLifetimeScope
        ///��һ������ʶ�������������У�ÿһ����������ô���һ����һ�Ĺ����ʵ�������˱�ʶ�˵������������е��ӱ�ʶ���п��Թ��������е�ʵ�������������̳в����û���ҵ����ʶ����������������׳��쳣��DependencyResolutionException��
        ///�ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() within a ILifetimeScope tagged with any of the provided tags value gets the same, shared instance. Dependent components in lifetime scopes that are children of the tagged scope will share the parent's instance. If no appropriately tagged scope can be found in the hierarchy an DependencyResolutionException is thrown. 
        ///4��InstancePerOwned 
        ///��һ����������������ӵ�е�ʵ�����������������У�ÿһ��������������Resolve() ��������һ����һ�Ĺ����ʵ���������������������������������е�ʵ�������ڼ̳в㼶��û�з��ֺ��ʵ�ӵ����ʵ�����������������׳��쳣��DependencyResolutionException��
        ///�ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() within a ILifetimeScope created by an owned instance gets the same, shared instance. Dependent components in lifetime scopes that are children of the owned instance scope will share the parent's instance. If no appropriate owned instance scope can be found in the hierarchy an DependencyResolutionException is thrown. 
        ///5��SingleInstance
        ///ÿһ��������������Resolve() ��������õ�һ����ͬ�Ĺ����ʵ������ʵ���ǵ���ģʽ��
        ///�ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() gets the same, shared instance. 
        /// </remarks>
        public void ConfigureContainer(ContainerBuilder container)
        {
            Assembly assemblyRepository = Assembly.Load("SmartCore.Repository");
            Assembly assemblyServices = Assembly.Load("SmartCore.Services");
            //�Խӿڷ�ʽ����ע��,ע����Щ������еĹ����ӿ���Ϊ���� InstancePerRequest ÿ��������ͬһ��ʵ��,���磺ʹ��efʱ��ʹ��ͬ�Ĳ���ʹ��ͬһ������������
            container.RegisterAssemblyTypes(assemblyRepository).AsImplementedInterfaces();
            container.RegisterAssemblyTypes(assemblyServices).AsImplementedInterfaces();
            //���Խ���ע��
            container.RegisterAssemblyTypes(typeof(Startup).Assembly).PropertiesAutowired();
            //RegisterGeneric �Է��������ע�� InstancePerDependencyΪÿ���������ߵ���(Resolve())������һ���µĶ���,Ψһ��ʵ�� InstancePerMatchingLifetimeScope ��һ������ʶ�������������У�ÿһ����������ô���һ����һ�Ĺ����ʵ�������˱�ʶ�˵������������е��ӱ�ʶ���п��Թ��������е�ʵ��
            container.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            //container.RegisterType<JwtServices>().As<IJwtServices>().AsImplementedInterfaces();
        }
         
        /// <summary>
        /// ������ ��Ϊ������һ�� ���� �ڱ�ĵط�ʹ��
        /// </summary>
        public ILifetimeScope AutofacContainer { get; private set; }
    }
}
