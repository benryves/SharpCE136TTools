using System;
using System.IO;

namespace Sharp.EL9300 {
	internal class LinkTransfer {

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

	}
}
