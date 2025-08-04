using System;
using System.Threading.Tasks;
using AmqpAgents.Messaging.Configuration;
using CloudNative.CloudEvents;

namespace AmqpAgents.Messaging.Examples
{
    public class TrainTravelAgent
    {
        public static async Task RunAsync()
        {
            var agentConnector = await AgentRoleConnector.CreateAsync(
                "travel-topology.json",
                "TrainTravelAgent"
            );

            agentConnector += (
                "train.travel",
                HandlerOptionsFlags.AutoComplete,
                async (CloudEvent cloudEvent, context) =>
                {
                    // Demux based on CloudEvent type
                    if (cloudEvent.Type != "com.contoso.travel.TrainSearchRequest")
                        return;

                    var request = cloudEvent.Data as TrainSearchRequest;
                    if (request == null)
                        return;

                    Console.WriteLine(
                        $"Searching trains: {request.Origin} → {request.Destination}"
                    );

                    var proposal = new TravelProposal(
                        request.TripId,
                        AgentType.Train,
                        new TravelOption[]
                        {
                            new("High-Speed Rail", 180.00m, "3h 45m"),
                            new("Regional Train", 95.00m, "6h 20m"),
                        },
                        180.00m,
                        0.88f
                    );

                    var proposalEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.TravelProposal",
                        Source = new Uri("TrainTravelAgent/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = proposal,
                    };

                    await agentConnector.SendAsync("travel.proposals", proposalEvent);
                }
            );

            agentConnector += (
                "train.refine",
                HandlerOptionsFlags.Transactional | HandlerOptionsFlags.AutoComplete,
                async (CloudEvent cloudEvent, context) =>
                {
                    // Demux based on CloudEvent type
                    if (cloudEvent.Type != "com.contoso.travel.RefinementRequest")
                        return;

                    var refinement = cloudEvent.Data as RefinementRequest;
                    if (refinement == null)
                        return;

                    Console.WriteLine($"Refining train options based on: {refinement.Feedback}");

                    var refinedProposal = new TravelProposal(
                        refinement.TripId,
                        AgentType.Train,
                        new TravelOption[]
                        {
                            new("Economy Regional", 65.00m, "7h 30m"),
                            new("Sleeper Train", 85.00m, "8h 45m overnight"),
                        },
                        65.00m,
                        0.82f
                    );

                    var proposalEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.TravelProposal",
                        Source = new Uri("TrainTravelAgent/instance1", UriKind.Relative),
                        Subject = refinement.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = refinedProposal,
                    };

                    await agentConnector.SendAsync("travel.proposals", proposalEvent);
                }
            );

            Console.WriteLine("TrainTravelAgent ready");
            await Task.Delay(-1);
        }
    }
}
