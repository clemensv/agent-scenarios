using System;

namespace AmqpAgents.Messaging.Examples
{
    public record TrainSearchRequest(
        string TripId,
        string Origin,
        string Destination,
        DateTime DepartDate,
        DateTime? ReturnDate,
        int Passengers
    );
}
