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
    public sealed partial class SceneConfigData : ASerialize, IConfigTable, IProto
    {
        [ProtoMember(1)]
        public List<SceneConfig> List { get; set; } = new List<SceneConfig>();
#if FANTASY_NET
        [ProtoIgnore]
        private readonly ConcurrentDictionary<uint, SceneConfig> _configs = new ConcurrentDictionary<uint, SceneConfig>();
#else 
        [ProtoIgnore]
        private readonly Dictionary<uint, SceneConfig> _configs = new Dictionary<uint, SceneConfig>();
#endif
        private static SceneConfigData _instance = null;

        public static SceneConfigData Instance
        {
            get { return _instance ??= ConfigTableHelper.Load<SceneConfigData>(); } 
            private set => _instance = value;
        }

        public SceneConfig Get(uint id, bool check = true)
        {
            if (_configs.ContainsKey(id))
            {
                return _configs[id];
            }
    
            if (check)
            {
                throw new Exception($"SceneConfig not find {id} Id");
            }
            
            return null;
        }
        public bool TryGet(uint id, out SceneConfig config)
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
    public sealed partial class SceneConfig : ASerialize, IProto
    {
		[ProtoMember(1)]
		public uint Id { get; set; } // ID
		[ProtoMember(2)]
		public uint ProcessConfigId { get; set; } // 进程Id
		[ProtoMember(3)]
		public uint WorldConfigId { get; set; } // 世界Id
		[ProtoMember(4)]
		public string SceneRuntimeType { get; set; } // Scene运行类型
		[ProtoMember(5)]
		public string SceneTypeString { get; set; } // Scene类型
		[ProtoMember(6)]
		public string NetworkProtocol { get; set; } // 协议类型
		[ProtoMember(7)]
		public int OuterPort { get; set; } // 外网端口
		[ProtoMember(8)]
		public int InnerPort { get; set; } // 内网端口
		[ProtoMember(9)]
		public int SceneType { get; set; } // Scene类型  
    } 
} 
#endif