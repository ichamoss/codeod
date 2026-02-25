using MECA500;

namespace Dr7Hmi
{
    public enum Meca500SimulationMode
    {
        None,
        //SimulationHwInTheLoop,
        Simulation,
        //Full
    }

    public abstract class Meca500Addressable : Meca500
    {
        public virtual int ControlPortId => 10000;
        public virtual int MonitoringPortId => 10001;
        public abstract string IpAddress { get; }

        public abstract Meca500SimulationMode SimulationMode { get; }

        protected override Meca500Behaviour CreateBehavior()
        {
            return new Meca500BehaviourSimulation();
        }

        public Meca500JogController JogController { get; protected set; }
    }
}
