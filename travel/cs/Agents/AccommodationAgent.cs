using System;
using System.Threading.Tasks;
using AmqpAgents.Messaging.Configuration;

namespace AmqpAgents.Messaging.Examples
{
    public class AccommodationAgent
    {
        public static async Task RunAsync()
        {
            var agentConnector = await AgentRoleConnector.CreateAsync(
                "travel-topology.json",
                "AccommodationAgent"
            );

            agentConnector += (
                "accommodations",
                HandlerOptionsFlags.None,
                async (AccommodationRequest request, context) =>
                {
                    Console.WriteLine($"Searching accommodations in {request.Location}");

                    var proposal = new TravelProposal(
                        request.TripId,
                        AgentType.Accommodation,
                        new TravelOption[]
                        {
                            new("Downtown Hotel", 120.00m, "4-star, city center"),
                            new("Business Hotel", 89.00m, "3-star, near airport"),
                            new("Vacation Rental", 75.00m, "2-bedroom apartment"),
                        },
                        120.00m,
                        0.91f
                    );

                    await agentConnector.SendAsync("travel.proposals", proposal);
                    await context.CompleteAsync();
                }
            );

            agentConnector += (
                "accommodation.refine",
                HandlerOptionsFlags.Transactional,
                async (RefinementRequest refinement, context) =>
                {
                    Console.WriteLine(
                        $"Refining accommodation options based on: {refinement.Feedback}"
                    );

                    var refinedProposal = new TravelProposal(
                        refinement.TripId,
                        AgentType.Accommodation,
                        new TravelOption[]
                        {
                            new("Budget Hostel", 35.00m, "Shared room, basic amenities"),
                            new("Motel", 55.00m, "Private room, highway location"),
                            new("Airbnb Studio", 65.00m, "Private studio, kitchen"),
                        },
                        35.00m,
                        0.87f
                    );

                    await agentConnector.SendAsync("travel.proposals", refinedProposal);
                    await context.CompleteAsync();
                }
            );

            Console.WriteLine("AccommodationAgent ready");
            await Task.Delay(-1);
        }
    }
}
