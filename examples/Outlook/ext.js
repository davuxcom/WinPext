// @import windows/platform

var TypeMap = {
    'pointer': [Process.pointerSize, Memory.readPointer, Memory.writePointer],
    'char': [1, Memory.readS8, Memory.writeS8],
    'uchar': [1, Memory.readU8, Memory.writeU8],
    'int8': [1, Memory.readS8, Memory.writeS8],
    'uint8': [1, Memory.readU8, Memory.writeU8],
    'int16': [2, Memory.readS16, Memory.writeS16],
    'uint16': [2, Memory.readU16, Memory.writeU16],
    'int': [4, Memory.readS32, Memory.writeS32],
    'uint': [4, Memory.readU32, Memory.writeU32],
    'int32': [4, Memory.readS32, Memory.writeS32],
    'uint32': [4, Memory.readU32, Memory.writeU32],
    'long': [4, Memory.readS32, Memory.writeS32],
    'ulong': [4, Memory.readU32, Memory.writeU32],
    'float': [4, Memory.readFloat, Memory.writeFloat],
    'double': [8, Memory.readDouble, Memory.writeDouble],
    'int64': [8, Memory.readS64, Memory.writeS64],
    'uint64': [8, Memory.readU64, Memory.writeU64],
};

var Struct = function (structInfo) {
    var base_ptr = null;
    var base_ptr_size = 0;
    // This is a container for objects returned from the type-set function
    // If the type-set function needs to allocate memory, this space stores it
    // thus tying it to the lifetime of this object (or until replaced by type-set fn).
    var object_cache = {};

    this.Get = function () { return base_ptr; }
    Object.defineProperty(this, "Size", { get: function () { return base_ptr_size; } });

    function LookupType(stringType) {
        for (var type in TypeMap) {
            if (stringType == type) { return TypeMap[type]; }
        }
        throw new Error("Didn't find " + JSON.stringify(stringType) + " in TypeMap");
    }

    function SizeOfType(stringType) { return LookupType(stringType)[0]; }
    function ResolveAddress(offset) { return base_ptr.add(offset); }

    function CreateGetterSetter(self, name, type, offset) {
        Object.defineProperty(self, name, {
            get: function () { return LookupType(type)[1](ResolveAddress(offset)); },
            set: function (newValue) {
                // If the type-set function returns something, hold onto it.
                // This is used to tie resource lifetime to this object.
                object_cache[name] = LookupType(type)[2](ResolveAddress(offset), newValue);
            }
        });
    };

    for (var member in structInfo) {
        if (member == "union") {
            var union_size = 0;
            var union = structInfo[member];
            for (var union_member in union) {
                var union_member_type = union[union_member];
                var union_member_size = SizeOfType(union_member_type);
                if (union_size < union_member_size) {
                    union_size = union_member_size;
                }
                CreateGetterSetter(this, union_member, union_member_type, base_ptr_size);
            }
            base_ptr_size += union_size;
        } else {
            var member_size = SizeOfType(structInfo[member]);
            CreateGetterSetter(this, member, structInfo[member], base_ptr_size);
            base_ptr_size += member_size;
        }
    }
    base_ptr = Memory.alloc(base_ptr_size);
}

// Add some custom types. [size, readFunc, writeFunc]
TypeMap['lpwstr'] = [Process.pointerSize, 
    (addr) => { return Memory.readUtf16String(Memory.readPointer(addr)); },     
    (addr, newValue) => { 
        var stringRef = Memory.allocUtf16String(newValue);
        Memory.writePointer(addr, stringRef);
        return stringRef; // tied to object lifetime.
    }
];
TypeMap['guid'] = [16, 
    Win32.GUID.read, 
    (addr, newValue) => { Memory.copy(addr, Win32.GUID.alloc(newValue), 16); }
];

var Shell32 = {
    SHGetPropertyStoreForWindow: new NativeFunction(Module.findExportByName("shell32.dll", "SHGetPropertyStoreForWindow"), 'uint', ['int','pointer', 'pointer']),
};

var VT_LPWSTR = 31;
var PROPVARIANT = {
    vt: 'uint16',
    reserved1: 'uchar',
    reserved2: 'uchar',
    reserved3: 'ulong',
    union: {
        intVal: 'int',
        pwszVal: 'lpwstr',
    },
    extra: 'ulong'
};

var PROPKEY = {
    fmtid: 'guid',
    pid: 'ulong'
}

var IPropertyStore = new COM.Interface(COM.IUnknown, {
    // HRESULT SetValue([in] REFPROPERTYKEY key, [in] REFPROPVARIANT propvar);
    SetValue: [3, ['pointer', 'pointer']],
}, "886d8eeb-8cf2-4446-8d02-cdba1dbdcf99");

function SetAppIdForWindow(hwnd, appId) {
    var propStore = new COM.Pointer(IPropertyStore);
    COM.ThrowIfFailed(Shell32.SHGetPropertyStoreForWindow(hwnd, IPropertyStore.IID, propStore.GetAddressOf()));

    var PKEY_AppUserModel_Id = new Struct(PROPKEY);
    PKEY_AppUserModel_Id.fmtid = "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3";
    PKEY_AppUserModel_Id.pid = 5;

    var propVar = new Struct(PROPVARIANT);
    propVar.vt = VT_LPWSTR;
    propVar.pwszVal = appId;
    console.log(propVar.pwszVal);
    console.log(propVar.intVal);

    COM.ThrowIfFailed(propStore.SetValue(PKEY_AppUserModel_Id.Get(), propVar.Get()));
}

CLR.AddNamespace("System");

setTimeout(function() {
    function CheckForMainWindow() {
        var hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.value;
        if (hwnd > 0) {
            SetAppIdForWindow(hwnd, "Notepad.2");
        } else {
            setTimeout(CheckForMainWindow, 1);
        }
    }
    CheckForMainWindow();
},5000);