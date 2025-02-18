﻿using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleGimbal")]
    public class ModuleGimbalAdapter : BaseGimbalAdapter {
        private readonly PartComponent part;

        public ModuleGimbalAdapter(PartComponent part, Data_Gimbal dataGimbal) : base(dataGimbal) {
            this.part = part;
        }

        [KSField]
        public Vector3d PitchYawRoll {
            get {
                if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return Vector3d.zero;
                if (!viewObject.TryGetComponent<Module_Gimbal>(out var moduleGimbal)) return Vector3d.zero;
                return ((Vector3d)moduleGimbal.GimbalActuation).SwapYAndZ;
            }
            set {
                if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                       out var viewObject)) return;
                if (!viewObject.TryGetComponent<Module_Gimbal>(out var moduleGimbal)) return;
                moduleGimbal.SetGimbalActuation((float)value.x, (float)value.z, (float)value.y);
            }
        }
    }
}
