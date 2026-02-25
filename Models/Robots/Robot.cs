using Framework;
using System.Linq;
using System;
using System.Windows.Controls;
using FrameworkUI;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using MECA500;

namespace Dr7Hmi
{
    public class Robot : Meca500Addressable
    {
        public override string IpAddress => throw new System.NotImplementedException();

        public override Meca500SimulationMode SimulationMode => Meca500SimulationMode.Simulation;


        [DefaultJointRx(0, 0, 0, 0, 0, 0)]
        public TeachJointRx PositionTransitionPalette { get; protected set; }

        [UserAction]
        public void SetTrfGripper1() => SetTrf(0, 36, 97, 0, 0, 180);

        [UserAction]
        public void SetFramePlate() => SetWrf(100, 100, 100, 0, 0, 180);

        PointRx test = new PointRx(0, 0, 0, 0, 0, 0);


        //[UserAction]
        //public void TestTransitions()
        //{
        //    ResetWrf();
        //    SetCartLinVel(200);
        //    SetTrfGripper1();
        //    MoveJ(PositionTransitionPalette);
        //    WaitEndMove();

        //    Api.Wait(Api.Seconds(0.5));

        //    MoveLin(0, 0, 50, 0.001, 0.001, 0);
        //    WaitEndMove();
        //    ResetWrf();
        //}

    }

    public static class Meca500Extensions
    {
        public static void MoveJ(this Meca500 robot, IEnumerable<JointRx> trajectory, bool reverse = false)
        {
            if (!reverse)
                foreach (var position in trajectory)
                    robot.MoveJ(position);
            else
                foreach (var position in trajectory.Reverse())
                    robot.MoveJ(position);
        }
    }
}
