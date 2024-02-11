using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodeEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NodeEditorDemo.Services;

internal class NodeSerializer : INodeSerializer
{
    private readonly JsonSerializerSettings _settings;

    private class ListContractResolver(Type listType) : DefaultContractResolver
    {
        private readonly Type _listType = listType;

        public override JsonContract ResolveContract(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) {
                return base.ResolveContract(_listType.MakeGenericType(type.GenericTypeArguments[0]));
            }
            return base.ResolveContract(type);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
        }
    }

    public NodeSerializer(Type listType)
    {
        _settings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ListContractResolver(listType),
            NullValueHandling = NullValueHandling.Ignore,
        };
    }

    public string Serialize<T>(T value)
    {
        return JsonConvert.SerializeObject(value, _settings);
    }

    public T? Deserialize<T>(string text)
    {
        return JsonConvert.DeserializeObject<T>(text, _settings);
    }

    public T? Load<T>(string path)
    {
        using System.IO.FileStream stream = System.IO.File.OpenRead(path);
        using System.IO.StreamReader streamReader = new(stream, Encoding.UTF8);
        string text = streamReader.ReadToEnd();
        return Deserialize<T>(text);
    }

    public void Save<T>(string path, T value)
    {
        string text = Serialize(value);
        if (string.IsNullOrWhiteSpace(text))
            return;
        using System.IO.FileStream stream = System.IO.File.Create(path);
        using System.IO.StreamWriter streamWriter = new(stream, Encoding.UTF8);
        streamWriter.Write(text);
    }
}
