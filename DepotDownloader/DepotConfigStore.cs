using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DepotDownloader
{

    [ProtoContract]
    class DepotConfigStore
    {

        [ProtoMember(1)]
        public IDictionary<uint, ulong> InstalledManifestIDs { get; private set; }

        string FileName = null;

        DepotConfigStore()
        {
            InstalledManifestIDs = new ConcurrentDictionary<uint, ulong>();
        }

        static bool Loaded
        {
            get { return Instance != null; }
        }

        public static DepotConfigStore Instance = null;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadFromFile(string filename)
        {
            if (Loaded)
                return;

            if (File.Exists(filename))
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                    Instance = ProtoBuf.Serializer.Deserialize<DepotConfigStore>(ds);
            }
            else
            {
                Instance = new DepotConfigStore();
            }

            Instance.FileName = filename;
        }

        private static volatile bool _saveQueued = false;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Save()
        {
            if (!Loaded)
                throw new Exception("Saved config before loading");

            if (_saveQueued)
                return;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                if (!_saveQueued)
                    return;

                do
                {
                    try
                    {
                        using (var fs = File.Open(Instance.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        using (var ds = new DeflateStream(fs, CompressionMode.Compress))
                            Serializer.Serialize(ds, Instance);
                        _saveQueued = false;
                        break;
                    }
                    catch (IOException)
                    {
                        // ok
                    }
                } while (_saveQueued);
            });

            _saveQueued = true;
        }

    }

}