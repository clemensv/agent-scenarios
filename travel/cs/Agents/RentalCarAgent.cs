using System;
using System.Threading.Tasks;
using AmqpAgents.Messaging.Configuration;

namespace AmqpAgents.Messaging.Examples
{
    public class RentalCarAgent
    {
        public static async Task RunAsync()
        {
            var agentConnector = await AgentRoleConnector.CreateAsync(
                "travel-topology.json",
                "RentalCarAgent"
            );

            agentConnector += (
                "rental.cars",
                HandlerOptionsFlags.None,
                async (RentalCarRequest request, context) =>
                {
                    Console.WriteLine($"Searching rental cars in {request.Location}");

                    var proposal = new TravelProposal(
                        request.TripId,
                        AgentType.RentalCar,
                        new TravelOption[]
                        {
                            new("Economy Car", 35.00m, "Compact, fuel efficient"),
                            new("SUV", 65.00m, "Spacious, all-terrain"),
                            new("Luxury Sedan", 95.00m, "Premium comfort"),
                        },
                        35.00m,
                        0.89f
                    );

                    await agentConnector.SendAsync("travel.proposals", proposal);
                    await context.CompleteAsync();
                }
            );

            agentConnector += (
                "rentalcar.refine",
                HandlerOptionsFlags.Transactional,
                async (RefinementRequest refinement, context) =>
                {
                    Console.WriteLine(
                        $"Refining rental car options based on: {refinement.Feedback}"
                    );

                    var refinedProposal = new TravelProposal(
                        refinement.TripId,
                        AgentType.RentalCar,
                        new TravelOption[]
                        {
                            new("Budget Compact", 25.00m, "Basic transportation"),
                            new("Used Car Rental", 28.00m, "Older model, reliable"),
                            new("Peer-to-Peer", 30.00m, "Local car sharing"),
                        },
                        25.00m,
                        0.83f
                    );

                    await agentConnector.SendAsync("travel.proposals", refinedProposal);
                    await context.CompleteAsync();
                }
            );

            Console.WriteLine("RentalCarAgent ready");
            await Task.Delay(-1);
        }
    }
}
