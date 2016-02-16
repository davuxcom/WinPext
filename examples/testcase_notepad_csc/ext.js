
buffer = [];
test_count = 0;
function VERIFY_IS_EQUAL(expected, actual) {
    buffer.push("Verify: " + expected + " " + actual);
    if (actual != expected) {
        for(var i = 0; i < buffer.length; ++i) {
            console.log(buffer[i]);
        }
        throw Error("Verify Failed\nExpected: " + expected + "\nActual: " + actual);
    }
    test_count++;
}

CLR.AddNamespace("System");

try {

CLR.EnableTraceListener();
System.Threading.Thread.Sleep(50);

System.Reflection.Assembly.LoadFile(HostInfo.UserLibraryPath);
CLR.AddNamespace("UserLibrary");

// Method
VERIFY_IS_EQUAL(UserLibrary.Test1.TestMethod(), "TestMethod");
// Method<T>(T)
VERIFY_IS_EQUAL(UserLibrary.Test1.TestGenericMethod.Of(System.IO.FileInfo)(new System.IO.FileInfo("f")),
 "System.IO.FileInfo");
// Method<T,V>(T, V)
VERIFY_IS_EQUAL(UserLibrary.Test1.TestGenericMethod.Of(System.IO.FileInfo, System.Text.StringBuilder)(new System.IO.FileInfo("f"), new System.Text.StringBuilder()),
 "System.IO.FileInfo System.Text.StringBuilder");
 // Generic+Boxed
VERIFY_IS_EQUAL(UserLibrary.Test1.TestGenericMethod.Of(System.IO.FileInfo, System.Text.StringBuilder).Box(new System.IO.FileInfo("f"), new System.Text.StringBuilder()).ToString(),
 "System.IO.FileInfo System.Text.StringBuilder");

// Static and instance fields
 
VERIFY_IS_EQUAL(UserLibrary.Test1.Static5, 5);
UserLibrary.Test1.Static5 = 7;
VERIFY_IS_EQUAL(UserLibrary.Test1.Static5, 7);
var test1 = new UserLibrary.Test1();
VERIFY_IS_EQUAL(test1.Instance5, 5);
test1.Instance5 = 4;
VERIFY_IS_EQUAL(test1.Instance5, 4);

// nested classes
var nested = new UserLibrary.Test1.NestedClass();
VERIFY_IS_EQUAL(nested.Instance5, "5");

var twiceNested = new UserLibrary.Test1.NestedClass.TwiceNestedClass();
VERIFY_IS_EQUAL(twiceNested.Instance7, "7");


// Boxing
VERIFY_IS_EQUAL(new System.Byte.Parse.Box("10").ToString(), "10");

// Test indexer
var dict = new System.Collections.Generic.Dictionary.Of(System.String, System.String)();
dict.Add("One", "OneValue");
VERIFY_IS_EQUAL(dict.get_Item("One"), "OneValue");
dict.set_Item("Two", "NewTwo")
VERIFY_IS_EQUAL(dict.get_Item("Two"), "NewTwo");

// further dictionary sanity check
VERIFY_IS_EQUAL(dict.Keys.Count, 2);
var dict_enum = dict.Values.GetEnumerator();
dict_enum.MoveNext();
dict_enum.MoveNext();
dict_enum.MoveNext();
VERIFY_IS_EQUAL(dict_enum.Current, null);

// Test property
var p = new System.Diagnostics.ProcessStartInfo();
p.Arguments = "testargs";
VERIFY_IS_EQUAL(p.Arguments, "testargs");

// Delegates

// Action
var didAct = false;
var act = new System.Action(function() {didAct=true;});
VERIFY_IS_EQUAL(act.Invoke() ? didAct : didAct, true);
 
didAct = false;
var act = new System.Action.Of(System.Boolean)(function(b) {didAct=true;});
VERIFY_IS_EQUAL(act.Invoke(true) ? didAct : didAct, true);

// Func<String,bool>
var fn = new System.Func.Of(System.String, System.Boolean)(function(str) { return true; });
VERIFY_IS_EQUAL(fn.Invoke("foo"), true);
// Func<bool,String>
var fn2 = new System.Func.Of(System.Boolean, System.String)(function(str) { return "foo"; });
VERIFY_IS_EQUAL(fn2 .Invoke(true), "foo");
// Func<String, FileInfo>
var fn3 = new System.Func.Of(System.String, System.IO.FileInfo)(function(str) {  return new System.IO.FileInfo(str); });
VERIFY_IS_EQUAL(fn3.Invoke("file_test.txt").Name, "file_test.txt");
// Func<FileInfo, String>
var fn4 = new System.Func.Of(System.IO.FileInfo, System.String)(function(fi) { return fi.Name;});
VERIFY_IS_EQUAL(fn4.Invoke(System.IO.FileInfo("file_test2.txt")), "file_test2.txt");

// Events

// Register
var asmLoaded = false;
var eventToken = System.AppDomain.CurrentDomain.AssemblyLoad += new System.AssemblyLoadEventHandler(function (s, e) { asmLoaded = true;});
System.Reflection.Assembly.LoadWithPartialName("PresentationFramework");
VERIFY_IS_EQUAL(asmLoaded, true);
// Unregister
asmLoaded = false;
System.AppDomain.CurrentDomain.AssemblyLoad.remove(eventToken);
System.Reflection.Assembly.LoadWithPartialName("System.Windows.Forms");
VERIFY_IS_EQUAL(asmLoaded, false); // will have generated loads

// Scenario test: thread
var apt = null;
var uiThread = new System.Threading.Thread(new System.Threading.ThreadStart(function() {
    apt = System.Threading.Thread.CurrentThread.ApartmentState;
}));
uiThread.SetApartmentState(System.Threading.ApartmentState.STA);
uiThread.Start();
uiThread.Join();
VERIFY_IS_EQUAL(apt, "STA");
VERIFY_IS_EQUAL(System.Threading.ApartmentState.STA, "STA");

// Verify that System.Byte is auto-casted to an object instance (i.e. implicit typeof())
var arr = System.Array.CreateInstance(System.Byte, 10);
// Verify that a boxed value can be used for byte, since otherwise we fail to downcast from int.
arr.SetValue(System.Byte.Parse.Box("10"),0);

var Registry = CLR.GetNamespace("Microsoft").Win32.Registry;

// Field
var sn = Registry.CurrentUser.OpenSubKey("Software").GetSubKeyNames();

// Ref param
var i = System.Int32.Parse.Box("5");
UserLibrary.Test1.TestRef(i);
VERIFY_IS_EQUAL(i.ToString(), 10);
// out param
var io = System.Int32.Parse.Box("5");
UserLibrary.Test1.TestOut(io);
VERIFY_IS_EQUAL(io.ToString(), 10);
// verify pinning

CLR.Pin(io);
console.log("GC: " + CLR.Prune());
var objectNotFound = false;
try {
i.ToString();
} catch(e) {
    objectNotFound = true;
}
io.ToString();
VERIFY_IS_EQUAL(objectNotFound, true);

} catch (e) {
    VERIFY_IS_EQUAL(e, null);
}
console.log("");
console.log("SUCCESS: " + test_count + " tests run.");