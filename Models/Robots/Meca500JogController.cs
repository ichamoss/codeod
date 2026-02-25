using Framework;
using MECA500;
using System;

namespace Dr7Hmi
{
    public enum IncrementLin
    {
        [DisplayName("0.01 mm")]
        Increment0_01,
        [DisplayName(" 0.1 mm")]
        Increment0_1,
        [DisplayName(" 1.0 mm")]
        Increment1,
        [DisplayName("10.0 mm")]
        Increment10
    }

    public enum IncrementDeg
    {
        [DisplayName("0.01 °")]
        Increment0_01,
        [DisplayName(" 0.1 °")]
        Increment0_1,
        [DisplayName(" 1.0 °")]
        Increment1,
        [DisplayName("10.0 °")]
        Increment10
    }

    public class Meca500JogController : Component
    {
        public Func<Meca500> RobotGetter
        {
            get
            {
                if (_robotGetter == null)
                    _robotGetter = () => this.FindAncestor<Meca500>();

                return _robotGetter;
            }
            set => _robotGetter = value;
        }
        private Func<Meca500> _robotGetter;

        protected Meca500 Robot => RobotGetter();

        [Tunable]
        [Persistent]
        public IncrementLin IncrementLin { get; protected set; }

        protected double IncrementLinValue
        {
            get
            {
                switch (IncrementLin)
                {
                    case IncrementLin.Increment0_01:
                        return 0.01;
                    case IncrementLin.Increment0_1:
                        return 0.1;
                    case IncrementLin.Increment1:
                        return 1.0;
                    case IncrementLin.Increment10:
                        return 10.0;
                    default:
                        return 0;
                }
            }
        }

        [Tunable]
        [Persistent]
        public IncrementDeg IncrementDeg { get; protected set; }

        protected double IncrementDegValue
        {
            get
            {
                switch (IncrementDeg)
                {
                    case IncrementDeg.Increment0_01:
                        return 0.01;
                    case IncrementDeg.Increment0_1:
                        return 0.1;
                    case IncrementDeg.Increment1:
                        return 1.0;
                    case IncrementDeg.Increment10:
                        return 10.0;
                    default:
                        return 0;
                }
            }
        }

        protected void ExecuteIfRobotActivated(Action action)
        {
            if (Robot.IsActivated)
            {
                action();
            }
            else
            {
                Api.ShowToast("Veuillez activer le robot pour pouvoir le déplacer");
            }
        }

        protected void MoveLinRelWrf(double dxInMm = 0, double dyInMm = 0, double dzInMm = 0, double drxInDeg = 0, double dryInDeg = 0, double drzInDeg = 0)
        {
            ExecuteIfRobotActivated(() =>
                Robot.MoveLinRelWrf(dxInMm, dyInMm, dzInMm, drxInDeg, dryInDeg, drzInDeg)
            );
        }

        [UserAction]
        public void StepXp() => MoveLinRelWrf(dxInMm: IncrementLinValue);

        [UserAction]
        public void StepXm() => MoveLinRelWrf(dxInMm: -IncrementLinValue);

        [UserAction]
        public void StepYp() => MoveLinRelWrf(dyInMm: IncrementLinValue);

        [UserAction]
        public void StepYm() => MoveLinRelWrf(dyInMm: -IncrementLinValue);

        [UserAction]
        public void StepZp() => MoveLinRelWrf(0, 0, IncrementLinValue, 0, 0, 0);

        [UserAction]
        public void StepZm() => MoveLinRelWrf(0, 0, -IncrementLinValue, 0, 0, 0);

        [UserAction]
        public void StepRXp() => MoveLinRelWrf(0, 0, 0, IncrementDegValue, 0, 0);

        [UserAction]
        public void StepRXm() => MoveLinRelWrf(0, 0, 0, -IncrementDegValue, 0, 0);

        [UserAction]
        public void StepRYp() => MoveLinRelWrf(0, 0, 0, 0, IncrementDegValue, 0);

        [UserAction]
        public void StepRYm() => MoveLinRelWrf(0, 0, 0, 0, -IncrementDegValue, 0);

        [UserAction]
        public void StepRZp() => MoveLinRelWrf(0, 0, 0, 0, 0, IncrementDegValue);

        [UserAction]
        public void StepRZm() => MoveLinRelWrf(0, 0, 0, 0, 0, -IncrementDegValue);


        protected void MoveJointsRel(double dj1, double dj2, double dj3, double dj4, double dj5, double dj6)
        {
            ExecuteIfRobotActivated(() =>
                Robot.MoveJointsRel(dj1, dj2, dj3, dj4, dj5, dj6)
            );
        }


        [UserAction]
        public void J1p() => MoveJointsRel(IncrementDegValue, 0, 0, 0, 0, 0);
        [UserAction]
        public void J1m() => MoveJointsRel(-IncrementDegValue, 0, 0, 0, 0, 0);
        [UserAction]
        public void J2p() => MoveJointsRel(0, IncrementDegValue, 0, 0, 0, 0);
        [UserAction]
        public void J2m() => MoveJointsRel(0, -IncrementDegValue, 0, 0, 0, 0);
        [UserAction]
        public void J3p() => MoveJointsRel(0, 0, IncrementDegValue, 0, 0, 0);
        [UserAction]
        public void J3m() => MoveJointsRel(0, 0, -IncrementDegValue, 0, 0, 0);
        [UserAction]
        public void J4p() => MoveJointsRel(0, 0, 0, IncrementDegValue, 0, 0);
        [UserAction]
        public void J4m() => MoveJointsRel(0, 0, 0, -IncrementDegValue, 0, 0);
        [UserAction]
        public void J5p() => MoveJointsRel(0, 0, 0, 0, IncrementDegValue, 0);
        [UserAction]
        public void J5m() => MoveJointsRel(0, 0, 0, 0, -IncrementDegValue, 0);
        [UserAction]
        public void J6p() => MoveJointsRel(0, 0, 0, 0, 0, IncrementDegValue);
        [UserAction]
        public void J6m() => MoveJointsRel(0, 0, 0, 0, 0, -IncrementDegValue);


        public double X => 0; //Robot.Xpos;
        public double Y => 0; //Robot.Ypos;
        public double Z => 0; //Robot.Zpos;
        public double RX => 0; // Robot.RXpos;
        public double RY => 0; // Robot.RYpos;
        public double RZ => 0; // Robot.RZpos;
    }
}
