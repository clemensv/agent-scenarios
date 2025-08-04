using System;

namespace AmqpAgents.Messaging.Examples
{
    public record RentalCarRequest(
        string TripId,
        string Location,
        DateTime PickupDate,
        DateTime ReturnDate,
        VehicleType VehicleType
    );
}
