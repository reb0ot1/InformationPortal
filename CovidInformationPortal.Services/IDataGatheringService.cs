using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CovidInformationPortal.Services
{
    public interface IDataGatheringService
    {
        Task StartGatheringData(DateTime searchedData);
    }
}
