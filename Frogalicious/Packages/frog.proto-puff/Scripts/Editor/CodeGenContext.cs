using System;
using System.Collections.Generic;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;
using UnityEngine;

namespace Frog.ProtoPuff.Editor
{
    internal class CodeGenContext
    {
        private readonly HashSet<string> _knownTypes = new HashSet<string>();

        private readonly Dictionary<string, Primitive> _enumBaseTypes = new Dictionary<string, Primitive>();
        private readonly HashSet<string> _noCopyTypes = new HashSet<string>();

        public string GetCSharpTypeName(string name)
        {
            if (PrimitivesMap.TryGet(name, out var primitive))
                return primitive.CSharpName();

            if (_knownTypes.Contains(name))
                return name;

            throw new Exception($"Field type `{name}` is unknown");
        }

        public bool IsNoCopyStruct(in PuffStruct structDef)
        {
            foreach (ref readonly var field in structDef.Fields.RefReadonlyIter())
            {
                if (field.IsRepeated || _noCopyTypes.Contains(field.Type))
                    return true;
            }

            return false;
        }

        public bool TryGetEnumBaseType(string type, out Primitive primitive)
        {
            return _enumBaseTypes.TryGetValue(type, out primitive);
        }

        public void RegisterEnum(string name, Primitive underlyingType)
        {
            Debug.Assert(underlyingType.CanBeEnumBaseType());

            _knownTypes.Add(name);
            _enumBaseTypes.Add(name, underlyingType);
        }

        public void RegisterStruct(string name, bool noCopy)
        {
            _knownTypes.Add(name);

            if (noCopy)
                _noCopyTypes.Add(name);
        }
    }
}