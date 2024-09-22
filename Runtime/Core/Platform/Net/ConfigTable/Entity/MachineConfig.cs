#if FANTASY_NET
using System;
using ProtoBuf;
using Fantasy;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Fantasy.ConfigTable;
using Fantasy.Serialize;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS0169
#pragma warning disable CS8618
#pragma warning disable CS8625
#pragma warning disable CS8603

namespace Fantasy.Platform.Net
{
    [ProtoContract]
    public sealed partial class MachineConfigData : ASerialize, IConfigTable, IProto
    {
        [ProtoMember(1)]
        public List<MachineConfig> List { get; set; } = new List<MachineConfig>();
#if FANTASY_NET
        [ProtoIgnore]
        private readonly ConcurrentDictionary<uint, MachineConfig> _configs = new ConcurrentDictionary<uint, MachineConfig>();
#else 
        [ProtoIgnore]
        private readonly Dictionary<uint, MachineConfig> _configs = new Dictionary<uint, MachineConfig>();
#endif
        private static MachineConfigData _instance = null;

        public static MachineConfigData Instance
        {
            get { return _instance ??= ConfigTableHelper.Load<MachineConfigData>(); } 
            private set => _instance = value;
        }

        public MachineConfig Get(uint id, bool check = true)
        {
            if (_configs.ContainsKey(id))
            {
                return _configs[id];
            }
    
            if (check)
            {
                throw new Exception($"MachineConfig not find {id} Id");
            }
            
            return null;
        }
        public bool TryGet(uint id, out MachineConfig config)
        {
            config = null;
            
            if (!_configs.ContainsKey(id))
            {
                return false;
            }
                
            config = _configs[id];
            return true;
        }
        public override void AfterDeserialization()
        {
            foreach (var config in List)
            {
#if FANTASY_NET
                _configs.TryAdd(config.Id, config);
#else
                _configs.Add(config.Id, config);
#endif
                config.AfterDeserialization();
            }
    
            EndInit();
        }
        
        public override void Dispose()
        {
            Instance = null;
        }
    }
    
    [ProtoContract]
    public sealed partial class MachineConfig : ASerialize, IProto
    {
		[ProtoMember(1)]
		public uint Id { get; set; } // Id
		[ProtoMember(2)]
		public string OuterIP { get; set; } // 外网IP
		[ProtoMember(3)]
		public string OuterBindIP { get; set; } // 外网绑定IP
		[ProtoMember(4)]
		public string InnerBindIP { get; set; } // 内网绑定IP  
    } 
}  
#endif