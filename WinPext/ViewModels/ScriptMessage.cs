using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Media;

namespace frida_windows_package_manager.ViewModels
{
    public class ScriptMessage
    {
        public string MessageClass { get; private set; }
        public string MessageType { get; private set; }
        public Brush TagBrush { get; private set; }
        public string Content { get; private set; }

        public ScriptMessage() {
            TagBrush = new SolidColorBrush(Colors.Gray);
        }

        [DataContract]
        class JsonMessage
        {
            [DataMember]
            public string type { get; set; }
            [DataMember]
            public string level { get; set; }
            [DataMember]
            public string payload { get; set; }
            [DataMember]
            public string stack { get; set; }
            [DataMember]
            public string description { get; set; }
        }

        public ScriptMessage(string message)
        {
            TagBrush = new SolidColorBrush(Colors.Gray);
            Content = message;

            JsonMessage msg = null;
            var jsc = new DataContractJsonSerializer(typeof(JsonMessage));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                ms.Position = 0;
                msg = (JsonMessage)jsc.ReadObject(ms);
            }

            if (msg.type == "log")
            {
                if (msg.level == "warning")
                {
                    msg.level = "warn";
                    TagBrush = new SolidColorBrush(Colors.DarkOrange);
                }
                else if (msg.level == "error")
                {
                    TagBrush = new SolidColorBrush(Colors.Red);
                }

                const string dotNetBridgeToken = "DotNet: ";
                if (msg.payload.StartsWith(dotNetBridgeToken))
                {
                    MessageClass = ".net";
                    msg.payload = msg.payload.Remove(0, dotNetBridgeToken.Length);
                }
                else
                {
                    MessageClass = "js";
                }

                Content = msg.payload;
                MessageType = msg.level;
            }
            else if (msg.type == "error")
            {
                if (!string.IsNullOrWhiteSpace(msg.stack))
                {
                    Content = msg.stack.Replace("\\n", "\r\n");
                }
                else
                {
                    Content = msg.description;
                }

                MessageClass = "js";
                MessageType = "error";
                TagBrush = new SolidColorBrush(Colors.Red);
            }
            else if (msg.type == "send")
            {
                Content = msg.payload;
                MessageType = "send";
                MessageClass = "js";
                TagBrush = new SolidColorBrush(Colors.DarkMagenta);
            }
        }

        public static ScriptMessage CreateInfo(string message)
        {
            var info = new ScriptMessage();
            info.TagBrush = new SolidColorBrush(Colors.Gray);
            info.MessageType = "";
            info.Content = message;
            return info;
        }

        internal static ScriptMessage CreateError(string message)
        {
            var error = CreateInfo(message);
            error.MessageType = "!";
            error.TagBrush = new SolidColorBrush(Colors.Red);
            return error;
        }
    }
}
