using System;
using System.Collections.Generic;
using System.Linq;
using PADMA.Core.Models;

namespace PADMA.Core.Utilities
{
    public sealed class UserEventPlacement
    {
        public required UserEvent Event { get; init; }
        public int ColumnIndex { get; init; }
        public int ColumnCount { get; init; }
    }

    public static class UserEventOverlapHelper
    {
        public static List<UserEventPlacement> AssignColumnsForDay(List<UserEvent> events, DateTime dayLocal)
        {
            // Sort by start then end
            var sorted = events
                .Where(e => e.StartLocal != DateTime.MinValue && e.EndLocal != DateTime.MinValue)
                .OrderBy(e => e.StartLocal)
                .ThenBy(e => e.EndLocal)
                .ToList();

            // Build clusters by overlap
            var clusters = new List<List<UserEvent>>();
            List<UserEvent> current = new();
            DateTime currentMaxEnd = DateTime.MinValue;

            foreach (var ev in sorted)
            {
                if (current.Count == 0)
                {
                    current.Add(ev);
                    currentMaxEnd = ev.EndLocal;
                    continue;
                }

                if (ev.StartLocal < currentMaxEnd)
                {
                    current.Add(ev);
                    if (ev.EndLocal > currentMaxEnd)
                        currentMaxEnd = ev.EndLocal;
                }
                else
                {
                    clusters.Add(current);
                    current = new List<UserEvent> { ev };
                    currentMaxEnd = ev.EndLocal;
                }
            }
            if (current.Count > 0)
                clusters.Add(current);

            // For each cluster assign columns greedily
            var result = new List<UserEventPlacement>();

            foreach (var cluster in clusters)
            {
                // columnsEnd[c] = time when column c becomes free
                var columnsEnd = new List<DateTime>();
                var placements = new Dictionary<UserEvent, int>();

                foreach (var ev in cluster.OrderBy(e => e.StartLocal).ThenBy(e => e.EndLocal))
                {
                    int col = -1;
                    for (int i = 0; i < columnsEnd.Count; i++)
                    {
                        if (columnsEnd[i] <= ev.StartLocal)
                        {
                            col = i;
                            break;
                        }
                    }

                    if (col == -1)
                    {
                        col = columnsEnd.Count;
                        columnsEnd.Add(ev.EndLocal);
                    }
                    else
                    {
                        columnsEnd[col] = ev.EndLocal;
                    }

                    placements[ev] = col;
                }

                var colCount = columnsEnd.Count;

                foreach (var ev in cluster)
                {
                    result.Add(new UserEventPlacement
                    {
                        Event = ev,
                        ColumnIndex = placements[ev],
                        ColumnCount = colCount
                    });
                }
            }

            return result;
        }
    }
}
