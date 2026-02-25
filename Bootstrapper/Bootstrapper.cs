using BetaComponents;
using Framework;
using Framework.Simulation;
using Framework.UserRightsServicePasswordOnly;
using FrameworkUI;
using Logger;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Dr7Hmi
{
    public class GetMachineService
    {
        public GetMachineService(Machine machine)
        {
            Machine = machine;
        }

        public Machine Machine { get; }
    }

    public class Bootstrapper : BaseBootstrapper<Machine>
    {
        public override void CreateView(Machine machine)
        {
            ViewModel = new AppMainViewModel(Machine);
            ViewModel.Initialize();

            ServiceLocator.RegisterService<IUIService>(ViewModel);
            ServiceLocator.RegisterService<IUIViewService>(ViewModel);
            ServiceLocator.RegisterService<GetMachineService>(new GetMachineService(machine));
        }

        public override IUserRightsService CreateUserRightService()
        {
            var rightService = new UserRightsService()
            {
                Levels = new List<Level>()
                    {
                        new Level("HUMARD", new byte[]{137,98,183,67,89,0,130,150,8,235,71,76,81,49,168,158,138,255,123,79,154,52,177,172,109,121,91,162,242,178,93,41,7,244,255,98,143,192,145,169,59,187,230,97,183,95,134,70,195,211,141,95,249,113,122,251,239,128,37,33,49,32,87,162 })
                        {
                            Features = new List<string>()
                            {
                                AppFeatures.Constructor,
                                AppFeatures.Maintenance,
                                AppFeatures.CanMinimizeHMI,
                                AppFeatures.CanCloseHMI,
                            }
                        },
                        new Level("Engineering", "ing")
                        {
                            Features = new List<string>()
                            {
                                AppFeatures.Maintenance,
                                AppFeatures.CanMinimizeHMI,
                                AppFeatures.CanCloseHMI,
                            }
                        },
                        new Level("Maintenance", "maint")
                        {
                            Features = new List<string>()
                            {
                                AppFeatures.Maintenance,
                            }
                        }
                    }
            };

#if DEBUG
            rightService.ForceLevel(rightService.Levels[0]);
#endif

            ServiceLocator.RegisterService<UserRightsService>(rightService);
            return rightService;
        }

        public override (IIOSystem system, bool ioSystemCreationFailed) InitializeIOsSystem(Machine machine, bool simulation)
        {
            IIOSystem ioSystem = null;
            bool ioSystemCreationFailed = false;

            try
            {
                //if (!simulation)
                //{
                //    ioSystem = new IOSystemMachine()
                //    {
                //        Route = "192.168.12.254.1.1",
                //    };
                //}
                //else
                {
                    ioSystem = new IOSystemSimulation();
                }

                ioSystem.Connect();
                ioSystem.Map(Machine);
            }
            catch (Exception e)
            {
                ioSystemCreationFailed = true;
                ServiceLocator.GetService<ILogService>().Debug(e, "IOSystem creation failed.");

                ioSystem = new IOSystemSimulation();
                ioSystem.Connect();
                ioSystem.Map(Machine);
            }
            finally
            {
                if (ioSystem != null && ioSystem is IOSystemSimulation)
                    InitializeIOsSimulation(Machine);

                ioSystem?.Start();
            }

            return (ioSystem, ioSystemCreationFailed);
        }
    }
}
