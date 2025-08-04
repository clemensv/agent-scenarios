using System;

namespace AmqpAgents.Messaging.Examples
{
    public record RoadTravelRequest(
        string TripId,
        string Origin,
        string Destination,
        DateTime DepartDate,
        DateTime? ReturnDate,
        RoadTravelType TravelType
    );
}
