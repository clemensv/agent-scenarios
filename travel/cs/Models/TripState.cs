using System;
using System.Collections.Concurrent;

namespace AmqpAgents.Messaging.Examples
{
    public record TripState(TravelRequest Request)
    {
        private readonly ConcurrentDictionary<AgentType, TravelProposal> _proposals = new();

        public void AddProposal(TravelProposal proposal) =>
            _proposals[proposal.AgentType] = proposal;

        public TravelProposal? GetProposal(AgentType type) =>
            _proposals.TryGetValue(type, out var p) ? p : null;
    }
}
