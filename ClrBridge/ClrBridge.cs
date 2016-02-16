using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ClrBridge
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid("ea688a1d-4be4-4cae-b2a3-9a389fcd1c8b")]
    public interface IClrBridge
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string CreateObject([MarshalAs(UnmanagedType.LPWStr)]string typeName, [MarshalAs(UnmanagedType.LPWStr)]string args);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string DescribeObject([MarshalAs(UnmanagedType.LPWStr)]string typeName, [MarshalAs(UnmanagedType.LPWStr)]string objRef);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string CreateDelegate([MarshalAs(UnmanagedType.LPWStr)]string typeName, IntPtr callback);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string InvokeMethod([MarshalAs(UnmanagedType.LPWStr)]string objRef, [MarshalAs(UnmanagedType.LPWStr)]string typeName, 
            [MarshalAs(UnmanagedType.LPWStr)]string methodName, [MarshalAs(UnmanagedType.LPWStr)]string args, [MarshalAs(UnmanagedType.LPWStr)]string genericTypesRef, int boxed);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ReleaseObject([MarshalAs(UnmanagedType.LPWStr)]string objRef);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string DescribeNamespace([MarshalAs(UnmanagedType.LPWStr)]string nameSpace);
    }

    public class ClrBridgeProvider
    {
        delegate int GetBridgeCallback(IntPtr bridgeAddress);
        public static int GetBridge(string callbackString)
        {
            var brigeCallbackDelegate = (GetBridgeCallback)Marshal.GetDelegateForFunctionPointer(new IntPtr(Convert.ToInt64(callbackString)), typeof(GetBridgeCallback));
            return brigeCallbackDelegate(Marshal.GetComInterfaceForObject(new ClrBridge(), typeof(IClrBridge)));
        }
    }

    class Delegate_Wrapper<TRet, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
    {
        Func<object[], object> _Invoked;
        public Delegate_Wrapper(Func<object[], object> callback) { _Invoked = callback; }
        public void Action_0() { _Invoked(new object[] { }); }
        public void Action_1(TArg1 arg1) { _Invoked(new object[] { arg1 }); }
        public void Action_2(TArg1 arg1, TArg2 arg2) { _Invoked(new object[] { arg1, arg2 }); }
        public void Action_3(TArg1 arg1, TArg2 arg2, TArg3 arg3) { _Invoked(new object[] { arg1, arg2, arg3 }); }
        public void Action_4(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) { _Invoked(new object[] { arg1, arg2, arg3, arg4 }); }
        public void Action_5(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) { _Invoked(new object[] { arg1, arg2, arg3, arg4, arg5 }); }
        public void Action_6(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) { _Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }); }
        public void Action_7(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) { _Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }); }
        public void Action_8(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) { _Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }); }
        public void Action_9(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9) { _Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }); }
        public void Action_10(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10) { _Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }); }
        public TRet Func_0() { return (TRet)_Invoked(new object[] { }); }
        public TRet Func_1(TArg1 arg1) { return (TRet)_Invoked(new object[] { arg1 }); }
        public TRet Func_2(TArg1 arg1, TArg2 arg2) { return (TRet)_Invoked(new object[] { arg1, arg2 }); }
        public TRet Func_3(TArg1 arg1, TArg2 arg2, TArg3 arg3) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3 }); }
        public TRet Func_4(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4 }); }
        public TRet Func_5(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4, arg5 }); }
        public TRet Func_6(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }); }
        public TRet Func_7(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }); }
        public TRet Func_8(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }); }
        public TRet Func_9(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }); }
        public TRet Func_10(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10) { return (TRet)_Invoked(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }); }
    }
    class Delegate_Wrapper
    {
        public static Delegate Create(Func<object[], object> callback, Type type)
        {
            var invokeMethod = type.GetMethod("Invoke");
            List<Type> GenericTypes = new List<Type>();
            GenericTypes.Add(invokeMethod.ReturnType == typeof(void) ? typeof(object) : invokeMethod.ReturnType);
            GenericTypes.AddRange(invokeMethod.GetParameters().Select(p => p.ParameterType));
            while (GenericTypes.Count < 11) { GenericTypes.Add(typeof(object)); }

            var delegateWrappedInstance = Activator.CreateInstance(typeof(Delegate_Wrapper<,,,,,,,,,,>).MakeGenericType(GenericTypes.ToArray()), new object[] { callback });
            var delegateWrappedMethod = delegateWrappedInstance.GetType().GetMethod(
                (invokeMethod.ReturnType == typeof(void) ? "Action_" : "Func_") + invokeMethod.GetParameters().Length);
            return delegateWrappedMethod.CreateDelegate(type, delegateWrappedInstance);
        }
    }

    #region Structures
    [DataContract(Namespace ="")]
    class OBJECT
    {
        [DataMember]public int __OBJECT = 0x77;
        [DataMember]public int Id;
    }
    [DataContract(Namespace = "")]
    class ERROR
    {
        [DataMember]public int __ERROR = 0x99;
        [DataMember]public string Message;
        [DataMember]public string Stack;
    }
    [DataContract(Namespace = "")]
    class MethodInfo
    {
        [DataMember]public string Name;
        [DataMember]public string[] Parameters;
    }
    [DataContract(Namespace = "")]
    class TypeInfo
    {
        [DataMember]public string TypeName;
        [DataMember]public bool IsDelegate;
        [DataMember]public bool IsEnum;
        [DataMember]public MethodInfo[] Methods;
        [DataMember]public string[] Fields;
        [DataMember]public object EnumValue;
        [DataMember]public string[] NestedTypes;
    }
    [DataContract(Namespace = "")]
    class NamespaceInfo
    {
        [DataMember]public string Name;
        [DataMember]public bool IsType;
    }
    #endregion

    public class ClrBridge : IClrBridge
    {
        public string CreateObject(string typeName, string args)
        {
            return NoThrowBoundary(() => DehydrateResult(Activator.CreateInstance(FindTypeByName(typeName), HydrateArguments(args))));
        }

        public string ReleaseObject(string objRef)
        {
            return NoThrowBoundary(() => _objects.Remove(JsonToObject<OBJECT>(objRef).Id));
        }

        public string DescribeObject(string typeName, string objRef)
        {
            return NoThrowBoundary(() =>
            {
                Type type;
                object instance = null;
                if (string.IsNullOrWhiteSpace(objRef))
                {
                    type = FindTypeByName(typeName);
                }
                else
                {
                    instance = ObjectRefToObject(JsonToObject<OBJECT>(objRef));
                    type = instance.GetType();
                }

                return new TypeInfo
                {
                    TypeName = type.FullName,
                    IsEnum = type.GetTypeInfo().IsEnum,
                    IsDelegate = typeof(MulticastDelegate).IsAssignableFrom(type.GetTypeInfo().BaseType),
                    NestedTypes = type.GetNestedTypes(BindingFlags.Public).Select(t => t.FullName).ToArray(),
                    Fields = type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public).Select(f => f.Name).ToArray(),
                    Methods = type.GetMethods().Select(m =>
                        new MethodInfo
                        {
                            Name = m.Name,
                            Parameters = m.GetParameters().Select(p => p.Name).ToArray(),
                        }).ToArray(),
                    EnumValue = (type.GetTypeInfo().IsEnum && instance != null) ? Convert.ChangeType(instance, Enum.GetUnderlyingType(instance.GetType())) : 0,
                };
            });
        }

        delegate IntPtr JsonDelegate([MarshalAs(UnmanagedType.LPWStr)]string args);
        public string CreateDelegate(string typeName, IntPtr callback)
        {
            return NoThrowBoundary(() =>
            {
                return DehydrateResult(Delegate_Wrapper.Create((args) =>
                {
                    var jsonCallback = (JsonDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(JsonDelegate));
                    var json_args = ObjectToJson(args.Select(a => DehydrateResult(a)).ToArray());
                    var ret = JsonToObject<object>(Marshal.PtrToStringUni(jsonCallback(json_args)));
                    if (ret != null && ret.GetType() == typeof(OBJECT))
                    {
                        ret = ObjectRefToObject((OBJECT)ret);
                    }
                    return ret;
                }, FindTypeByName(typeName)));
            });
        }

        public string InvokeMethod(string objRef, string typeName, string methodName, string args, string genericTypesRef, int box)
        {
            bool returnBoxed = box == 1;
            return NoThrowBoundary(() =>
            {
                Type type;
                object instance = null;
                if (!string.IsNullOrWhiteSpace(objRef))
                {
                    instance = ObjectRefToObject(JsonToObject<OBJECT>(objRef));
                    type = instance.GetType();
                }
                else
                {
                    type = FindTypeByName(typeName);
                }

                var parameters = HydrateArguments(args);
                var method = type.GetMethod(methodName, parameters.Select(s => s.GetType()).ToArray());
                if (method == null) // Fuzzy match for types that will cast.
                {
                    method = type.GetMethods().FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == parameters.Length);
                }

                if (method != null)
                {
                    if (!string.IsNullOrWhiteSpace(genericTypesRef))
                    {
                        method = method.MakeGenericMethod((Type[])ObjectRefToObject(JsonToObject<OBJECT>(genericTypesRef)));
                    }

                    var resolvedArgs = HydrateArguments(args);
                    var refMap = resolvedArgs.Select(o => ObjetToObjectRef(o)).ToArray();
                    var rawRet = method.Invoke(instance, resolvedArgs);
                    // 'out' and 'ref' params must be picked up if they changed.
                    for (var i = 0; i < refMap.Length; ++i)
                    {
                        if (refMap[i] > -1) { _objects[refMap[i]] = resolvedArgs[i]; }
                    }
                    return DehydrateResult(rawRet, returnBoxed);
                }
                else
                {
                    var property = type.GetField(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        if (parameters.Length == 0)
                        {
                            return DehydrateResult(property.GetValue(instance), returnBoxed);
                        }
                        else if (parameters.Length == 1)
                        {
                            property.SetValue(instance, parameters[0]);
                            return null;
                        }
                    }
                    throw new InvalidOperationException("ClrBridge.Call: Didn't find method or field: m:" + methodName + " o:" + objRef + " i:" + instance + " a:" + args);
                }
            });
        }

        public string DescribeNamespace(string nameSpace)
        {
            return NoThrowBoundary(() =>
            {
                var ret = new List<NamespaceInfo>();
                var matchNamespaces = new HashSet<string>();
                foreach (var type in GetLoadedAssemblies().SelectMany(a => a.GetTypes()))
                {
                    if (type.FullName.Contains("<") || type.FullName.Contains("+")) { continue; };

                    if (type.FullName.StartsWith(nameSpace + "."))
                    {
                        if (type.FullName.LastIndexOf('.') == nameSpace.Length)
                        {
                            ret.Add(new NamespaceInfo { Name = type.Name, IsType = true });
                        }
                        else
                        {
                            matchNamespaces.Add(type.FullName.Substring(nameSpace.Length + 1, 
                                type.FullName.Substring(nameSpace.Length + 1).IndexOf(".")));
                        }
                    }
                }
                ret.AddRange(matchNamespaces.Select(namespaceName => new NamespaceInfo { Name = namespaceName, IsType = false }));
                return ret;
            });
        }

        static int _object_last = 0;
        static Dictionary<int, object> _objects = new Dictionary<int, object>();

        int ObjetToObjectRef(object o) { return _objects.ContainsValue(o) ? _objects.FirstOrDefault(x => x.Value == o).Key : -1; }
        object ObjectRefToObject(OBJECT o) { return _objects[o.Id]; }

        string NoThrowBoundary(Func<object> invoke)
        {
            try
            {
                return ObjectToJson(invoke());
            }
            catch (Exception ex)
            {
                ex = ex.InnerException != null ? ex.InnerException : ex;
                return ObjectToJson(new ERROR { Message = ex.Message, Stack = ex.StackTrace });
            }
        }

        #region Serialization
        DataContractJsonSerializer GetBridgeSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, new DataContractJsonSerializerSettings
            {
                EmitTypeInformation = EmitTypeInformation.Always,
                KnownTypes = new Type[] { typeof(OBJECT), typeof(ERROR), typeof(MethodInfo), typeof(TypeInfo), typeof(NamespaceInfo) },
            });
        }

        T JsonToObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) { return default(T); }
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)GetBridgeSerializer(typeof(T)).ReadObject(ms);
            }
        }

        string ObjectToJson(object o)
        {
            if (o == null) { return "null"; }
            using (var ms = new MemoryStream())
            {
                GetBridgeSerializer(o.GetType()).WriteObject(ms, o);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }

        object[] HydrateArguments(string args)
        {
            var resolved = JsonToObject<object[]>(args);
            for (var i = 0; i < resolved.Length; ++i)
            {
                if (resolved[i].GetType() == typeof(OBJECT))
                {
                    resolved[i] = ObjectRefToObject((OBJECT)resolved[i]);
                }
                else if (resolved[i].GetType() == typeof(decimal))
                {
                    resolved[i] = double.Parse(resolved[i].ToString());
                }
            }
            return resolved;
        }

        object DehydrateResult(object output, bool returnOnlyObjects = false)
        {
            if (output == null || (!returnOnlyObjects && !output.GetType().GetTypeInfo().IsEnum && 
                (output.GetType().GetTypeInfo().IsPrimitive || output.GetType() == typeof(string))))
            {
                return output;
            }
            else
            {
                lock (_objects)
                {
                    var objId = ++_object_last;
                    _objects.Add(objId, output);
                    return new OBJECT { Id = objId };
                }
            }
        }
        #endregion

        IEnumerable<Assembly> GetLoadedAssemblies()
        {
#if NETFX_CORE
            return new Assembly[] { typeof(System.String).GetTypeInfo().Assembly };
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        Type FindTypeByName(string typeName)
        {
            Type type = GetLoadedAssemblies().Select(a => a.GetType(typeName)).FirstOrDefault(t => t != null);
            if (type == null)
            {
                throw new ApplicationException("Can't resolve typeName: " + typeName);
            }
            return type;
        }
    }
}