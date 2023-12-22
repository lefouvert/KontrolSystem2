﻿using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPScience;
using KontrolSystem.Parsing;
using KSP.Game.Science;
using KSP.Sim.Definitions;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleScienceExperiment")]
        public class ModuleScienceExperimentAdapter {
            private readonly PartComponent part;
            private readonly Data_ScienceExperiment dataScienceExperiment;

            public ModuleScienceExperimentAdapter(PartComponent part, Data_ScienceExperiment dataScienceExperiment) {
                this.part = part;
                this.dataScienceExperiment = dataScienceExperiment;
            }

            [KSField] public string PartName => part?.PartName ?? "Unknown";

            [KSField] public bool IsDeployed => dataScienceExperiment.PartIsDeployed;

            [KSField]
            public KSPScienceModule.ExperimentAdapter[] Experiments =>
                dataScienceExperiment.Experiments.SelectMany(experiment => {
                    ExperimentDefinition experimentDefinition = ModuleData.Game.ScienceManager.ScienceExperimentsDataStore.GetExperimentDefinition(experiment.ExperimentDefinitionID);
                    if (experimentDefinition != null) {
                        return new KSPScienceModule.ExperimentAdapter(experimentDefinition, experiment).Yield();
                    }

                    return Enumerable.Empty<KSPScienceModule.ExperimentAdapter>();
                }).ToArray();
        }
    }
}
