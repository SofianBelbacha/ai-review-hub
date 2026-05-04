using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.Enums
{
    public enum AiAnalysisStatus
    {
        Pending = 0,  // en attente d'analyse
        Processing = 1,  // en cours d'analyse
        Completed = 2,  // analysé
        Failed = 3   // échec
    }
}
