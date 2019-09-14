using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace custom_window.Core
{
    public static class IoC
    {
        public static IKernel kernel { get; set; } = new StandardKernel();

        public static void Setup()
        {
            //Bind all required viewModels
            BindViewModels();
        }

        private static void BindViewModels()
        {
            kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
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
