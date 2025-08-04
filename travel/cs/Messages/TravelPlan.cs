using System;

namespace AmqpAgents.Messaging.Examples
{
    public record TravelPlan(
        string TripId,
        TravelOption[] SelectedOptions,
        decimal TotalCost,
        string[] Recommendations
    );
}
