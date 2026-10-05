using System;
using System.IO;
using System.Text;

namespace Sharp.EL9300 {
	public class LinkTransfer {

		private byte[] head = new byte[32];
		public byte[] Head {
			get => head;
			set {
				if (value.Length != 32) throw new InvalidOperationException("Head must be 32 bytes in size.");
				head = value;
			}
		}

		private byte[] body = new byte[0];
		public byte[] Body {
			get => body;
			set {
				if (body == null) throw new NullReferenceException();
				body = value;
			}
		}

		private void CheckBodySize() {
			var bodySize = head[3] + (head[4] << 8);
			if (body.Length != bodySize) throw new InvalidDataException("Body size does not match size specified in header.");
		}

		public LinkTransfer(byte[] head, byte[] body) {
			this.head = head;
			this.body = body;
			CheckBodySize();
		}

		public LinkTransfer(Stream stream) {
			var reader = new BinaryReader(stream);
			head = reader.ReadBytes(32);
			var bodySize = head[3] + (head[4] << 8);
			body = reader.ReadBytes(bodySize);
		}

		public LinkTransfer(string path) {
			using (var reader = new BinaryReader(File.OpenRead(path))) {
				head = reader.ReadBytes(32);
				var bodySize = head[3] + (head[4] << 8);
				body = reader.ReadBytes(bodySize);
			}
		}

		public void Save(Stream stream) {
			CheckBodySize();
			var writer = new BinaryWriter(stream);
			writer.Write(head);
			writer.Write(body);
		}

		public void Save(string path) {
			using (var stream = File.Create(path)) {
				Save(stream);
			}
		}

		public string GetFileName() {
			switch (head[0x00]) {
				case 0: // Matrices, stats
					if (head[0x11] == 0xFF) {
						return Encoding.ASCII.GetString(body, 1, 8).TrimEnd() + ".gam";
					} else if (head[0x11] == 0x40) {
						return "DATA.g1l";
					} else {
						return Encoding.ASCII.GetString(head, 0x11, 1) + ".g1m";
					}
				case 1: // Graph equations
				case 2: // Programs
				case 3: // Solver equations
					var extension = "?ype"[head[0x00]];
					if (head[0x02] == 0) {
						return Encoding.ASCII.GetString(body, 9, 16).TrimEnd('\0') + ".g1" + extension;
					} else {
						return Encoding.ASCII.GetString(body, 1, 8).TrimEnd() + ".ga" + extension;
					}
				case 4: // Backups
					return "BACKUP.gcb";
				default:
					return null;
			}
		}
	}
}
