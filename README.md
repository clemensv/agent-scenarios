# Agent Scenarios: Complex Multi-Agent System Examples

## Overview

This repository contains a collection of example scenarios that demonstrate
complex interactions between distributed agents using AMQP and CloudEvents.

The programming model in the included C# code is hypothetical. There is no such
library at the moment, but the API shape is absolutely plausible. C# is also
just used as an illustration vehicle here.

These scenarios showcase asynchronous message flows between different types of
agents, including both deterministic rule-based agents and LLM-based intelligent
agents.

## Purpose

The goal of these samples is to illustrate:

- **Asynchronous Message Flows**: How agents communicate through message queues
  and event streams without blocking operations
- **Multi-Agent Coordination**: Complex orchestration patterns between multiple
  specialized agents
- **Hybrid Agent Architectures**: Integration of deterministic logic agents with
  LLM-powered intelligent agents
- **Real-World Scenarios**: Practical use cases that demonstrate
  enterprise-grade agent interactions

## Agent Types

### Deterministic Agents

- **Rule-based agents** that follow predefined logic and workflows
- **Orchestration services** that coordinate multiple specialist agents
- **Data processing agents** that transform and route messages
- **Integration agents** that connect to external systems and APIs

### LLM-Based Agents

- **Intelligent reasoning agents** that make context-aware decisions
- **Natural language processing agents** for understanding user intent
- **Adaptive agents** that learn from interactions and improve over time
- **Creative agents** that generate novel solutions and recommendations

## Message Flow Patterns

The scenarios demonstrate various asynchronous messaging patterns:

- **Request-Response**: Simple query-reply interactions
- **Publish-Subscribe**: Event-driven notifications and reactions  
- **Request-Proposal-Refinement**: Iterative improvement cycles
- **Scatter-Gather**: Parallel processing with result aggregation
- **Saga Pattern**: Long-running distributed transactions
- **Event Sourcing**: State management through event streams

## Technology Stack

- **Messaging**: AMQP 1.0 for reliable asynchronous message delivery
- **Events**: CloudEvents specification for standardized event formatting
- **Orchestration**: Message-driven coordination between agents
- **Observability**: Distributed tracing and event logging

## Scenarios

### 🧳 Travel Planning System

**Location**: [`/travel`](./travel/)

A comprehensive travel planning system that demonstrates the
**Request-Proposal-Refinement** pattern with six specialized agents:

- **Orchestrator**: TravelPlannerService coordinates the entire workflow
- **Specialists**: AirTravelAgent, TrainTravelAgent, RoadTravelAgent,
  AccommodationAgent, RentalCarAgent
- **Pattern**: Parallel expert consultations with iterative refinement cycles
- **Complexity**: Multi-round negotiations with constraint optimization

**Key Features**:

- Asynchronous parallel processing of travel options
- Iterative refinement based on budget and preferences  
- Event-driven state management with travel lifecycle events
- Integration of deterministic routing logic with LLM-based recommendation
  engines

## Getting Started

Each scenario includes:

- **Documentation**: Detailed explanation of the agent architecture and message
  flows
- **Topology Configuration**: AMQP messaging topology definitions
- **Message Schemas**: CloudEvents message type definitions
- **Sequence Diagrams**: Visual representation of agent interactions
- **Implementation Guidance**: Code examples and integration patterns

## Architecture Principles

### Asynchronous-First Design

All agent interactions are designed to be non-blocking, enabling:

- High throughput and scalability
- Resilience to individual agent failures
- Independent scaling of agent components
- Natural handling of variable processing times

### Event-Driven Architecture

Agents communicate through events and messages:

- **Loose coupling** between agent components
- **Temporal decoupling** allowing agents to process at their own pace
- **Location transparency** enabling distributed deployment
- **Protocol independence** supporting various transport mechanisms

### Hybrid Intelligence

Combining deterministic and LLM-based agents:

- **Predictable behavior** for business-critical operations
- **Intelligent adaptation** for complex decision-making
- **Cost optimization** using appropriate agent types for different tasks
- **Explainable workflows** with clear decision audit trails

## Contributing

We welcome contributions of new scenarios that demonstrate:

- Novel agent interaction patterns
- Integration with different LLM providers
- Advanced messaging topologies
- Real-world business use cases

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file
for details.

---

**Note**: These examples are designed for educational and demonstration
purposes. For production deployments, additional considerations around security,
monitoring, error handling, and performance optimization should be implemented.
