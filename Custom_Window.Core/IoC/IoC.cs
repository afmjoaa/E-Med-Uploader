using Ninject;

namespace custom_window.Core
{
    public static class IoC
    {
        public static IKernel kernel { get; set; } = new StandardKernel();

        /// <summary>
        /// a shortcut to access the IUIManager
        /// </summary>
        public static IUIManager UI => IoC.Get<IUIManager>();

        public static void Setup()
        {
            //Bind all required viewModels
            BindViewModels();
        }

        private static void BindViewModels()
        {
            kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
            kernel.Bind<PatientInfoCheckViewModel>().ToConstant(new PatientInfoCheckViewModel());
        }

        /// <summary>
        /// gets the service from ioc of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return kernel.Get<T>();
        }

    }
}
