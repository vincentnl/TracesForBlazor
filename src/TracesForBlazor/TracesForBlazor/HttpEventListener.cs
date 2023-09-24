

// using Yarp.Telemetry.Consumption;
//
// namespace TracesForBlazor;
//
// public class MyTelemetryConsumer : IHttpTelemetryConsumer
// {
//     public void OnConnectStart(DateTime timestamp, string address)
//     {
//         Console.WriteLine("start");
//     }
//
//     public void OnConnectStop(DateTime timestamp)
//     {
//         Console.WriteLine("stop");
//     }
// }

// public class HttpEventListener
// {
//     public sealed class MyListener : EventListener
//     {
//         protected override void OnEventSourceCreated(EventSource eventSource)
//         {
//             if (eventSource.Name == "System.Net.Http") EnableEvents(eventSource, EventLevel.Informational);
//         }
//
//         protected override void OnEventWritten(EventWrittenEventArgs eventData)
//         {
//             Console.WriteLine($"{DateTime.UtcNow:ss:fff} {eventData.EventName}: " +
//                               string.Join(' ',
//                                   eventData.PayloadNames!.Zip(eventData.Payload!)
//                                       .Select(pair => $"{pair.First}={pair.Second}")));
//         }
//     }
// }