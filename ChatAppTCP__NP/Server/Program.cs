using System.Net;
using System.Net.Sockets;


var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var listener =new TcpListener(ip, port);

var users = new Dictionary<string, TcpClient>();
listener.Start(10);
while (true)
{
	try
	{
		//	Get client
        var SentClient = await listener.AcceptTcpClientAsync();


		//	Get stream of client
        var SentClientStream = SentClient.GetStream();

		//	Create adapter for read from stream
		var binaryReader = new BinaryReader(SentClientStream);

		//	Read string from stream
		var readString = binaryReader.ReadString();

		if (readString.Contains('/') == false)
		{
			users.Add(readString, SentClient);

		}
		
		else
		{

			//	Seperate readString
			int indexSplash = readString.IndexOf("/");
			int index2Point = readString.IndexOf(":");
			var SentUserName = readString.Substring(0, indexSplash);
			var RecievedUserName = readString.Substring(indexSplash + 1,index2Point - indexSplash - 1);
			var message = readString.Substring(index2Point + 1);
			Console.WriteLine($@"
SentUserName - {SentUserName}
RecievedUserName - {RecievedUserName}
message - {message}");

			//	Check reciever client contains in users
			if (users.ContainsKey(RecievedUserName))
			{
				var RecievedClient = users[RecievedUserName];

				var RecievedClientStream = RecievedClient.GetStream();
				var binaryWriter = new BinaryWriter(RecievedClientStream);
				var messageString = RecievedUserName + ':' + message;
				binaryWriter.Write(messageString);
			}
			else
			{
				var binaryWriter = new BinaryWriter(SentClientStream);
				binaryWriter.Write("Such a person does not exist!");

			}
		}

	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
	
}
