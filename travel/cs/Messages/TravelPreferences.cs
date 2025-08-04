using System;

namespace AmqpAgents.Messaging.Examples
{
    public record TravelPreferences(
        string[] PreferredModes,
        string AccommodationType,
        bool FlexibleDates
    );
}
