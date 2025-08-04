using System;

namespace AmqpAgents.Messaging.Examples
{
    public record TravelRequest(
        string TripId,
        string Origin,
        string Destination,
        DateRange Dates,
        TravelPreferences Preferences,
        decimal Budget
    );
}
