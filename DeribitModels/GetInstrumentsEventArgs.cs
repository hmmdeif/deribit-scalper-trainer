using System;
using System.Collections.Generic;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    public class GetInstrumentsEventArgs : EventArgs
    {
        public List<GetInstrumentResultsModel> Results { get; set; }

        public GetInstrumentsEventArgs(List<GetInstrumentResultsModel> results)
        {
            Results = results;
        }
    }
}
