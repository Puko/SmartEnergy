using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using SmartEnergy.Extensions;
using SmartEnergy.Interfaces;

namespace SmartEnergy.Api.Websocket
{
    public class WebsocketClient : IDisposable
    {
        public const string DisconnectedMessage = "Disconnected";

        private ClientWebSocket _websocket;
        private readonly Uri _websocketUrl;
        private List<IMessageReceiver> _receivers = new List<IMessageReceiver>();

        public WebsocketClient(ClientWebSocket websocket, Uri websocketUrl)
        {
            _websocket = websocket;
            _websocketUrl = websocketUrl;
        }

        public WebsocketClient(Uri uri) : this(new ClientWebSocket(), uri)
        {

        }

        public bool IsConnected => _websocket?.State == WebSocketState.Open;

        public void Subscribe(IMessageReceiver receiver)
        {
            var r = _receivers.FirstOrDefault(x => x.GetType() == receiver.GetType());
            if(r != null)
                _receivers.Remove(r);

            _receivers.Add(receiver);
        }

        public void Unsubscribe(IMessageReceiver receiver)
        {
            var r = _receivers.FirstOrDefault(x => x.Equals(receiver));
            if(r != null)
            {
                _receivers.Remove(r);
            }
        }

        public async Task<bool> ReconnectAsync(ILogService logService)
        {
            try
            {
                _websocket.Dispose();
                _websocket = new ClientWebSocket();
                await _websocket.ConnectAsync(_websocketUrl, CancellationToken.None);

                await this.UnsubscribeAll();

                foreach (var messageReceiver in _receivers)
                {
                    await messageReceiver.ResubscribeAsync();
                }

                return true;
            }
            catch (Exception e) 
            {
                logService.Exception(e, "Reconnection failed.");
            }

            return false;
        }

        public async Task ConnectAsync(ILogService logService)
        {
            try
            {
                await _websocket.ConnectAsync(_websocketUrl, CancellationToken.None);
            }
            catch (Exception e)
            {
                logService.Exception(e, "Connection to websokcet failed.");
            }
        }

        public async IAsyncEnumerable<string> ListenAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);

            while (!cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result = null;
                using var ms = new MemoryStream();
                do
                {
                    try
                    {
                        result = await _websocket.ReceiveAsync(buffer, cancellationToken);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    catch (Exception e)
                    {
                        result = new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                            WebSocketCloseStatus.InternalServerError, e.Message);
                    }

                } while (result != null && !result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    yield return DisconnectedMessage;
                    break;
                }

                var message = Encoding.UTF8.GetString(ms.ToArray());
                foreach (var item in _receivers)
                {
                    item.OnMessage(message);
                }

                yield return message;
            }
        }

        public async Task SendStringAsync(string data, CancellationToken cancellation)
        {
            try
            {
                var encoded = Encoding.UTF8.GetBytes(data);
                var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
                await _websocket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellation);
            }
            catch (Exception e)
            {

            }
        }

        public void Dispose()
        {
            _websocket.Dispose();
        }
    }
}
