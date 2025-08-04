using System;
using System.Threading.Tasks;
using AmqpAgents.Messaging.Configuration;
using CloudNative.CloudEvents;

namespace AmqpAgents.Messaging.Examples
{
    public class AirTravelAgent
    {
        public static async Task RunAsync()
        {
            var agentConnector = await AgentRoleConnector.CreateAsync(
                "travel-topology.json",
                "AirTravelAgent"
            );

            agentConnector += (
                "air.travel",
                HandlerOptionsFlags.AutoComplete,
                async (CloudEvent cloudEvent, context) =>
                {
                    // Demux based on CloudEvent type
                    if (cloudEvent.Type != "com.contoso.travel.FlightSearchRequest")
                        return;

                    var request = cloudEvent.Data as FlightSearchRequest;
                    if (request == null)
                        return;

                    Console.WriteLine(
                        $"Searching flights: {request.Origin} → {request.Destination}"
                    );

                    var proposal = new TravelProposal(
                        request.TripId,
                        AgentType.Air,
                        new TravelOption[]
                        {
                            new("Direct Flight", 450.00m, "2h 30m"),
                            new("Connecting Flight", 320.00m, "4h 15m"),
                        },
                        450.00m,
                        0.95f
                    );

                    var proposalEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.TravelProposal",
                        Source = new Uri("AirTravelAgent/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = proposal,
                    };

                    await agentConnector.SendAsync("travel.proposals", proposalEvent);
                }
            );

            agentConnector += (
                "air.refine",
                HandlerOptionsFlags.Transactional | HandlerOptionsFlags.AutoComplete,
                async (CloudEvent cloudEvent, context) =>
                {
                    // Demux based on CloudEvent type
                    if (cloudEvent.Type != "com.contoso.travel.RefinementRequest")
                        return;

                    var refinement = cloudEvent.Data as RefinementRequest;
                    if (refinement == null)
                        return;

                    Console.WriteLine($"Refining flight options based on: {refinement.Feedback}");

                    var refinedProposal = new TravelProposal(
                        refinement.TripId,
                        AgentType.Air,
                        new TravelOption[]
                        {
                            new("Budget Airline", 180.00m, "3h 15m"),
                            new("Red-eye Flight", 220.00m, "2h 45m"),
                        },
                        180.00m,
                        0.85f
                    );

                    var proposalEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.TravelProposal",
                        Source = new Uri("AirTravelAgent/instance1", UriKind.Relative),
                        Subject = refinement.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = refinedProposal,
                    };

                    await agentConnector.SendAsync("travel.proposals", proposalEvent);
                }
            );

            Console.WriteLine("AirTravelAgent ready");
            await Task.Delay(-1);
        }
    }
}
