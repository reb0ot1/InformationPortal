using AutoMapper;
using CovidInformationPortal.Models.Response;
using CovidInformationPortal.Models.VisualizationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Server.MapperProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<TootlTipModel, TootlTipApiModel>().ReverseMap();
            CreateMap<SeriesItemModel, SeriesItemApiModel>().ReverseMap();
            CreateMap<LineChartDataModel, LineChartApiModel>().ReverseMap();
        }
    }
}
