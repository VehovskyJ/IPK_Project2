using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;

namespace IPK_Project2;

public static class Sniffer {
	private static int _packetCaptured = 0;
	private static SharpPcap.ILiveDevice? _dev;
	private static Cli _args;

	// Start capturing packets
	public static void Sniff(Cli args) {
		_args = args;
		
		// Get capturing device
		_dev = CaptureDeviceList.Instance.FirstOrDefault(device => device.Name == args.Interface);

		if (_dev == null) {
			throw new Exception($"Device {args.Interface} not found");
		}

		// Register packet handler
		_dev.OnPacketArrival += new PacketArrivalEventHandler(OnPacket);
		_dev.Open(DeviceModes.Promiscuous);
		_dev.StartCapture();
		
		// Wait until the program is terminated by user or by internal logic
		while (true) { }
	}

	// Stop capturing packets and close the device
	public static void Close() {
		if (_dev != null) {
			_dev.StopCapture();
			_dev.Close();
		}
	}

	// Handle captured packet
	static void OnPacket(object sender, PacketCapture packetCapture) {
		// Terminate the program when the maximum number of packets captured is reached
		if (_args.Num > 0 && _packetCaptured >= _args.Num) {
			Close();
			Environment.Exit(0);
		}
		_packetCaptured++;

		try {
			PacketInfo packetInfo = new PacketInfo();
			packetInfo.PacketToObject(packetCapture);
			packetInfo.PrintPacket(_packetCaptured, _args.Num);
		} catch (Exception e) {
			Error.Exit(e.Message);
		}
	}
}