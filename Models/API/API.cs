using Framework;
using SeiyaCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Woopsa;

namespace SeiyaRobot
{
    public static class WoopsaExtensions
    {
        public static WoopsaBoundClientObject ObjectByName(this WoopsaBoundClientObject obj, string name)
        {
            return obj.ByName(name) as WoopsaBoundClientObject;
        }

        public static WoopsaMethod MethodByName(this WoopsaBoundClientObject obj, string name)
        {
            return obj.ByName(name) as WoopsaMethod;
        }
    }

    public static class ServerAPI
    {
        public static string ComAddress
        {
            get
            {
                if (_comAddress == null)
                {
                    try
                    {
                        var ip = ConfigurationManager.AppSettings["ShelveIP"];
                        var port = int.Parse(ConfigurationManager.AppSettings["ShelvePort"]);

                        if (ip != null && ip != "")
                            _comAddress = $"http://{ip}:{port}/h4";
                        else
                            _comAddress = "http://127.0.0.1:81/h4";
                    }
                    catch
                    {
                        _comAddress = "http://127.0.0.1:81/h4";
                    }
                }

                return _comAddress;
            }
        }
        private static string _comAddress = null;

        public static void Execute(Action<WoopsaBoundClientObject> invocation)
        {
            using (new UnlockedSection())
            {
                using (WoopsaClient client = new WoopsaClient(ComAddress))
                {
                    WoopsaBoundClientObject root = client.CreateBoundRoot();
                    invocation(root);
                }
            }
        }
    }

    public static class MissionAPI
    {
        public static bool IsInSimulation { get; set; } = false;

        public static Mission GetNextMission()
        {
            if (IsInSimulation)
                return GetNextMissionSimulated();
            else
                return GetNextMissionFromServer();
        }

        private static Mission GetNextMissionSimulated()
        {
            var mission = new Mission();
            mission.Picks = new List<Pick>()
            {
                new Pick()
                {
                    BoxType = BoxType.L120,
                    Slot = new Slot()
                    {
                        Row = 2,
                        Position = SlotPosition.GH
                    }
                },
                new Pick()
                {
                    BoxType = BoxType.L250,
                    Slot = new Slot()
                    {
                        Row = 3,
                        Position = SlotPosition.J
                    }
                },
                new Pick()
                {
                    BoxType = BoxType.L280,
                    Slot = new Slot()
                    {
                        Row = 6,
                        Position = SlotPosition.G
                    }
                }
            };

            return mission;

        }

        private static Mission GetNextMissionFromServer()
        {
            string getNextMission = null;
            ServerAPI.Execute(root => getNextMission = root.ObjectByName("Mission").MethodByName("GetNextMission").Invoke().AsText);

            if (getNextMission != "")
                return JsonSerializer.Deserialize<Mission>(getNextMission);
            else
                return null;
        }

        public static void OpenShelve()
        {
            if (!IsInSimulation)
                ServerAPI.Execute(root => root.ObjectByName("Mission").MethodByName("OpenShelveForProcess").Invoke());
        }

        public static bool IsShelveOpen()
        {
            if (IsInSimulation)
                return true;
            else
            {
                bool result = false;
                ServerAPI.Execute(root => result = root.ObjectByName("Mission").MethodByName("IsShelveOpen").Invoke().ToBool());
                return result;
            }
        }

        public static void CloseShelve()
        {
            if (!IsInSimulation)
                ServerAPI.Execute(root => root.ObjectByName("Mission").MethodByName("CloseShelveForProcess").Invoke());
        }

        public static void NotifyPickDone(int serial)
        {
            if (!IsInSimulation)
                ServerAPI.Execute(root => root.ObjectByName("Mission").MethodByName("NotifyPickDone").Invoke(serial));
        }

        public static int ScanQR(bool scanTop, bool scanBottom) // return the serial
        {
            if (!IsInSimulation)
            {
                int scan = 0;
                ServerAPI.Execute(root => scan = root.ObjectByName("Mission").MethodByName("ScanQR").Invoke(scanTop, scanBottom).ToInt32());
                return scan;
            }
            else
                return 0;
        }
    }

    public static class SystemAPI
    {
        public static void NotifyRobotStatus(double batteryLevel, string status, int alarmId, int alarmSeverity, string alarmMessage, bool mustAckRobots)
        => ServerAPI.Execute(root => root.ObjectByName("System").MethodByName("NotifyRobotStatus").Invoke(batteryLevel, status, alarmId, alarmSeverity, alarmMessage, mustAckRobots));

        public static void UpdateDestinations(IEnumerable<string> destinations)
            => ServerAPI.Execute(root => root.ObjectByName("System").MethodByName("UpdateDestinations").Invoke(string.Join(";", destinations.Select(s => s.Replace(';', '_')))));

        public static void PingShelve()
                    => ServerAPI.Execute(root => root.ObjectByName("System").MethodByName("Ping").Invoke());
    }
}
