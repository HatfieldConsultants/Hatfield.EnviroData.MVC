using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.WQDataProfile;
using Hatfield.EnviroData.QualityAssurance;

namespace Hatfield.EnviroData.MVC.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<Action, SampleListItemViewModel>()
                .ForMember(x => x.Id, l => l.MapFrom(m => m.ActionID))
                .ForMember(x => x.Name, l => l.MapFrom(m => m.BeginDateTime.ToString("MMM-dd-yyyy")));

            Mapper.CreateMap<Site, SiteViewModel>()
                .ForMember(x => x.Latitude, l => l.MapFrom(m => m.Latitude))
                .ForMember(x => x.Longitude, l => l.MapFrom(m => m.Longitude))
                .ForMember(x => x.SamplingFeatureID, l => l.MapFrom(m => m.SamplingFeatureID))
                .ForMember(x => x.SiteTypeCV, l => l.MapFrom(m => m.SiteTypeCV))
                .ForMember(x => x.SpatialReferenceID, l => l.MapFrom(m => m.SpatialReferenceID));

            Mapper.CreateMap<Action, ESDATModel>()
                .ForMember(x => x.DateReported, l => l.MapFrom(m => m.BeginDateTime))
                .ForMember(x => x.ProjectId, l => l.Ignore())
                .ForMember(x => x.LabName, l => l.Ignore())
                .ForMember(x => x.LabSignatory, l => l.Ignore())
                .ForMember(x => x.SDGID, l => l.Ignore())
                .ForMember(x => x.COCNumber, l => l.Ignore())
                .ForMember(x => x.LabRequestId, l => l.Ignore())
                .ForMember(x => x.LabRequestNumber, l => l.Ignore())
                .ForMember(x => x.LabRequestVersion, l => l.Ignore())
                .ForMember(x => x.AssociatedFiles, l => l.Ignore())
                .ForMember(x => x.CopiesSentTo, l => l.Ignore())
                .ForMember(x => x.SampleFileData, l => l.Ignore())
                .ForMember(x => x.ChemistryData, l => l.Ignore());
                //.ForMember(x => x.SampleFileData, l => l.ResolveUsing<SampleFileDataResolver>().ConstructedBy(() => new SampleFileDataResolver()).FromMember(m => m))
                //.ForMember(x => x.ChemistryData, l => l.ResolveUsing<ChemistryFileDataResolver>().ConstructedBy(() => new ChemistryFileDataResolver()).FromMember(m => m.RelatedActions));

            Mapper.CreateMap<Action, ESDATDataDisplayViewModel>()
                .ForMember(x => x.DateReported, l => l.MapFrom(m => m.BeginDateTime))
                .ForMember(x => x.ProjectId, l => l.Ignore())
                .ForMember(x => x.LabName, l => l.Ignore())
                .ForMember(x => x.LabSignatory, l => l.Ignore())
                .ForMember(x => x.SDGID, l => l.Ignore())
                .ForMember(x => x.COCNumber, l => l.Ignore())
                .ForMember(x => x.LabRequestId, l => l.Ignore())
                .ForMember(x => x.LabRequestNumber, l => l.Ignore())
                .ForMember(x => x.LabRequestVersion, l => l.Ignore())
                .ForMember(x => x.AssociatedFiles, l => l.Ignore())
                .ForMember(x => x.CopiesSentTo, l => l.Ignore())
                .ForMember(x => x.SampleFileData, l => l.Ignore())
                .ForMember(x => x.ChemistryData, l => l.Ignore())
                .ForMember(x => x.CurrentSampleDataVersion, l => l.Ignore());

            Mapper.CreateMap<Variable, VariableViewModel>()
                .ForMember(x => x.NoDataValue, l => l.MapFrom(m => m.NoDataValue))
                .ForMember(x => x.SpeciationCV, l => l.MapFrom(m => m.SpeciationCV))
                .ForMember(x => x.VariableCode, l => l.MapFrom(m => m.VariableCode))
                .ForMember(x => x.VariableDefinition, l => l.MapFrom(m => m.VariableDefinition))
                .ForMember(x => x.VariableID, l => l.MapFrom(m => m.VariableID))
                .ForMember(x => x.VariableNameCV, l => l.MapFrom(m => m.VariableNameCV))
                .ForMember(x => x.VariableTypeCV, l => l.MapFrom(m => m.VariableTypeCV));

            //Mapper.CreateMap<Result, StationAnalyteQueryViewModel>();
            //Mapper.CreateMap<MeasurementResultValue, StationAnalyteQueryViewModel>();
            //Mapper.CreateMap<Unit, StationAnalyteQueryViewModel>();


            Mapper.CreateMap<IWQDefaultValueProvider, AdminDefaultValuesDataViewModel>();
            Mapper.CreateMap<AdminDefaultValuesDataViewModel, WQDefaultValueModel>();

            Mapper.CreateMap<IQualityCheckingResult, ResultMessageViewModel>()
                .ForMember(x => x.Level, l => l.MapFrom(m => m.Level.ToString()))
                .ForMember(x => x.Message, l => l.MapFrom(m => m.Message));

        }
    }
}