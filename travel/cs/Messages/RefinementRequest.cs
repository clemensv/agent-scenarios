using System;

namespace AmqpAgents.Messaging.Examples
{
    public record RefinementRequest(
        string TripId,
        AgentType AgentType,
        string FeedbackType,
        string Feedback
    );
}
