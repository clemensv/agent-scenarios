using System;
using System.Threading.Tasks;
using AmqpAgents.Messaging.Configuration;

namespace AmqpAgents.Messaging.Examples
{
    public class RoadTravelAgent
    {
        public static async Task RunAsync()
        {
            var agentConnector = await AgentRoleConnector.CreateAsync(
                "travel-topology.json",
                "RoadTravelAgent"
            );

            agentConnector += (
                "road.travel",
                HandlerOptionsFlags.None,
                async (RoadTravelRequest request, context) =>
                {
                    Console.WriteLine(
                        $"Searching road travel: {request.Origin} → {request.Destination}"
                    );

                    var proposal = new TravelProposal(
                        request.TripId,
                        AgentType.Road,
                        new TravelOption[]
                        {
                            new("Express Bus", 45.00m, "5h 30m"),
                            new("Standard Bus", 35.00m, "7h 15m"),
                        },
                        45.00m,
                        0.82f
                    );

                    await agentConnector.SendAsync("travel.proposals", proposal);
                    await context.CompleteAsync();
                }
            );

            agentConnector += (
                "road.refine",
                HandlerOptionsFlags.Transactional,
                async (RefinementRequest refinement, context) =>
                {
                    Console.WriteLine(
                        $"Refining road travel options based on: {refinement.Feedback}"
                    );

                    var refinedProposal = new TravelProposal(
                        refinement.TripId,
                        AgentType.Road,
                        new TravelOption[]
                        {
                            new("Budget Bus", 25.00m, "8h 45m"),
                            new("Rideshare", 30.00m, "6h 30m"),
                        },
                        25.00m,
                        0.78f
                    );

                    await agentConnector.SendAsync("travel.proposals", refinedProposal);
                    await context.CompleteAsync();
                }
            );

            Console.WriteLine("RoadTravelAgent ready");
            await Task.Delay(-1);
        }
    }
}
