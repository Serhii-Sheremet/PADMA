using System;
using System.Collections.Generic;
using PADMA.Core.Models;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Interface describing a generic assembler for constructing calendar timelines (transits).
    /// </summary>
    public interface ITransitAssembler
    {
        /// <summary>
        /// Builds a complete timeline (list of days) within a given date range.
        /// </summary>
        /// <param name="fromUtc">Start of calculation range (UTC).</param>
        /// <param name="toUtc">End of calculation range (UTC).</param>
        /// <returns>List of DayTimeline objects, each containing multiple transit slices.</returns>
        List<DayTimeline> Build(DateTime fromUtc, DateTime toUtc);
    }
}
