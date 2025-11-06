using JasperFx.Core;
using Wolverine.SignalR;

// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.SignalR;

public interface IWolverineWebSocketMessage : WebSocketMessage;

public record RequestMessage(string Message) : IWolverineWebSocketMessage;
public record ResponseMessage(string Message) : IWolverineWebSocketMessage;
public record DelayedResponseMessage(int MessageId);

public class SignalRHandler
{
    private static int _counter = 0;

    [EnlistInCurrentConnectionSaga]
    public (ResponseMessage, OutgoingMessages) Handle(RequestMessage request)
    {
        var current = _counter;
        Interlocked.Increment(ref _counter);

        return (
            new($"Message {current} recieved."), 
            [
                new DelayedResponseMessage(current).DelayedFor(5.Seconds())
            ]);
    }

    public ResponseMessage Handle(DelayedResponseMessage message)
    {
        return new($"Message {message.MessageId} processed.");
    }
}