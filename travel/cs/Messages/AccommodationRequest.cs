using System;

namespace AmqpAgents.Messaging.Examples
{
    public record AccommodationRequest(
        string TripId,
        string Location,
        DateTime CheckIn,
        DateTime CheckOut,
        int Guests,
        AccommodationPreferences Preferences
    );
}
