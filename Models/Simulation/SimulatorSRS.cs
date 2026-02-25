using BetaComponents;
using BetaComponents.RobotStaubli;
using Framework;
using MECA500;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dr7Hmi
{
    public class SimulatorSRS : StaubliCyclicMonitor
    {
        #region Connection

        protected override bool IsFullySimulated
        {
            get
            {
                if (_isFullySimulated == null)
                {
                    try
                    {
                        _isFullySimulated = !bool.Parse(ConfigurationManager.AppSettings["EnableSRSSimulator"]);
                    }
                    catch
                    {
                        _isFullySimulated = true;
                    }
                }

                return _isFullySimulated.Value;
            }
        }
        private bool? _isFullySimulated;

        protected override TcpClient CreateClient()
        {
            return new TcpClient("127.0.0.1", 2001);
        }

        #endregion

        protected override IEnumerable<byte> GetProcessData()
        {
            var data = new List<byte>();

            var machine = this.FindAncestor<Machine>();

            SimulationHelpers.AddMeca500Joints(data, machine.Robot);


            SimulationHelpers.AddFrame500(data, () => (machine.Robot.Behaviour as Meca500BehaviourBaseSimulation).ActualWrf); //aioMeca500Wrf
            SimulationHelpers.AddFrame500(data, () => (machine.Robot.Behaviour as Meca500BehaviourBaseSimulation).ActualTrf); //aioMeca500Trf

            //AddMcs500Joints(data, machine.RobotLoader);
            //AddFrame(data, () => machine.RobotLoader.ActualWrf); //aioMcs500Wrf
            //AddFrame(data, () => machine.RobotLoader.ActualTrf); //aioMcs500Trf

            //data.AddRange(SimulationHelpers.Encode3Bytes(machine.RobotStore.J0, 0, 300));

            //SimulationHelpers.AddMeca500Joints(data, machine.RobotStore);

            //SimulationHelpers.AddFrame500(data, () => (machine.RobotStore.Behaviour as Meca500BehaviourBaseSimulation).ActualWrf); //aioMeca500Wrf
            //SimulationHelpers.AddFrame500(data, () => (machine.RobotStore.Behaviour as Meca500BehaviourBaseSimulation).ActualTrf); //aioMeca500Trf

            return data;
        }
    }

    public static class SimulationHelpers
    {

        public static IEnumerable<byte> Encode2Bytes(double value, double min, double max)
        {
            uint raw = (uint)((value - min) * 65535.0 / (max - min));

            yield return (byte)((raw >> 8) & 0xFF);
            yield return (byte)(raw & 0xFF);
        }

        public static IEnumerable<byte> Encode3Bytes(double value, double min, double max)
        {
            uint raw = (uint)((value - min) * 16777215.0 / (max - min));

            yield return (byte)((raw >> 16) & 0xFF);
            yield return (byte)((raw >> 8) & 0xFF);
            yield return (byte)(raw & 0xFF);
        }


        //private void AddMcs500Joints(List<byte> data, Mcs500 robot)
        //{
        //    data.AddRange(Encode3Bytes(robot.J1, -145, 145));
        //    data.AddRange(Encode3Bytes(robot.J2, -145, 145));
        //    data.AddRange(Encode3Bytes(robot.J3, -102, 0));
        //    data.AddRange(Encode3Bytes((robot.J4 + 36000.0) % 360, 0, 360));
        //}

        public static void AddMeca500Joints(List<byte> data, Robot robot)
        {
            data.AddRange(Encode3Bytes(robot.TargetJoints.J1, -175, 175));
            data.AddRange(Encode3Bytes(robot.TargetJoints.J2, -70, 90));
            data.AddRange(Encode3Bytes(robot.TargetJoints.J3, -135, 70));
            data.AddRange(Encode3Bytes(robot.TargetJoints.J4, -170, 170));
            data.AddRange(Encode3Bytes(robot.TargetJoints.J5, -115, 115));
            data.AddRange(Encode3Bytes((robot.TargetJoints.J6 + 36000.0) % 360, 0, 360));
        }

        public static void AddFrame500(List<byte> data, Func<Matrix3D> frameGetter)
        {
            var frame = frameGetter();
            var angles = frame.DecodeRxRyRz();

            void AddAngle(double angleInRad)
            {
                var angle = (Matrix3DHelpers.RadToDeg(angleInRad) + 360.0) % 360;

                if (angle > 180.0)
                    angle -= 360.0;

                data.AddRange(Encode2Bytes(angle, -180, 180));
            }

            data.AddRange(Encode2Bytes(frame.GetX(), -500, 500));
            data.AddRange(Encode2Bytes(frame.GetY(), -500, 500));
            data.AddRange(Encode2Bytes(frame.GetZ(), -500, 500));
            AddAngle(angles.rx);
            AddAngle(angles.ry);
            AddAngle(angles.rz);
        }
    }
}
