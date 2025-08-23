using AdminToys;
using Exiled.API.Features;
using Mirror;
using System;
using System.Collections.Generic;

namespace RpCommands.Extensions
{
    public static class MirrorExtensions
    {
        private static readonly Dictionary<Type, ulong> SubWriteClassToMinULong = new()
        {
            [typeof(AdminToyBase)] = 16,
        };

        public static void SendFakeSyncVar<T>(this Player target, NetworkBehaviour networkBehaviour, ulong dirtyBit, T syncVar)
        {
            if (target.Connection == null)
                return;

            Type networkType = networkBehaviour.GetType();

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            Compression.CompressVarUInt(writer, 1);

            int headerPosition = writer.Position;
            writer.WriteByte(0);
            int contentPosition = writer.Position;

            writer.WriteULong(0);
            writer.WriteULong(dirtyBit);

            bool IsWritten = false;

            foreach (KeyValuePair<Type, ulong> kv in SubWriteClassToMinULong)
            {
                if (networkType.IsSubclassOf(kv.Key))
                {
                    if (kv.Value >= dirtyBit)
                        writer.Write(syncVar);

                    writer.WriteULong(dirtyBit);

                    if (kv.Value <= dirtyBit)
                        writer.Write(syncVar);

                    IsWritten = true;
                }
            }

            if (!IsWritten)
                writer.Write(syncVar);

            int endPosition = writer.Position;
            writer.Position = headerPosition;
            int size = endPosition - contentPosition;
            byte safety = (byte)(size & 0xFF);
            writer.WriteByte(safety);
            writer.Position = endPosition;

            target.Connection.Send(new EntityStateMessage
            {
                netId = networkBehaviour.netId,
                payload = writer.ToArraySegment(),
            });
        }
    }
}
