using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.MVC.Models;

namespace Hatfield.EnviroData.MVC.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<Action, SampleListItemViewModel>()
                .ForMember(x => x.Id, l => l.MapFrom(m => m.ActionID))
                .ForMember(x => x.Name, l => l.MapFrom(m => m.BeginDateTime.ToString("mm-dd-yyyy")));

            Mapper.CreateMap<Action, ESDATModel>()
                .ForMember(x => x.DateReported, l => l.Ignore())
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
        }
    }
}