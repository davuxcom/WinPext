
require('resume.js').invoke();

var ThrowIfFailed = COM.ThrowIfFailed;

var ICoreImmersiveApplication = new COM.Interface(COM.IInspectable, {
    get_MainView: [2, ['pointer']],
}, "1ADA0E3E-E4A2-4123-B451-DC96BF800419");

var ICoreApplicationView = new COM.Interface(COM.IInspectable, {
	get_CoreWindow: [0, ['pointer']],
}, "638BB2DB-451D-4661-B099-414F34FFB9F1");

var ICoreWindow = new COM.Interface(COM.IInspectable, {
	get_Dispatcher: [3, ['pointer']],
    GetAsyncKeyState: [14, ['int','pointer']],
    GetKeyState: [15, ['int', 'pointer']],
    add_KeyDown: [29, ['pointer', 'pointer']],
    remove_KeyDown: [30, ['int64']],
}, "79B9D5F2-879E-4B89-B798-79E47598030C");

var CoreDispatcherPriority = {
	Normal: 0,
}
var ICoreDispatcher = new COM.Interface(COM.IInspectable, {
	RunAsync: [2, ['int', 'pointer', 'pointer']],
}, "60DB2FA8-B705-4FDE-A7D6-EBBB1891D39E");

var IDispatchedHandler = new COM.Interface(COM.IUnknown, {
	Invoke: [0, []],
}, "D1F276C4-98D8-4636-BF49-EB79507548E9");

var IApplicationViewStatics2 = new COM.Interface(COM.IInspectable, {
	GetForCurrentView: [0, ['pointer']],
}, "AF338AE5-CF64-423C-85E5-F3E72448FB23");

var IApplicationView = new COM.Interface(COM.IInspectable, {
	put_Title: [7, ['pointer']],
}, "D222D519-4361-451E-96C4-60F4F9742DB0");

var IApplicationView2 = new COM.Interface(COM.IInspectable, {
    add_VisibleBoundsChanged: [3, ['pointer','pointer']]
}, "E876B196-A545-40DC-B594-450CBA68CC00");

var IApplicationView3 = new COM.Interface(COM.IInspectable, {
	TryEnterFullScreenMode: [4, ['pointer']],
	ExitFullScreenMode: [5, []],
}, "903C9CE5-793A-4FDF-A2B2-AF1AC21E3108");

function GetMainXamlWindow() {
	var coreApplication = WinRT.GetActivationFactory("Windows.ApplicationModel.Core.CoreApplication", ICoreImmersiveApplication);
	var mainView = new COM.Pointer(ICoreApplicationView);
	ThrowIfFailed(coreApplication.get_MainView(mainView.GetAddressOf()));
	var coreWindow = new COM.Pointer(ICoreWindow);
	ThrowIfFailed(mainView.get_CoreWindow(coreWindow.GetAddressOf()));
	return coreWindow;
}

function RunOnXAMLUIThread(callback) {
	var coreWindow = GetMainXamlWindow();
	var coreDispatcher = new COM.Pointer(ICoreDispatcher);
	ThrowIfFailed(coreWindow.get_Dispatcher(coreDispatcher.GetAddressOf()));

	// Build a callback object.
	var dispatcherFrame = new COM.RuntimeObject(IDispatchedHandler.IID);
	// HRESULT IDispatchedHandler.Invoke(void);
	dispatcherFrame.AddEntry(function (this_ptr) { callback(); return COM.S_OK; }, 'uint', ['pointer']);

	ThrowIfFailed(coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, dispatcherFrame.GetAddress(), Memory.alloc(Process.pointerSize)));
}

// main
setTimeout(function() {
	WinRT.Initialize();
	
	RunOnXAMLUIThread(function () {
        var coreWindow = GetMainXamlWindow();
		var appViewStatics = WinRT.GetActivationFactory("Windows.UI.ViewManagement.ApplicationView", IApplicationViewStatics2);
		var appView = new COM.Pointer(IApplicationView);
		ThrowIfFailed(appViewStatics.GetForCurrentView(appView.GetAddressOf()));
        var appView3 = appView.As(IApplicationView3);
		ThrowIfFailed(appView3.ExitFullScreenMode());
	});
}, 2000); // Startup delay - avoid splash screen