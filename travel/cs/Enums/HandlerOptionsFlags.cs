using System;

namespace AmqpAgents.Messaging.Examples
{
    [Flags]
    public enum HandlerOptionsFlags
    {
        None = 0,
        Transactional = 1,
        AutoComplete = 2,
    }
}
