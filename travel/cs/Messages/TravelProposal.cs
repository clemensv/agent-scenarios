using System;

namespace AmqpAgents.Messaging.Examples
{
    public record TravelProposal(
        string TripId,
        AgentType AgentType,
        TravelOption[] Options,
        decimal EstimatedCost,
        float Confidence
    );
}
