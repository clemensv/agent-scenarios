using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AmqpAgents.Messaging.Configuration;
using CloudNative.CloudEvents;

namespace AmqpAgents.Messaging.Examples
{
    public class TravelPlannerService
    {
        private static readonly ConcurrentDictionary<string, TripState> _tripStates = new();

        public static async Task RunAsync()
        {
            var agentConnector = await AgentRoleConnector.CreateAsync(
                "travel-topology.json",
                "TravelPlannerService"
            );

            agentConnector += (
                "travel.requests",
                HandlerOptionsFlags.Transactional,
                async (CloudEvent cloudEvent, context) =>
                {
                    // Demux based on CloudEvent type
                    if (cloudEvent.Type != "com.contoso.travel.TravelRequest")
                        return;

                    var request = cloudEvent.Data as TravelRequest;
                    if (request == null)
                        return;

                    Console.WriteLine(
                        $"Planning trip {request.TripId}: {request.Origin} → {request.Destination}"
                    );
                    _tripStates[request.TripId] = new TripState(request);

                    // Send flight search request as CloudEvent
                    var flightEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.FlightSearchRequest",
                        Source = new Uri("TravelPlannerService/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = new FlightSearchRequest(
                            request.TripId,
                            request.Origin,
                            request.Destination,
                            request.Dates.Start,
                            request.Dates.End,
                            1
                        ),
                    };
                    await agentConnector.SendAsync("air.travel", flightEvent);

                    // Send train search request as CloudEvent
                    var trainEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.TrainSearchRequest",
                        Source = new Uri("TravelPlannerService/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = new TrainSearchRequest(
                            request.TripId,
                            request.Origin,
                            request.Destination,
                            request.Dates.Start,
                            request.Dates.End,
                            1
                        ),
                    };
                    await agentConnector.SendAsync("train.travel", trainEvent);

                    // Send road travel request as CloudEvent
                    var roadEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.RoadTravelRequest",
                        Source = new Uri("TravelPlannerService/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = new RoadTravelRequest(
                            request.TripId,
                            request.Origin,
                            request.Destination,
                            request.Dates.Start,
                            request.Dates.End,
                            RoadTravelType.Bus
                        ),
                    };
                    await agentConnector.SendAsync("road.travel", roadEvent);

                    // Send accommodation request as CloudEvent
                    var accommodationEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.AccommodationRequest",
                        Source = new Uri("TravelPlannerService/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = new AccommodationRequest(
                            request.TripId,
                            request.Destination,
                            request.Dates.Start,
                            request.Dates.End,
                            1,
                            new AccommodationPreferences()
                        ),
                    };
                    await agentConnector.SendAsync("accommodations", accommodationEvent);

                    // Send rental car request as CloudEvent
                    var rentalEvent = new CloudEvent
                    {
                        Type = "com.contoso.travel.RentalCarRequest",
                        Source = new Uri("TravelPlannerService/instance1", UriKind.Relative),
                        Subject = request.TripId,
                        Time = DateTimeOffset.UtcNow,
                        Data = new RentalCarRequest(
                            request.TripId,
                            request.Destination,
                            request.Dates.Start,
                            request.Dates.End,
                            VehicleType.Economy
                        ),
                    };
                    await agentConnector.SendAsync("rental.cars", rentalEvent);

                    await context.CompleteAsync();
                }
            );

            agentConnector += (
                "travel.proposals",
                HandlerOptionsFlags.None,
                async (CloudEvent cloudEvent, context) =>
                {
                    // Demux based on CloudEvent type
                    if (cloudEvent.Type != "com.contoso.travel.TravelProposal")
                        return;

                    var proposal = cloudEvent.Data as TravelProposal;
                    if (proposal == null)
                        return;

                    Console.WriteLine(
                        $"Received {proposal.AgentType} proposal for trip {proposal.TripId}: ${proposal.EstimatedCost}"
                    );

                    if (_tripStates.TryGetValue(proposal.TripId, out var state))
                    {
                        state.AddProposal(proposal);

                        // Send refinement feedback based on budget and preferences
                        var feedback = GenerateRefinementFeedback(state.Request, proposal);
                        if (feedback != null)
                        {
                            var refineName = proposal.AgentType.ToString().ToLower() switch
                            {
                                "air" => "air.refine",
                                "train" => "train.refine",
                                "road" => "road.refine",
                                "accommodation" => "accommodation.refine",
                                "rentalcar" => "rentalcar.refine",
                                _ => null,
                            };
                            if (refineName != null)
                            {
                                var refinementEvent = new CloudEvent
                                {
                                    Type = "com.contoso.travel.RefinementRequest",
                                    Source = new Uri(
                                        "TravelPlannerService/instance1",
                                        UriKind.Relative
                                    ),
                                    Subject = proposal.TripId,
                                    Time = DateTimeOffset.UtcNow,
                                    Data = feedback,
                                };
                                await agentConnector.SendAsync(refineName, refinementEvent);
                            }
                        }
                    }

                    await context.CompleteAsync();
                }
            );

            Console.WriteLine("TravelPlannerService ready");
            await Task.Delay(-1);
        }

        private static RefinementRequest? GenerateRefinementFeedback(
            TravelRequest request,
            TravelProposal proposal
        )
        {
            if (proposal.EstimatedCost > request.Budget * 0.8m)
            {
                return new RefinementRequest(
                    proposal.TripId,
                    proposal.AgentType,
                    "budget",
                    $"Options too expensive, max budget: {request.Budget}"
                );
            }
            return null;
        }
    }
}
