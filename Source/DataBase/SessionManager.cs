using System.Linq;
using System.Reflection;
using DataBase.Mapeamentos;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Caches.SysCache2;
using NHibernate.Cfg;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Engine;
using prjConfiguracao;
using StructureMap;
using StructureMap.Pipeline;

namespace DataBase
{
    /// <summary>
    /// Classe reponsavel por fazer a configuração do nhibernate
    /// </summary>
    public static class SessionManager
    {

        public static void ConfigureDataAccess()
        {
            //Chama o configurador do nhibernate utilizando o structuremap para fazer a injeção de dependencia
            ObjectFactory.Configure(i => ConfigureDataAccess(i, cBuscarConfiguracao.ObterConfiguracaoDoNHibernate()));
            //verifica se tudo foi configurado com sucesso
            ObjectFactory.AssertConfigurationIsValid();

        }

        private static void ConfigureDataAccess(ConfigurationExpression i, IPersistenceConfigurer databaseConfigurer)
        {
            //ValidatorEngine validatorEngine;
            //Configura o IoC para session factory do nhibernate ser singleton por toda a aplicação
            i.For<ISessionFactory>()
                .Singleton()
                .Use(ConfigureNHibernate(databaseConfigurer/*, out validatorEngine*/));

            //Configura o IoC para criar uma nova sessão a cada requisição
            i.For<ISession>()
                .LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.PerRequest))
                .Use(() =>
                     ObjectFactory.GetInstance<ISessionFactory>
                         ().OpenSession());

            //Configura o validador de entidades do nhibernate
            /*i.For<ValidatorEngine>()
                .Singleton()
                .Use(validatorEngine);*/
        }

        /// <summary>
        /// Metodo responsavel por executar o mapeamento das classes com o banco de dados
        /// </summary>
        /// <param name="databaseConfigurer"></param>
        /// <param name="validatorEngine"></param>
        /// <returns></returns>
        private static ISessionFactory ConfigureNHibernate(IPersistenceConfigurer databaseConfigurer/*,
                                                           out ValidatorEngine validatorEngine*/)
        {
            //ValidatorEngine ve = null;

            ISessionFactory factory = Fluently.Configure()
                .Database(databaseConfigurer)
                .Mappings(m =>
                          m.FluentMappings.AddFromAssemblyOf<AtivoMap>()
                              //.Conventions.Add(typeof (CascadeAll))
                )
                .Cache(x =>
                        x.UseQueryCache()
                        .UseSecondLevelCache()
                        .ProviderClass<SysCacheProvider>()
                    )
                .ExposeConfiguration(c =>
                                         {
                                             //ve = ConfigureValidator(c);
                                             c.SetProperty("adonet.batch_size", "5");
                                             c.SetProperty("generate_statistics", "false");
                                             //c.SetProperty("cache.use_second_level_cache", "true");
                                         })
                .BuildConfiguration().BuildSessionFactory();

            //validatorEngine = ve;
            return factory;
        }

        /// <summary>
        /// Configura o validados do nhibernate para validar as classes do dominio
        /// </summary>
        /// <param name="nHibernateConfiguration"></param>
        /// <returns></returns>
        private static ValidatorEngine ConfigureValidator(Configuration nHibernateConfiguration)
        {
            var configure = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();
            configure.Register(
                Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.Namespace != null && t.Namespace.Equals("BsBios.Portal.Domain.Entities"))
                    .ValidationDefinitions())
                .SetDefaultValidatorMode(ValidatorMode.UseAttribute)
                .IntegrateWithNHibernate.ApplyingDDLConstraints().And.RegisteringListeners();

            var ve = new ValidatorEngine();
            ve.Configure(configure);
            nHibernateConfiguration.Initialize(ve);
            return ve;
        }
    }
}