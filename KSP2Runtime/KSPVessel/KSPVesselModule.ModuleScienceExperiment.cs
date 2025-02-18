﻿using System.Linq;
using KontrolSystem.KSP.Runtime.KSPScience;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleScienceExperiment")]
    public class ModuleScienceExperimentAdapter {
        private readonly Data_ScienceExperiment dataScienceExperiment;
        private readonly PartComponent part;

        public ModuleScienceExperimentAdapter(PartComponent part, Data_ScienceExperiment dataScienceExperiment) {
            this.part = part;
            this.dataScienceExperiment = dataScienceExperiment;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField] public bool IsDeployed => dataScienceExperiment.PartIsDeployed;

        [KSField]
        public KSPScienceModule.ExperimentAdapter[] Experiments =>
            dataScienceExperiment.ExperimentStandings.Zip(dataScienceExperiment.Experiments,
                    (standing, config) =>
                        new KSPScienceModule.ExperimentAdapter(part.SimulationObject, standing, config))
                .ToArray();
    }
}
