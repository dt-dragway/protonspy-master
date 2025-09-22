using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Antiufo.Controls;
using FFmpeg.AutoGen;
using iSpyApplication.Cloud;
using iSpyApplication.Controls;
using iSpyApplication.Joystick;
using iSpyApplication.Onvif;
using iSpyApplication.Properties;
using iSpyApplication.Server;
using iSpyApplication.Sources;
using iSpyApplication.Sources.Audio;
using iSpyApplication.Sources.Audio.talk;
using iSpyApplication.Utilities;
using Microsoft.Win32;
using NATUPNPLib;
using NAudio.Wave;
using NETWORKLIST;
using SharpDX.DirectInput;
using PictureBox = iSpyApplication.Controls.PictureBox;
using Timer = System.Timers.Timer;

namespace iSpyApplication
{
    /// <summary>
    ///     Summary description for MainForm
    /// </summary>
    public partial class MainForm : MetroFramework.Forms.MetroForm, INetworkListManagerEvents
    {
        public const string VLCx86 = "https://www.videolan.org/vlc/download-windows.html";
        public const string VLCx64 = "https://download.videolan.org/pub/videolan/vlc/last/win64/";

        public const string Website = "https://www.ispyconnect.com";
        public const string ContentSource = "https://ispyrtcdata.blob.core.windows.net/downloads/";
        public static bool NeedsSync;
        private static DateTime _needsMediaRefresh = DateTime.MinValue;
        //private static Player _player = null;

        public static DateTime LastAlert = DateTime.MinValue;

        public static MainForm InstanceReference;

        public static bool VLCRepeatAll;
        public static bool NeedsMediaRebuild = false;
        public static int MediaPanelPage;
        public static bool LoopBack;
        public static string NL = Environment.NewLine;
        public static Font Drawfont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);
        public static Font DrawfontBig = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Regular, GraphicsUnit.Pixel);
        public static Font DrawfontMed = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Regular, GraphicsUnit.Pixel);
        public static Font Iconfont = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold, GraphicsUnit.Pixel);
        public static Brush IconBrush = new SolidBrush(Color.White);
        public static Brush IconBrushOff = new SolidBrush(Color.FromArgb(64, 255, 255, 255));
        public static Brush IconBrushActive = new SolidBrush(Color.Red);
        public static Brush OverlayBrush = new SolidBrush(Color.White);
        public static int ThreadKillDelay = 10000;
        public static SolidBrush CameraDrawBrush = new SolidBrush(Color.White);
        public static Pen CameraLine = new Pen(Color.Green, 2);
        public static Pen CameraNav = new Pen(Color.White, 1);
        public static Brush RecordBrush = new SolidBrush(Color.Red);
        public static Brush OverlayBackgroundBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        
        public static string Identifier;
        public static DataTable IPTABLE;
        public static bool IPLISTED = true;
        public static bool IPRTSP = false, IPHTTP = true;
        public static string IPADDR = "";
        public static string IPCHANNEL = "0";
        public static string IPMODEL = "";
        public static string IPUN = "";
        public static string IPPORTS = "80,8080";
        public static int IPPORT = 80;
        public static string IPPASS = "";
        public static string IPTYPE = "";
        public static int Affiliateid = 0;
        public static string EmailAddress = "", MobileNumber = "";
        public static string Group="";
        public static float CpuUsage, CpuTotal;
        public static bool HighCPU;
        public static int RecordingThreads;
        public static List<string> Plugins = new List<string>();
        public static bool NeedsResourceUpdate;
        private static readonly List<FilePreview> Masterfilelist = new List<FilePreview>();

        public static EncoderParameters EncoderParams;
        public static bool ShuttingDown = false;
        public static string Webserver = Website;
        public static string WebserverSecure = Website.Replace("http:", "https:");


        public static Rectangle RPower = new Rectangle(94, 3, 16, 16);
        public static Rectangle RPowerOn = new Rectangle(94, 43, 16, 16);
        public static Rectangle RPowerOff = new Rectangle(94, 83, 16, 16);
        public static Rectangle RAdd = new Rectangle(127, 3, 16, 16);
        public static Rectangle RAddOff = new Rectangle(127, 83, 16, 16);
        public static Rectangle REdit = new Rectangle(3, 2, 16, 16);
        public static Rectangle REditOff = new Rectangle(3, 82, 16, 16);
        public static Rectangle RHold = new Rectangle(255, 2, 16, 16);
        public static Rectangle RHoldOn = new Rectangle(284, 42, 16, 16);
        public static Rectangle RHoldOff = new Rectangle(284, 82, 16, 16);
        public static Rectangle RRecord = new Rectangle(188,2,16,16);
        public static Rectangle RRecordOn = new Rectangle(188, 42, 16, 16);
        public static Rectangle RRecordOff = new Rectangle(188, 82, 16, 16);
        public static Rectangle RNext = new Rectangle(65, 3, 16, 16);
        public static Rectangle RNextOff = new Rectangle(65, 82, 16, 16);
        public static Rectangle RGrab = new Rectangle(157,2,16,16);
        public static Rectangle RGrabOff = new Rectangle(157, 82, 16, 16);
        public static Rectangle RTalk = new Rectangle(313, 2, 16,16);
        public static Rectangle RTalkOn = new Rectangle(313, 42, 16, 16);
        public static Rectangle RTalkOff = new Rectangle(313, 82, 16, 16);
        public static Rectangle RFiles = new Rectangle(223,3,16,16);
        public static Rectangle RFilesOff = new Rectangle(223, 83, 16, 16);
        public static Rectangle RListen = new Rectangle(347,2,16,16);
        public static Rectangle RListenOn = new Rectangle(380,43,16,16);
        public static Rectangle RListenOff = new Rectangle(347, 83, 16, 16);
        public static Rectangle RWeb = new Rectangle(411, 3, 16, 16);
        public static Rectangle RWebOff = new Rectangle(411, 83, 16, 16);
        public static Rectangle RText = new Rectangle(443, 3, 16, 16);
        public static Rectangle RTextOff = new Rectangle(443, 83, 16, 16);
        public static Rectangle RFolder = new Rectangle(473, 3, 16, 16);
        public static Rectangle RFolderOff = new Rectangle(473, 83, 16, 16);

        private static List<string> _tags;
        public static bool CustomWebserver;
        public static List<string> Tags
        {
            get
            {
                if (_tags != null)
                    return _tags;
                _tags = new List<string>();
                if (!string.IsNullOrEmpty(Conf.Tags))
                {
                    var l = Conf.Tags.Split(',').ToList();
                    foreach (var t in l)
                    {
                        if (!string.IsNullOrEmpty(t))
                        {
                            var s = t.Trim();
                            if (!s.StartsWith("{"))
                                s = "{" + s;
                            if (!s.EndsWith("}"))
                                s = s + "}";
                            _tags.Add(s.ToUpper(CultureInfo.InvariantCulture));
                        }
                    }
                }
                return _tags;
            }
            set { _tags = value; }
        } 
        public ISpyControl LastFocussedControl = null;

        internal static LocalServer MWS;

        public static string PurchaseLink = "http://www.ispyconnect.com/astore.aspx";
        private static int _storageCounter;
        private static Timer _rescanIPTimer, _tmrJoystick;
        
        private static string _counters = "";
        private static readonly Random Random = new Random();
        private static ViewController _vc;
        private static int _pingCounter;
        private static ImageCodecInfo _encoder;
        private static bool _needsDelete = false;
        

        

        
        
        private static string _browser = string.Empty;

        
        private MenuItem menuItem37;
        private ToolStripMenuItem tagsToolStripMenuItem;
        private MenuItem menuItem38;
        private MenuItem menuItem39;
        private MenuItem menuItem40;
        private MenuItem menuItem34;
        private ToolStripMenuItem openWebInterfaceToolStripMenuItem;
        

        public static void AddObject(object o)
        {
            var c = o as objectsCamera;
            if (c != null)
            {
                c.settings.order = MaxOrderIndex;
                _cameras.Add(c);
                MaxOrderIndex++;
            }
            var m = o as objectsMicrophone;
            if (m != null)
            {
                m.settings.order = MaxOrderIndex;
                _microphones.Add(m);
                MaxOrderIndex++;
            }
            var f = o as objectsFloorplan;
            if (f != null)
            {
                f.order = MaxOrderIndex;
                _floorplans.Add(f);
                MaxOrderIndex++;

            }
            var a = o as objectsActionsEntry;
            if (a != null)
                _actions.Add(a);
            var oc = o as objectsCommand;
            if(oc!=null)
                _remotecommands.Add(oc);
            LayoutPanel.NeedsRedraw = true;
        }

        private static List<PTZSettings2Camera> _ptzs;
        private static List<ManufacturersManufacturer> _sources;
        private static IPAddress[] _ipv4Addresses, _ipv6Addresses;
        
        private readonly List<float> _cpuAverages = new List<float>();
        private readonly int _mCookie = -1;
        private readonly IConnectionPoint _mIcp;
        private static readonly object ThreadLock = new object();

        private readonly FolderSelectDialog _fbdSaveTo = new FolderSelectDialog
                                                        {
                                                            Title = "Select a folder to copy the file to"
                                                        };


        public object ContextTarget;
        //internal Player Player;
        internal PlayerVLC _player;
        public McRemoteControlManager.RemoteControlDevice RemoteManager;
        public bool SilentStartup;
        
        internal CameraWindow TalkCamera;

        private MenuItem _aboutHelpItem;
        private ToolStripMenuItem _addCameraToolStripMenuItem;
        private ToolStripMenuItem _addFloorPlanToolStripMenuItem;
        private ToolStripMenuItem _addMicrophoneToolStripMenuItem;
        private ToolStripMenuItem _applyScheduleToolStripMenuItem;
        private ToolStripMenuItem _applyScheduleToolStripMenuItem1;
        private bool _closing;
        private PerformanceCounter _cpuCounter, _cputotalCounter;
        private ToolStripMenuItem _deleteToolStripMenuItem;
        private ToolStripMenuItem _editToolStripMenuItem;
        private MenuItem _exitFileItem;
        private ToolStripMenuItem _exitToolStripMenuItem;
        private MenuItem _fileItem;
        private ToolStripMenuItem _floorPlanToolStripMenuItem;
        private FileSystemWatcher _fsw;
        private MenuItem _helpItem;
        private ToolStripMenuItem _helpToolstripMenuItem;
        private Timer _houseKeepingTimer;
        private ToolStripMenuItem _iPCameraToolStripMenuItem;
        private static string _lastPath = Program.AppDataPath;
        private static string _currentFileName = "";
        private ToolStripMenuItem _listenToolStripMenuItem;
        private ToolStripMenuItem _localCameraToolStripMenuItem;
        private PersistWindowState _mWindowState;
        private MenuItem _menuItem1;
        private MenuItem _menuItem10;
        private MenuItem _menuItem12;
        private MenuItem _menuItem13;
        private MenuItem _menuItem14;
        private MenuItem _menuItem15;
        private MenuItem _menuItem16;
        private MenuItem _menuItem17;
        private MenuItem _menuItem18;
        private MenuItem _menuItem19;
        private MenuItem _menuItem2;
        private MenuItem _menuItem20;
        private MenuItem _menuItem21;
        private MenuItem _menuItem22;
        private MenuItem _menuItem23;
        private MenuItem _menuItem24;
        private MenuItem _menuItem25;
        private MenuItem _menuItem26;
        private MenuItem _menuItem27;
        private MenuItem _menuItem28;
        private MenuItem _menuItem29;
        private MenuItem _menuItem3;
        private MenuItem _menuItem30;
        private MenuItem _menuItem31;
        private MenuItem _menuItem32;
        private MenuItem _menuItem33;
        private MenuItem _menuItem34;
        private MenuItem _menuItem35;
        private MenuItem _menuItem36;
        private MenuItem _menuItem37;
        private MenuItem _menuItem38;
        private MenuItem _menuItem39;
        private MenuItem _menuItem4;
        private MenuItem _menuItem5;
        private MenuItem _menuItem6;
        private MenuItem _menuItem7;
        private MenuItem _menuItem8;
        private MenuItem _menuItem9;
        private MenuItem _miApplySchedule;
        private MenuItem _miOffAll;
        private MenuItem _miOffSched;
        private MenuItem _miOnAll;
        private MenuItem _miOnSched;
        private ToolStripMenuItem _microphoneToolStripMenuItem;
        private ToolStripMenuItem _onMobileDevicesToolStripMenuItem;
        private PerformanceCounter _pcMem;
        public LayoutPanel _pnlCameras;
        private Panel _pnlContent;
        private ToolStripMenuItem _positionToolStripMenuItem;
        private FormWindowState _previousWindowState = FormWindowState.Normal;
        private PTZTool _ptzTool;
        private PTZCommandButtons _ptzCommandButtons;
        private ToolStripMenuItem _recordNowToolStripMenuItem;
        private ToolStripMenuItem _remoteCommandsToolStripMenuItem;
        private ToolStripMenuItem _resetRecordingCounterToolStripMenuItem;
        private ToolStripMenuItem _resetSizeToolStripMenuItem;
        private ToolStripMenuItem _settingsToolStripMenuItem;
        private ToolStripMenuItem _showFilesToolStripMenuItem;
        private ToolStripMenuItem _showISpy100PercentOpacityToolStripMenuItem;
        private ToolStripMenuItem _showISpy10PercentOpacityToolStripMenuItem;
        private ToolStripMenuItem _showISpy30OpacityToolStripMenuItem;
        private ToolStripMenuItem _showToolstripMenuItem;
        private bool _shuttingDown;
        private string _startCommand = "";
        private Thread _storageThread;
        private ToolStripMenuItem _switchAllOffToolStripMenuItem;
        private ToolStripMenuItem _switchAllOnToolStripMenuItem;
        private ToolStripMenuItem _takePhotoToolStripMenuItem;
        private IAudioSource _talkSource;
        private ITalkTarget _talkTarget;
        private ToolStripMenuItem _thruWebsiteToolStripMenuItem;
        private ToolStripButton _toolStripButton1;
        private ToolStripButton _toolStripButton4;
        private ToolStripButton _toolStripButton8;
        private ToolStripDropDownButton _toolStripDropDownButton1;
        private ToolStripDropDownButton _toolStripDropDownButton2;
        private ToolStripMenuItem _viewMediaToolStripMenuItem;
        private ToolStripStatusLabel _tsslStats;
        private ToolStripMenuItem _uSbCamerasAndMicrophonesOnOtherToolStripMenuItem;
        private ToolStripMenuItem _unlockToolstripMenuItem;
        private Thread _updateChecker;
        private Timer _updateTimer;
        private ToolStripMenuItem _viewMediaOnAMobileDeviceToolStripMenuItem;
        private ToolStripMenuItem _websiteToolstripMenuItem;
        private ToolStripMenuItem alwaysOnTopToolStripMenuItem1;
        private ToolStripMenuItem autoLayoutToolStripMenuItem;
        private IContainer components;
        private ToolStripMenuItem configurePluginToolStripMenuItem;
        private ContextMenuStrip ctxtMainForm;
        private ContextMenuStrip ctxtMnu;
        private ContextMenuStrip ctxtPlayer;
        private ContextMenuStrip ctxtTaskbar;
        private ToolStripMenuItem defaultPlayerToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem displayToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem fileMenuToolStripMenuItem;
        private FlowLayoutPanel flCommands;
        internal MediaPanel flowPreview;
        private ToolStripMenuItem fullScreenToolStripMenuItem;
        private ToolStripMenuItem fullScreenToolStripMenuItem1;
        private ToolStripMenuItem iPCameraWithWizardToolStripMenuItem;
        private ToolStripMenuItem iSpyToolStripMenuItem;
        private ToolStripMenuItem inExplorerToolStripMenuItem;
        private ToolStripMenuItem layoutToolStripMenuItem;
        private MainMenu mainMenu;
        private ToolStripMenuItem mediaPaneToolStripMenuItem;
        private MenuItem menuItem1;
        private MenuItem menuItem10;
        private MenuItem menuItem11;
        private MenuItem menuItem12;
        private MenuItem menuItem14;
        private MenuItem menuItem15;
        private MenuItem menuItem16;
        private MenuItem menuItem17;
        private MenuItem menuItem18;
        private MenuItem menuItem52;
        private MenuItem menuItem19;
        private MenuItem menuItem2;
        private MenuItem menuItem20;
        private MenuItem menuItem21;
        private MenuItem menuItem22;
        private MenuItem menuItem23;
        private MenuItem menuItem24;
        private MenuItem menuItem26;
        private MenuItem menuItem3;
        private MenuItem menuItem4;
        private MenuItem menuItem5;
        private MenuItem menuItem6;
        private MenuItem menuItem7;
        private MenuItem menuItem8;
        private MenuItem menuItem9;
        private MenuItem mnuResetLayout;
        private MenuItem mnuSaveLayout;
        private NotifyIcon notifyIcon1;

        private ToolStripMenuItem oNVIFCameraToolStripMenuItem;
        private ToolStripMenuItem opacityToolStripMenuItem;
        private ToolStripMenuItem opacityToolStripMenuItem1;
        private ToolStripMenuItem opacityToolStripMenuItem2;
        private ToolStripMenuItem opacityToolStripMenuItem3;
        private ToolStripMenuItem otherVideoSourceToolStripMenuItem;
        private ToolStripMenuItem pTZControllerToolStripMenuItem;
        private ToolStripMenuItem pTZControllerToolStripMenuItem1;
        private ToolStripMenuItem pTZCommandButtonsToolStripMenuItem;
        private ToolStripMenuItem pTZToolStripMenuItem;
        private Panel panel2;
        private ToolStripMenuItem pluginCommandsToolStripMenuItem;
        private ToolStripMenuItem resetLayoutToolStripMenuItem1;
        private ToolStripMenuItem saveLayoutToolStripMenuItem1;
        private ToolStripMenuItem saveToToolStripMenuItem;
        private ToolStripMenuItem showInFolderToolStripMenuItem;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ToolStripMenuItem statusBarToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStrip toolStripMenu;
        private ToolStripMenuItem toolStripToolStripMenuItem;
        private ToolTip toolTip1;
        private ToolStripButton tsbPlugins;
        private ToolStripStatusLabel tsslMediaInfo;
        private ToolStripStatusLabel tsslMonitor;
        private ToolStripStatusLabel tsslPerformance;
        private ToolStripMenuItem videoFileToolStripMenuItem;
        private ToolStripMenuItem viewControllerToolStripMenuItem;
        private MenuItem menuItem27;
        private MenuItem menuItem28;
        private ToolStripDropDownButton tssbGridViews;
        private ToolStripMenuItem manageToolStripMenuItem;
        private ToolStripMenuItem archiveToolStripMenuItem;
        private MenuItem menuItem29;
        private MenuItem menuItem30;
        private MenuItem menuItem25;
        private ToolStripMenuItem gridViewsToolStripMenuItem;
        private ToolStripMenuItem maximiseToolStripMenuItem;
        private ToolStripMenuItem uploadToCloudToolStripMenuItem;
        private MediaPanelControl mediaPanelControl1;
        private ToolStripMenuItem viewLogFileToolStripMenuItem;
        private ToolStripMenuItem switchToolStripMenuItem;
        private ToolStripMenuItem alertsOnToolStripMenuItem1;
        private ToolStripMenuItem alertsOffToolStripMenuItem;
        private ToolStripMenuItem scheduleOnToolStripMenuItem;
        private ToolStripMenuItem scheduleOffToolStripMenuItem;
        private ToolStripMenuItem onToolStripMenuItem;
        private ToolStripMenuItem offToolStripMenuItem;
        private ToolStripMenuItem pTZScheduleOnToolStripMenuItem;
        private ToolStripMenuItem pTZScheduleOffToolStripMenuItem;
        private MenuItem menuItem31;
        private MenuItem menuItem32;
        private MenuItem menuItem33;
        private ToolStripMenuItem gridViewsToolStripMenuItem1;
        private ToolStripStatusLabel tsslPRO;
        private MenuItem menuItem35;
        private MenuItem menuItem36;
        private ToolStripMenuItem websiteToolStripMenuItem;

        private MetroFramework.Controls.MetroPanel metroPanelStatus;
        private MetroFramework.Controls.MetroLabel metroLabelStats;
        private MetroFramework.Controls.MetroLabel metroLabelMonitor;
        private MetroFramework.Controls.MetroLabel metroLabelPerformance;
        private MetroFramework.Controls.MetroLabel metroLabelMediaInfo;
        private MetroFramework.Controls.MetroLabel metroLabelPRO;

        public MainForm(bool silent, string command)
        {
            if (Conf.StartupForm != "iSpy")
            {
                SilentStartup = true;
            }

            SilentStartup = SilentStartup || silent || Conf.Enable_Password_Protect || Conf.StartupMode == 1;

            //need to wrap initialize component
            if (SilentStartup)
            {
                ShowInTaskbar = false;
                ShowIcon = false;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                switch (Conf.StartupMode)
                {
                    case 0:
                        _mWindowState = new PersistWindowState {Parent = this, RegistryPath = @"Software\ispy\startup"};
                        break;
                    case 2:
                        WindowState = FormWindowState.Maximized;
                        break;
                    case 3:
                        WindowState = FormWindowState.Maximized;
                        FormBorderStyle = FormBorderStyle.None;
                        break;
                }
            }

            InitializeComponent();
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Style = MetroFramework.MetroColorStyle.Blue;

            this.metroPanelStatus = new MetroFramework.Controls.MetroPanel();
            this.metroPanelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanelStatus.HorizontalScrollbarBarColor = true;
            this.metroPanelStatus.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanelStatus.HorizontalScrollbarSize = 10;
            this.metroPanelStatus.Location = new System.Drawing.Point(0, 533);
            this.metroPanelStatus.Name = "metroPanelStatus";
            this.metroPanelStatus.Size = new System.Drawing.Size(887, 30);
            this.metroPanelStatus.TabIndex = 1;
            this.metroPanelStatus.VerticalScrollbarBarColor = true;
            this.metroPanelStatus.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanelStatus.VerticalScrollbarSize = 10;
            this.metroPanelStatus.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Controls.Add(this.metroPanelStatus);

            this.metroLabelStats = new MetroFramework.Controls.MetroLabel();
            this.metroLabelStats.Name = "metroLabelStats";
            this.metroLabelStats.Text = "Loading...";
            this.metroLabelStats.Location = new System.Drawing.Point(0, 5);
            this.metroLabelStats.Size = new System.Drawing.Size(100, 20);
            this.metroLabelStats.TabIndex = 2;
            this.metroLabelStats.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanelStatus.Controls.Add(this.metroLabelStats);

            this.metroLabelMonitor = new MetroFramework.Controls.MetroLabel();
            this.metroLabelMonitor.Name = "metroLabelMonitor";
            this.metroLabelMonitor.Text = "Monitoring...";
            this.metroLabelMonitor.Location = new System.Drawing.Point(100, 5);
            this.metroLabelMonitor.Size = new System.Drawing.Size(100, 20);
            this.metroLabelMonitor.TabIndex = 3;
            this.metroLabelMonitor.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanelStatus.Controls.Add(this.metroLabelMonitor);

            this.metroLabelPerformance = new MetroFramework.Controls.MetroLabel();
            this.metroLabelPerformance.Name = "metroLabelPerformance";
            this.metroLabelPerformance.Text = "Perf. Tips";
            this.metroLabelPerformance.Location = new System.Drawing.Point(200, 5);
            this.metroLabelPerformance.Size = new System.Drawing.Size(100, 20);
            this.metroLabelPerformance.TabIndex = 4;
            this.metroLabelPerformance.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanelStatus.Controls.Add(this.metroLabelPerformance);

            this.metroLabelMediaInfo = new MetroFramework.Controls.MetroLabel();
            this.metroLabelMediaInfo.Name = "metroLabelMediaInfo";
            this.metroLabelMediaInfo.Text = "";
            this.metroLabelMediaInfo.Location = new System.Drawing.Point(300, 5);
            this.metroLabelMediaInfo.Size = new System.Drawing.Size(100, 20);
            this.metroLabelMediaInfo.TabIndex = 5;
            this.metroLabelMediaInfo.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanelStatus.Controls.Add(this.metroLabelMediaInfo);

            this.metroLabelPRO = new MetroFramework.Controls.MetroLabel();
            this.metroLabelPRO.Name = "metroLabelPRO";
            this.metroLabelPRO.Text = "Try Agent";
            this.metroLabelPRO.Location = new System.Drawing.Point(400, 5);
            this.metroLabelPRO.Size = new System.Drawing.Size(100, 20);
            this.metroLabelPRO.TabIndex = 6;
            this.metroLabelPRO.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanelStatus.Controls.Add(this.metroLabelPRO);

            if (!SilentStartup)
            {
                if (Conf.StartupMode == 0)
                    _mWindowState = new PersistWindowState {Parent = this, RegistryPath = @"Software\ispy\startup"};
            }


            RenderResources();

            _startCommand = command;

            Windows7Renderer r = Windows7Renderer.Instance;
            toolStripMenu.Renderer = r;
            statusStrip1.Renderer = r;

            _pnlCameras.BackColor = Conf.MainColor.ToColor();

            RemoteManager = new McRemoteControlManager.RemoteControlDevice();
            RemoteManager.ButtonPressed += RemoteManagerButtonPressed;

            SetPriority();
            Arrange(false);


            _jst = new JoystickDevice();
            bool jsactive = false;
            string[] sticks = _jst.FindJoysticks();
            foreach (string js in sticks)
            {
                string[] nameid = js.Split('|');
                if (nameid[1] == Conf.Joystick.id)
                {
                    Guid g = Guid.Parse(nameid[1]);
                    jsactive = _jst.AcquireJoystick(g);
                }
            }

            if (!jsactive)
            {
                _jst.ReleaseJoystick();
                _jst = null;
            }
            else
            {
                _tmrJoystick = new Timer(100);
                _tmrJoystick.Elapsed += TmrJoystickElapsed;
                _tmrJoystick.Start();
            }
            try
            {
                INetworkListManager mNlm = new NetworkListManager();
                var icpc = (IConnectionPointContainer) mNlm;
                //similar event subscription can be used for INetworkEvents and INetworkConnectionEvents
                Guid tempGuid = typeof (INetworkListManagerEvents).GUID;
                icpc.FindConnectionPoint(ref tempGuid, out _mIcp);
                if (_mIcp != null)
                {
                    _mIcp.Advise(this, out _mCookie);
                }
            }
            catch (Exception)
            {
                _mIcp = null;
            }
            InstanceReference = this;

            try
            {
                Discovery.FindDevices();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static DateTime NeedsMediaRefresh
        {
            get { return _needsMediaRefresh; }
            set
            {
                //only store first recorded or reset value
                if (value == DateTime.MinValue)
                    _needsMediaRefresh = value;
                else
                {
                    if (_needsMediaRefresh == DateTime.MinValue)
                        _needsMediaRefresh = value;
                }
            }
        }

        public static ReadOnlyCollection<FilePreview> MasterFileList => Masterfilelist.AsReadOnly();


        public bool StorageThreadRunning
        {
            get
            {
                lock (ThreadLock)
                {
                    try
                    {
                        return _storageThread != null && !_storageThread.Join(TimeSpan.Zero);
                    }
                    catch
                    {
                        return true;
                    }
                }
            }
        }

        public static int ProductID => Program.Platform != "x86" ? 19 : 11;

        private static string DefaultBrowser
        {
            get
            {
                if (!string.IsNullOrEmpty(_browser))
                    return _browser;

                _browser = string.Empty;
                RegistryKey key = null;
                try
                {
                    key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //trim off quotes
                    if (key != null) _browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                    if (!_browser.EndsWith(".exe"))
                    {
                        _browser = _browser.Substring(0, _browser.LastIndexOf(".exe", StringComparison.Ordinal) + 4);
                    }
                }
                finally
                {
                    key?.Close();
                }
                return _browser;
            }
        }

        public void ConnectivityChanged(NLM_CONNECTIVITY newConnectivity)
        {
            var i = (int) newConnectivity;
            if (!WsWrapper.WebsiteLive)
            {
                if (newConnectivity != NLM_CONNECTIVITY.NLM_CONNECTIVITY_DISCONNECTED)
                {
                    if ((i & (int) NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV4_INTERNET) != 0 ||
                        ((int) newConnectivity & (int) NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV6_INTERNET) != 0)
                    {
                        if (!WsWrapper.WebsiteLive)
                        {
                            WsWrapper.LastLiveCheck = Helper.Now.AddMinutes(-5);
                        }
                    }
                }
            }
        }

        public static void MasterFileAdd(FilePreview fp)
        {
            lock (ThreadLock)
            {
                Masterfilelist.Add(fp);
            }
            var wss = MWS.WebSocketServer;
            if (wss != null)
                wss.SendToAll("new events|" + fp.Name);
        }

        public static void MasterFileRemoveAll(int objecttypeid, int objectid)
        {
            lock (ThreadLock)
            {
                Masterfilelist.RemoveAll(p => p.ObjectTypeId == objecttypeid && p.ObjectId == objectid);
            }
        }

        public static void MasterFileRemove(string filename)
        {
            lock (ThreadLock)
            {
                Masterfilelist.RemoveAll(p => p.Filename == filename);
            }
        }

        private bool IsOnScreen(Form form)
        {
            Screen[] screens = Screen.AllScreens;
            var formTopLeft = new Point(form.Left, form.Top);
            //hack for maximised window
            if (form.WindowState == FormWindowState.Maximized)
            {
                formTopLeft.X += 8;
                formTopLeft.Y += 8;
            }

            return screens.Any(screen => screen.WorkingArea.Contains(formTopLeft));
        }

        protected override void WndProc(ref Message message)
        {
            RemoteManager.ProcessMessage(message);
            base.WndProc(ref message);
        }


        private void RemoteManagerButtonPressed(object sender, McRemoteControlManager.RemoteControlEventArgs e)
        {
            ProcessKey(e.Button.ToString().ToLower());
        }

        public static void SetPriority()
        {
            switch (Conf.Priority)
            {
                case 1:
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
                    break;
                case 2:
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                    break;
                case 3:
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                    break;
                case 4:
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                    break;
            }
        }


        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                notifyIcon1.Visible = false;
                notifyIcon1.Dispose();

                components?.Dispose();
                _mWindowState?.Dispose();

                Drawfont.Dispose();
                _updateTimer?.Dispose();
                _houseKeepingTimer?.Dispose();
                //sometimes hangs??
                //if (_fsw != null)
                //    _fsw.Dispose();
            }
            base.Dispose(disposing);
        }

        // Close the main form
        private void ExitFileItemClick(object sender, EventArgs e)
        {
            ShuttingDown = true;
            Close();
        }

        // On "Help->About"
        private void AboutHelpItemClick(object sender, EventArgs e)
        {
            var form = new AboutForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        private void VolumeControlDoubleClick(object sender, EventArgs e)
        {
            _pnlCameras.Maximise(sender);
        }

        private void FloorPlanDoubleClick(object sender, EventArgs e)
        {
            _pnlCameras.Maximise(sender);
        }

        private static Enums.LayoutMode _layoutMode;
        public static Enums.LayoutMode LayoutMode
        {
            get
            {
                return _layoutMode;
            }
            set
            {
                _layoutMode = value;
                
                Conf.ArrangeMode = (_layoutMode == Enums.LayoutMode.AutoGrid ? 1 : 0);
                
            }
        }

        public static bool LockLayout => Conf.LockLayout || _layoutMode == Enums.LayoutMode.AutoGrid;

        private static void AddPlugin(FileInfo dll)
        {
            try
            {
                Assembly plugin = Assembly.LoadFrom(dll.FullName);
                object ins = null;
                try
                {
                    ins = plugin.CreateInstance("Plugins.Main", true);
                }
                catch
                {
                    // ignored
                }
                if (ins != null)
                {
                    Logger.LogMessage("Added: " + dll.FullName);
                    Plugins.Add(dll.FullName);
                }
            }
            catch // (Exception ex)
            {
                //Logger.LogException(ex);
            }
        }


        public void Play(string filename, int objectId, string displayName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Play(filename,objectId,displayName)));
                return;
            }
            if (_player == null)
            {
                try
                {
                    _player = new PlayerVLC(displayName, this);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex,"Play with VLC");
                    Conf.PlaybackMode = (int) Enums.PlaybackMode.Default;
                    MessageBox.Show(
                        "Could not start VLC. Check you have the right version ("+VlcHelper.MinVersion+"+) installed. Using default player instead");
                    return;
                }

                _player.Show(this);
                _player.Closed += PlayerClosed;
                _player.Activate();
                _player.BringToFront();
                _player.Owner = this;
            }

            _player.ObjectID = objectId;
            _player.Play(filename, displayName);

        }

        private void PlayerClosed(object sender, EventArgs e)
        {
            //_player = null;
            _player = null;
        }

       

        private void MainFormLoad(object sender, EventArgs e)
        {
            MainInit();
        }

        private void MainInit()
        {
            UISync.Init(this);
            Logger.InitLogging();
            try
            {
                File.WriteAllText(Program.AppDataPath + "exit.txt", "RUNNING");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            try
            {
                ffmpeg.avdevice_register_all();
                Program.SetFfmpegLogging();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            if (!SilentStartup)
            {
                switch (Conf.StartupMode)
                {
                    case 0:
                        break;
                    case 2:
                        break;
                    case 3:
                        WinApi.SetWinFullScreen(Handle);
                        break;
                }
            }

            mediaPanelControl1.MainClass = this;
            EncoderParams = new EncoderParameters(1)
                            {
                                Param =
                                {
                                    [0] =
                                        new EncoderParameter(
                                        System.Drawing.Imaging.Encoder.Quality,
                                        Conf.JPEGQuality)
                                }
                            };

            //this initializes the port mapping collection
            IStaticPortMappingCollection map = NATControl.Mappings;
            if (string.IsNullOrEmpty(Conf.MediaDirectory) || Conf.MediaDirectory == "NotSet")
            {
                Conf.MediaDirectory = Program.AppDataPath + @"WebServerRoot\Media\";
            }

            if (Conf.MediaDirectories == null || Conf.MediaDirectories.Length == 0)
            {
                Conf.MediaDirectories = new[]
                                        {
                                            new configurationDirectory
                                            {
                                                Entry = Conf.MediaDirectory,
                                                DeleteFilesOlderThanDays =
                                                    Conf.DeleteFilesOlderThanDays,
                                                Enable_Storage_Management =
                                                    Conf.Enable_Storage_Management,
                                                MaxMediaFolderSizeMB = Conf.MaxMediaFolderSizeMB,
                                                StopSavingOnStorageLimit =
                                                    Conf.StopSavingOnStorageLimit,
                                                ID = 0
                                            }
                                        };
            }
            else
            {
                if (Conf.MediaDirectories.First().Entry == "NotSet")
                {
                    Conf.MediaDirectories = new[]
                                            {
                                                new configurationDirectory
                                                {
                                                    Entry = Conf.MediaDirectory,
                                                    DeleteFilesOlderThanDays =
                                                        Conf.DeleteFilesOlderThanDays,
                                                    Enable_Storage_Management =
                                                        Conf.Enable_Storage_Management,
                                                    MaxMediaFolderSizeMB =
                                                        Conf.MaxMediaFolderSizeMB,
                                                    StopSavingOnStorageLimit =
                                                        Conf.StopSavingOnStorageLimit,
                                                    ID = 0
                                                }
                                            };
                }
            }

            //reset stop saving flag
            foreach (configurationDirectory d in Conf.MediaDirectories)
            {
                d.StopSavingFlag = false;
            }

            if (!Directory.Exists(Conf.MediaDirectories[0].Entry))
            {
                string notfound = Conf.MediaDirectories[0].Entry;
                Logger.LogError("Media directory could not be found (" + notfound + ") - reset it to " +
                               Program.AppDataPath + @"WebServerRoot\Media\" + " in settings if it doesn't attach.");
            }

            if (!VlcHelper.VLCAvailable)
            {
                Logger.LogWarningToFile(
                    "VLC not installed - install VLC (" + Program.Platform + ") for additional connectivity.");
                if (Program.Platform == "x64")
                {
                    Logger.LogWarningToFile("Download: <a href=\""+VLCx64+"\">"+VLCx64+"</a>");
                }
                else
                    Logger.LogWarningToFile("Download: <a href=\"" + VLCx86 + "\">" + VLCx86 + "</a>");
            }

            _fsw = new FileSystemWatcher
                   {
                       Path = Program.AppDataPath,
                       IncludeSubdirectories = false,
                       Filter = "external_command.txt",
                       NotifyFilter = NotifyFilters.LastWrite
                   };
            _fsw.Changed += FswChanged;
            _fsw.EnableRaisingEvents = true;

            tsslPRO.Visible = !Conf.Subscribed;
            if (metroLabelPRO!=null)
                metroLabelPRO.Visible = !Conf.Subscribed;

            if (metroLabelStats != null)
            {
                metroLabelStats.Click += new System.EventHandler(this._tsslStats_Click);
                metroLabelPerformance.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
                metroLabelPRO.Click += new System.EventHandler(this.tsslPRO_Click);
            }

            Menu = mainMenu;
            notifyIcon1.ContextMenuStrip = ctxtTaskbar;
            Identifier = Guid.NewGuid().ToString();
            MWS = new LocalServer
                  {
                      ServerRoot = Program.AppDataPath + @"WebServerRoot\"
                  };

#if DEBUG
            MWS.ServerRoot = Program.AppPath + @"WebServerRoot\";
#endif

            if (Conf.Monitor)
            {
                Process[] w = Process.GetProcessesByName("ispymonitor");
                if (w.Length == 0)
                {
                    try
                    {
                        var si = new ProcessStartInfo(Program.AppPath + "/ispymonitor.exe", "ispy");
                        Process.Start(si);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            SetBackground();

            toolStripMenu.Visible = Conf.ShowToolbar;
            statusStrip1.Visible = false;
            Menu = !Conf.ShowFileMenu ? null : mainMenu;

            if (SilentStartup)
            {
                WindowState = FormWindowState.Minimized;
            }

            if (Conf.Password_Protect_Startup)
            {
                _locked = true;
                WindowState = FormWindowState.Minimized;
            }

            if (Conf.Fullscreen && !SilentStartup && !_locked)
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;
                WinApi.SetWinFullScreen(Handle);
            }
            

            statusBarToolStripMenuItem.Checked = menuItem4.Checked = Conf.ShowStatus;
            toolStripToolStripMenuItem.Checked = menuItem6.Checked = Conf.ShowToolbar;
            fileMenuToolStripMenuItem.Checked = menuItem5.Checked = Conf.ShowFileMenu;
            fullScreenToolStripMenuItem1.Checked = menuItem3.Checked = Conf.Fullscreen;
            alwaysOnTopToolStripMenuItem1.Checked = menuItem8.Checked = Conf.AlwaysOnTop;
            mediaPaneToolStripMenuItem.Checked = menuItem7.Checked = Conf.ShowMediaPanel;
            menuItem22.Checked = Conf.LockLayout;
            menuItem39.Checked = LayoutMode == Enums.LayoutMode.AutoGrid;
            TopMost = Conf.AlwaysOnTop;

            Iconfont = new Font(FontFamily.GenericSansSerif, Conf.BigButtons ? 22 : 15, FontStyle.Bold,
                GraphicsUnit.Pixel);
            double dOpacity;
            Double.TryParse(Conf.Opacity.ToString(CultureInfo.InvariantCulture), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dOpacity);
            Opacity = dOpacity/100.0;


            if (Conf.ServerName == "NotSet")
            {
                Conf.ServerName = SystemInformation.ComputerName;
            }

            notifyIcon1.Text = Conf.TrayIconText;
            notifyIcon1.BalloonTipClicked += NotifyIcon1BalloonTipClicked;
            autoLayoutToolStripMenuItem.Checked = menuItem26.Checked = Conf.AutoLayout;

            _updateTimer = new Timer(200);
            _updateTimer.Elapsed += UpdateTimerElapsed;
            _updateTimer.AutoReset = true;
            _updateTimer.SynchronizingObject = this;
            //GC.KeepAlive(_updateTimer);

            _houseKeepingTimer = new Timer(1000);
            _houseKeepingTimer.Elapsed += HouseKeepingTimerElapsed;
            _houseKeepingTimer.AutoReset = true;
            _houseKeepingTimer.SynchronizingObject = this;
            //GC.KeepAlive(_houseKeepingTimer);

            //load plugins
            LoadPlugins();

            resetLayoutToolStripMenuItem1.Enabled = mnuResetLayout.Enabled = false; //reset layout

            NetworkChange.NetworkAddressChanged += NetworkChangeNetworkAddressChanged;
            mediaPaneToolStripMenuItem.Checked = Conf.ShowMediaPanel;
            ShowHideMediaPane();
            if (!string.IsNullOrEmpty(Conf.MediaPanelSize))
            {
                string[] dd = Conf.MediaPanelSize.Split('x');
                int d1 = Convert.ToInt32(dd[0]);
                int d2 = Convert.ToInt32(dd[1]);
                try
                {
                    splitContainer1.SplitterDistance = d1;
                    splitContainer2.SplitterDistance = d2;
                }
                catch
                {
                    // ignored
                }
            }
            //load in object list

            if (_startCommand.Trim().StartsWith("open"))
            {
                ParseCommand(_startCommand);
                _startCommand = "";
            }
            else
            {
                if (!File.Exists(Program.AppDataPath + @"XML\objects.xml"))
                {
                    File.Copy(Program.AppPath + @"XML\objects.xml", Program.AppDataPath + @"XML\objects.xml");
                }
                ParseCommand("open " + Program.AppDataPath + @"XML\objects.xml");
            }
            if (_startCommand != "")
            {
                ParseCommand(_startCommand);
            }

            StopAndStartServer();

            if (_mWindowState == null)
            {
                _mWindowState = new PersistWindowState {Parent = this, RegistryPath = @"Software\ispy\startup"};
            }

            if (Conf.Enabled_ShowGettingStarted)
                ShowGettingStarted();

            if (File.Exists(Program.AppDataPath + "custom.txt"))
            {
                string[] cfg =
                    File.ReadAllText(Program.AppDataPath + "custom.txt").Split(Environment.NewLine.ToCharArray());
                bool setSecure = false;
                foreach (string s in cfg)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        string[] nv = s.Split('=');

                        if (nv.Length > 1)
                        {
                            switch (nv[0].ToLower().Trim())
                            {
                                case "business":
                                    Conf.Vendor = nv[1].Trim();
                                    break;
                                case "link":
                                    PurchaseLink = nv[1].Trim();
                                    break;
                                case "manufacturer":
                                    IPTYPE = Conf.DefaultManufacturer = nv[1].Trim();
                                    break;
                                case "model":
                                    IPMODEL = nv[1].Trim();
                                    break;
                                case "affiliateid":
                                case "affiliate id":
                                case "aid":
                                    int aid;
                                    if (Int32.TryParse(nv[1].Trim(), out aid))
                                    {
                                        Affiliateid = aid;
                                    }
                                    break;
                                case "tags":
                                    if (string.IsNullOrEmpty(Conf.Tags))
                                        Conf.Tags = nv[1].Trim();
                                    break;
                                case "featureset":
                                    //only want to set this on install (allow them to modify)
                                    if (Conf.FirstRun)
                                    {
                                        int featureset;
                                        if (Int32.TryParse(nv[1].Trim(), out featureset))
                                        {
                                            Conf.FeatureSet = featureset;
                                        }
                                    }
                                    break;
                                case "permissions":
                                    //only want to set this on install (allow them to modify)
                                    if (Conf.FirstRun)
                                    {
                                        var groups = nv[1].Trim().Split('|');
                                        var l = new List<configurationGroup>();
                                        foreach (var g in groups)
                                        {
                                            if (!string.IsNullOrEmpty(g))
                                            {
                                                var g2 = g.Split(',');
                                                if (g2.Length >= 3)
                                                {
                                                    if (!string.IsNullOrEmpty(g2[0]))
                                                    {
                                                        int perm;
                                                        if (int.TryParse(g2[2], out perm))
                                                        {
                                                            l.Add(new configurationGroup
                                                                  {
                                                                      featureset = perm,
                                                                      name = g2[0],
                                                                      password =
                                                                          EncDec.EncryptData(g2[1],
                                                                              Conf.EncryptCode)
                                                                  });
                                                        }
                                                    }
                                                }
                                            }   
                                        }
                                        if (l.FirstOrDefault(p => p.name.ToLower() == "admin") == null)
                                        {
                                            l.Add(new configurationGroup{
                                                      featureset = 1,
                                                      name = "Admin",
                                                      password = ""
                                                  });
                                        }
                                        if (l.Count>0)
                                            Conf.Permissions = l.ToArray();

                                    }
                                    break;
                                case "webserver":
                                    string ws = nv[1].Trim().Trim('/');
                                    if (!string.IsNullOrEmpty(ws))
                                    {
                                        Webserver = ws;
                                        if (!setSecure)
                                            WebserverSecure = Webserver;
                                        CustomWebserver = true;
                                    }
                                    break;
                                case "webserversecure":
                                    WebserverSecure = nv[1].Trim().Trim('/');
                                    setSecure = true;
                                    break;
                                case "recordondetect":
                                    bool defaultRecordOnDetect;
                                    if (bool.TryParse(nv[1].Trim(), out defaultRecordOnDetect))
                                        Conf.DefaultRecordOnDetect = defaultRecordOnDetect;
                                    break;
                                case "recordonalert":
                                    bool defaultRecordOnAlert;
                                    if (bool.TryParse(nv[1].Trim(), out defaultRecordOnAlert))
                                        Conf.DefaultRecordOnAlert = defaultRecordOnAlert;
                                    break;
                            }
                        }
                    }
                }
                Conf.FirstRun = false;
                Logger.LogMessage("Webserver: " + Webserver);

                string logo = Program.AppDataPath + "logo.jpg";
                if (!File.Exists(logo))
                    logo = Program.AppDataPath + "logo.png";

                if (File.Exists(logo))
                {
                    try
                    {
                        Image bmp = Image.FromFile(logo);
                        var pb = new PictureBox {Image = bmp};
                        pb.Width = pb.Image.Width;
                        pb.Height = pb.Image.Height;

                        pb.Left = _pnlCameras.Width/2 - pb.Width/2;
                        pb.Top = _pnlCameras.Height/2 - pb.Height/2;

                        _pnlCameras.Controls.Add(pb);
                        _pnlCameras.BrandedImage = pb;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
                _lastClicked = _pnlCameras;
            }

            LoadCommands();
            if (!SilentStartup && Conf.ViewController)
            {
                ShowViewController();
                viewControllerToolStripMenuItem.Checked = menuItem14.Checked = true;
            }

            pTZControllerToolStripMenuItem.Checked =
                menuItem18.Checked = pTZControllerToolStripMenuItem1.Checked = Conf.ShowPTZController;

            menuItem52.Checked = pTZCommandButtonsToolStripMenuItem.Checked =  Conf.ShowPTZCommandButtons;


            if (Conf.ShowPTZController && !SilentStartup)
                ShowHidePTZTool();

            if (Conf.ShowPTZCommandButtons && !SilentStartup)
                ShowHidePTZCommandButtons();


            ListGridViews();

            Conf.RunTimes++;

            try
            {
                _cputotalCounter = new PerformanceCounter("Processor", "% Processor Time", "_total", true);
                _cpuCounter = new PerformanceCounter("Process", "% Processor Time",
                    Process.GetCurrentProcess().ProcessName, true);
                try
                {
                    _pcMem = new PerformanceCounter("Process", "Working Set - Private",
                        Process.GetCurrentProcess().ProcessName, true);
                }
                catch
                {
                    try
                    {
                        _pcMem = new PerformanceCounter("Memory", "Available MBytes");
                    }
                    catch (Exception ex2)
                    {
                        Logger.LogException(ex2);
                        _pcMem = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                _cputotalCounter = null;
            }


            if (Conf.StartupForm != "iSpy")
            {
                ShowGridView(Conf.StartupForm);
                
            }

            foreach (var cg in Conf.GridViews)
            {
                if (cg.ShowAtStartup)
                {
                    ShowGridView(cg.name);
                }
            }

            var t = new Thread(()=>ConnectServices()) {IsBackground = true};
            t.Start();

            _updateTimer.Start();
            _houseKeepingTimer.Start();
        }

        public void CloseGridViewRemote(string index)
        {
            if (InvokeRequired)
            {
                Invoke(new Delegates.ExternalCommandDelegate(CloseGridViewRemote), index);
                return;
            }

            int ind = Convert.ToInt32(index);
            var cg = Conf.GridViews.ToList()[ind];
            if (cg != null)
            {
                var view = _views.FirstOrDefault(p => p.Cg == cg);
                view?.Close();
            }
        }

        public void ShowGridViewRemote(string index)
        {
            if (InvokeRequired)
            {
                Invoke(new Delegates.ExternalCommandDelegate(ShowGridViewRemote), index);
                return;
            }

            int ind = Convert.ToInt32(index);
            var cg = Conf.GridViews.ToList()[ind];
            if (cg != null)
            {
                var view = _views.FirstOrDefault(p => p.Cg == cg);
                if (view != null && !view.IsDisposed)
                {
                    view.Reinit(ref cg);
                    view.Show();
                    view.BringToFront();
                    view.Focus();
                    return;
                }
                if (view != null)
                    _views.Remove(view);

                
                var gv = new GridView(this, ref cg);
                gv.Show();
                gv.BringToFront();
                gv.Focus();
                _views.Add(gv);
                return;
                
               
            }

        }
        internal void ShowGridView(string name)
        {
            configurationGrid cg = Conf.GridViews.FirstOrDefault(p => p.name == name);
            if (cg != null)
            {
                for(int i=0;i<_views.Count;i++)
                {
                    GridView g = _views[i];
                    if (g != null && !g.IsDisposed)
                    {
                        if (g.Cg == cg)
                        {
                            g.BringToFront();
                            g.Focus();
                            return;
                        }
                    }
                    else
                    {
                        _views.RemoveAt(i);
                        i--;
                    }
                        
                }
                var gv = new GridView(this, ref cg);
                gv.Show();
                _views.Add(gv);
            }
        }
        private readonly List<GridView> _views = new List<GridView>();

        public static void LoadPlugins()
        {
            Plugins = new List<string>();
            if (Directory.Exists(Program.AppPath + "Plugins"))
            {
                var plugindir = new DirectoryInfo(Program.AppPath + "Plugins");
                Logger.LogMessage("Checking Plugins...");
                foreach (FileInfo dll in plugindir.GetFiles("*.dll"))
                {
                    AddPlugin(dll);
                }
                foreach (DirectoryInfo d in plugindir.GetDirectories())
                {
                    Logger.LogMessage(d.Name);
                    foreach (FileInfo dll in d.GetFiles("*.dll"))
                    {
                        AddPlugin(dll);
                    }
                }
            }
        }

        private static void NetworkChangeNetworkAddressChanged(object sender, EventArgs e)
        {
            //schedule update check for a few seconds as a network change involves 2 calls to this event - removing and adding.
            if (_rescanIPTimer == null)
            {
                _rescanIPTimer = new Timer(5000);
                _rescanIPTimer.Elapsed += RescanIPTimerElapsed;
                _rescanIPTimer.Start();
            }
        }

        private static void RescanIPTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _rescanIPTimer.Stop();
            _rescanIPTimer = null;
            try
            {
                if (Conf.IPMode == "IPv4")
                {
                    _ipv4Addresses = null;
                    bool iplisted = false;
                    foreach (IPAddress ip in AddressListIPv4)
                    {
                        if (Conf.IPv4Address == ip.ToString())
                            iplisted = true;
                    }
                    if (!iplisted)
                    {
                        _ipv4Address = "";
                        Conf.IPv4Address = AddressIPv4;
                    }
                    if (iplisted)
                        return;
                }
                if (!string.IsNullOrEmpty(Conf.WSUsername) && !string.IsNullOrEmpty(Conf.WSPassword))
                {
                    switch (Conf.IPMode)
                    {
                        case "IPv4":

                            Logger.LogError(
                                "Your IP address has changed. Please set a static IP address for your local computer to ensure uninterrupted connectivity.");
                            //force reload of ip info
                            AddressIPv4 = Conf.IPv4Address;
                            if (Conf.Subscribed)
                            {
                                if (Conf.DHCPReroute && Conf.IPMode == "IPv4")
                                {
                                    //check if IP address has changed
                                    if (Conf.UseUPNP)
                                    {
                                        //change router ports
                                        try
                                        {
                                            if (NATControl.SetPorts(Conf.ServerPort, Conf.LANPort))
                                                Logger.LogMessage("Router port forwarding has been updated. (" +
                                                                        Conf.IPv4Address + ")");
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.LogException(ex);
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogMessage(
                                            "Please check Use UPNP in web settings to handle this automatically");
                                    }
                                }
                                else
                                {
                                    Logger.LogMessage(
                                        "Enable DHCP Reroute in Web Settings to handle this automatically");
                                }
                            }
                            MWS.StopServer();
                            MWS.StartServer();
                            WsWrapper.ForceSync();
                            break;
                        case "IPv6":
                            _ipv6Addresses = null;
                            bool iplisted = false;
                            foreach (IPAddress ip in AddressListIPv6)
                            {
                                if (Conf.IPv6Address == ip.ToString())
                                    iplisted = true;
                            }
                            if (!iplisted)
                            {
                                Logger.LogError(
                                    "Your IP address has changed. Please set a static IP address for your local computer to ensure uninterrupted connectivity.");
                                _ipv6Address = "";
                                AddressIPv6 = Conf.IPv6Address;
                                Conf.IPv6Address = AddressIPv6;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex,"network change");
            }
        }

        internal void RenderResources()
        {
            Helper.SetTitle(this);
            uploadToCloudToolStripMenuItem.Text = LocRm.GetString("UploadToCloud");
            archiveToolStripMenuItem.Text = LocRm.GetString("Archive");
            saveToToolStripMenuItem.Text = LocRm.GetString("SaveTo");
            deleteToolStripMenuItem.Text = LocRm.GetString("Delete");
            showInFolderToolStripMenuItem.Text = LocRm.GetString("ShowInFolder");
            maximiseToolStripMenuItem.Text = LocRm.GetString("Maximise");
            _aboutHelpItem.Text = LocRm.GetString("About");
            switchToolStripMenuItem.Text = LocRm.GetString("Switch");
            onToolStripMenuItem.Text = LocRm.GetString("On");
            offToolStripMenuItem.Text = LocRm.GetString("Off");
            alertsOnToolStripMenuItem1.Text = LocRm.GetString("AlertsOn");
            alertsOffToolStripMenuItem.Text = LocRm.GetString("AlertsOff");
            scheduleOnToolStripMenuItem.Text = LocRm.GetString("ScheduleOn");
            scheduleOffToolStripMenuItem.Text = LocRm.GetString("ScheduleOff");
            pTZScheduleOnToolStripMenuItem.Text = LocRm.GetString("PTZScheduleOn");
            pTZScheduleOffToolStripMenuItem.Text = LocRm.GetString("PTZScheduleOff");
            openWebInterfaceToolStripMenuItem.Text = LocRm.GetString("OpenWebInterface");
            menuItem33.Text = LocRm.GetString("Lock");
            
            _addCameraToolStripMenuItem.Text = LocRm.GetString("AddCamera");
            _addFloorPlanToolStripMenuItem.Text = LocRm.GetString("AddFloorplan");
            _addMicrophoneToolStripMenuItem.Text = LocRm.GetString("Addmicrophone");
            menuItem26.Text = autoLayoutToolStripMenuItem.Text = LocRm.GetString("AutoLayout");
            gridViewsToolStripMenuItem.Text = gridViewsToolStripMenuItem1.Text = LocRm.GetString("GridViews");
            _deleteToolStripMenuItem.Text = LocRm.GetString("remove");
            _editToolStripMenuItem.Text = LocRm.GetString("Edit");
            _exitFileItem.Text = LocRm.GetString("Exit");
            _exitToolStripMenuItem.Text = LocRm.GetString("Exit");
            _fileItem.Text = LocRm.GetString("file");
            fileMenuToolStripMenuItem.Text = LocRm.GetString("Filemenu");
            menuItem5.Text = LocRm.GetString("Filemenu");
            _floorPlanToolStripMenuItem.Text = LocRm.GetString("FloorPlan");
            fullScreenToolStripMenuItem.Text = LocRm.GetString("fullScreen");
            fullScreenToolStripMenuItem1.Text = LocRm.GetString("fullScreen");
            _helpItem.Text = LocRm.GetString("help");
            _helpToolstripMenuItem.Text = LocRm.GetString("help");
            _iPCameraToolStripMenuItem.Text = LocRm.GetString("IpCamera");
            _menuItem24.Text = LocRm.GetString("ShowGettingStarted");
            _listenToolStripMenuItem.Text = LocRm.GetString("Listen");
            _localCameraToolStripMenuItem.Text = LocRm.GetString("LocalCamera");
            _menuItem1.Text = "-";
            _menuItem10.Text = LocRm.GetString("checkForUpdates");
            _menuItem13.Text = "-";
            _menuItem15.Text = LocRm.GetString("ResetAllRecordingCounters");
            _menuItem16.Text = LocRm.GetString("View");
            _menuItem17.Text = inExplorerToolStripMenuItem.Text = LocRm.GetString("files");
            _menuItem18.Text = LocRm.GetString("clearCaptureDirectories");
            _menuItem19.Text = LocRm.GetString("saveObjectList");
            _menuItem2.Text = LocRm.GetString("help");
            _menuItem20.Text = viewLogFileToolStripMenuItem.Text = LocRm.GetString("Logfile");
            _menuItem21.Text = LocRm.GetString("openObjectList");
            _menuItem22.Text = LocRm.GetString("LogFiles");
            _menuItem23.Text = LocRm.GetString("audiofiles");
            menuItem29.Text = LocRm.GetString("Archive");
            _menuItem25.Text = LocRm.GetString("MediaOnAMobiledeviceiphon");
            _menuItem26.Text = LocRm.GetString("supportIspyWithADonation");
            _menuItem27.Text = "-";
            _menuItem29.Text = LocRm.GetString("Current");
            _menuItem3.Text = LocRm.GetString("MediaoverTheWeb");
            _menuItem30.Text = "-";
            _menuItem31.Text = LocRm.GetString("removeAllObjects");
            _menuItem32.Text = "-";
            _menuItem33.Text = LocRm.GetString("switchOff");
            _menuItem34.Text = LocRm.GetString("Switchon");
            _miOnAll.Text = LocRm.GetString("All");
            _miOffAll.Text = LocRm.GetString("All");
            _miOnSched.Text = LocRm.GetString("Scheduled");
            _miOffSched.Text = LocRm.GetString("Scheduled");
            _miApplySchedule.Text = _applyScheduleToolStripMenuItem1.Text = LocRm.GetString("ApplySchedule");
            _applyScheduleToolStripMenuItem.Text = LocRm.GetString("ApplySchedule");
            _menuItem35.Text = LocRm.GetString("ConfigureremoteCommands");
            _menuItem36.Text = LocRm.GetString("Edit");
            _menuItem37.Text = LocRm.GetString("CamerasAndMicrophones");
            _menuItem38.Text = LocRm.GetString("ViewUpdateInformation");
            _menuItem39.Text = LocRm.GetString("AutoLayoutObjects");
            _menuItem4.Text = LocRm.GetString("ConfigureremoteAccess");
            _menuItem5.Text = LocRm.GetString("GoTowebsite");
            _menuItem6.Text = "-";
            _menuItem7.Text = LocRm.GetString("videofiles");
            _menuItem8.Text = LocRm.GetString("settings");
            _menuItem9.Text = LocRm.GetString("options");
            menuItem39.Text = LocRm.GetString("Grid");
            _microphoneToolStripMenuItem.Text = LocRm.GetString("Microphone");
            notifyIcon1.Text = LocRm.GetString("Ispy");
            _onMobileDevicesToolStripMenuItem.Text = LocRm.GetString("MobileDevices");

            opacityToolStripMenuItem.Text = LocRm.GetString("Opacity");
            opacityToolStripMenuItem1.Text = LocRm.GetString("Opacity10");
            opacityToolStripMenuItem2.Text = LocRm.GetString("Opacity30");
            opacityToolStripMenuItem3.Text = LocRm.GetString("Opacity100");

            menuItem9.Text = LocRm.GetString("Opacity");
            menuItem10.Text = LocRm.GetString("Opacity10");
            menuItem11.Text = LocRm.GetString("Opacity30");
            menuItem12.Text = LocRm.GetString("Opacity100");


            _positionToolStripMenuItem.Text = LocRm.GetString("Position");
            _recordNowToolStripMenuItem.Text = LocRm.GetString("RecordNow");
            _remoteCommandsToolStripMenuItem.Text = LocRm.GetString("RemoteCommands");
            _resetRecordingCounterToolStripMenuItem.Text = LocRm.GetString("ResetRecordingCounter");
            _resetSizeToolStripMenuItem.Text = LocRm.GetString("ResetSize");
            _settingsToolStripMenuItem.Text = LocRm.GetString("settings");
            _showFilesToolStripMenuItem.Text = LocRm.GetString("ShowFiles");
            _showISpy100PercentOpacityToolStripMenuItem.Text = LocRm.GetString("ShowIspy100Opacity");
            _showISpy10PercentOpacityToolStripMenuItem.Text = LocRm.GetString("ShowIspy10Opacity");
            _showISpy30OpacityToolStripMenuItem.Text = LocRm.GetString("ShowIspy30Opacity");
            _showToolstripMenuItem.Text = LocRm.GetString("showIspy");
            statusBarToolStripMenuItem.Text = LocRm.GetString("Statusbar");
            menuItem4.Text = LocRm.GetString("Statusbar");
            _switchAllOffToolStripMenuItem.Text = LocRm.GetString("SwitchAllOff");
            _switchAllOnToolStripMenuItem.Text = LocRm.GetString("SwitchAllOn");
            _takePhotoToolStripMenuItem.Text = LocRm.GetString("TakePhoto");
            _thruWebsiteToolStripMenuItem.Text = LocRm.GetString("Online");
            _toolStripButton1.Text = LocRm.GetString("WebSettings");
            _toolStripButton4.Text = LocRm.GetString("settings");
            _toolStripButton8.Text = LocRm.GetString("Commands");
            _toolStripDropDownButton1.Text = LocRm.GetString("AccessMedia");
            _toolStripDropDownButton2.Text = LocRm.GetString("AddMenu");
            _viewMediaToolStripMenuItem.Text = LocRm.GetString("Viewmedia");
            toolStripToolStripMenuItem.Text = LocRm.GetString("toolStrip");
            menuItem6.Text = LocRm.GetString("toolStrip");
            _tsslStats.Text = LocRm.GetString("Loading");
            if (metroLabelStats!=null)
                metroLabelStats.Text = LocRm.GetString("Loading");
            _unlockToolstripMenuItem.Text = LocRm.GetString("unlock");
            _viewMediaOnAMobileDeviceToolStripMenuItem.Text = LocRm.GetString("ViewMediaOnAMobiledevice");
            _websiteToolstripMenuItem.Text = LocRm.GetString("website");
            _uSbCamerasAndMicrophonesOnOtherToolStripMenuItem.Text =
                LocRm.GetString("CamerasAndMicrophonesOnOtherComputers");
            fullScreenToolStripMenuItem.Text = LocRm.GetString("Fullscreen");
            menuItem3.Text = LocRm.GetString("Fullscreen");
            alwaysOnTopToolStripMenuItem1.Text = LocRm.GetString("AlwaysOnTop");
            menuItem8.Text = LocRm.GetString("AlwaysOnTop");
           
            _exitToolStripMenuItem.Text = LocRm.GetString("Exit");

            layoutToolStripMenuItem.Text = LocRm.GetString("Layout");
            displayToolStripMenuItem.Text = LocRm.GetString("Display");

            mnuSaveLayout.Text = saveLayoutToolStripMenuItem1.Text = LocRm.GetString("SaveLayout");
            mnuResetLayout.Text = resetLayoutToolStripMenuItem1.Text = LocRm.GetString("ResetLayout");
            mediaPaneToolStripMenuItem.Text = LocRm.GetString("ShowMediaPanel");
            menuItem7.Text = LocRm.GetString("ShowMediaPanel");
            iPCameraWithWizardToolStripMenuItem.Text = LocRm.GetString("IPCameraWithWizard");
            tsbPlugins.Text = LocRm.GetString("Plugins");

            menuItem14.Text = viewControllerToolStripMenuItem.Text = LocRm.GetString("ViewController");
            menuItem28.Text = LocRm.GetString("RemoveAllObjects");
            menuItem40.Text = LocRm.GetString("Find");
            

            LocRm.SetString(menuItem15, "ArrangeMedia");
            LocRm.SetString(menuItem22, "LockLayout");
            LocRm.SetString(menuItem16, "Bottom");
            LocRm.SetString(menuItem17, "Left");
            LocRm.SetString(menuItem19, "Right");
            LocRm.SetString(menuItem18, "PTZController");
            LocRm.SetString(menuItem52, "PTZCommandButtons");
            LocRm.SetString(tsslPerformance, "PerfTips");
            if (metroLabelPerformance!=null)
                LocRm.SetString(metroLabelPerformance, "PerfTips");
            LocRm.SetString(menuItem21, "Optimised");
            LocRm.SetString(_menuItem29, "Current");
            LocRm.SetString(menuItem1, "Native");
            LocRm.SetString(_menuItem27, "Minimise");
            LocRm.SetString(iSpyToolStripMenuItem, "PlayInISpy");
            LocRm.SetString(defaultPlayerToolStripMenuItem, "PlayInDefault");
            LocRm.SetString(websiteToolStripMenuItem, "PlayOnWebsite");
            LocRm.SetString(oNVIFCameraToolStripMenuItem, "ONVIFCamera");
            LocRm.SetString(videoFileToolStripMenuItem, "VideoFile");
            LocRm.SetString(otherVideoSourceToolStripMenuItem, "OtherVideoSource");



            tssbGridViews.Text = LocRm.GetString("GridViews");
            manageToolStripMenuItem.Text = LocRm.GetString("Manage");
            menuItem25.Text = LocRm.GetString("DefaultDeviceManager");
            LocRm.SetString(menuItem37,"ChangeUser");

            _toolStripDropDownButton1.Visible = menuItem7.Visible = mediaPaneToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Access_Media));
            
            _toolStripButton8.Visible =
                _remoteCommandsToolStripMenuItem.Visible =
                    _menuItem35.Visible = (Helper.HasFeature(Enums.Features.Remote_Commands));
            _toolStripButton1.Visible =
                _viewMediaToolStripMenuItem.Visible =
                    _menuItem3.Visible =
                        _viewMediaOnAMobileDeviceToolStripMenuItem.Visible =
                            _menuItem25.Visible = _menuItem4.Visible = (Helper.HasFeature(Enums.Features.Web_Settings));
            menuItem18.Visible = (Helper.HasFeature(Enums.Features.PTZ));
            menuItem52.Visible = pTZCommandButtonsToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.PTZ));
            tsbPlugins.Visible = (Helper.HasFeature(Enums.Features.Plugins));
            _localCameraToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Source_Local));
            _iPCameraToolStripMenuItem.Visible =
                iPCameraWithWizardToolStripMenuItem.Visible =
                    _addCameraToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.IPCameras));
            _floorPlanToolStripMenuItem.Visible =
                _addFloorPlanToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Floorplans));
            videoFileToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Source_VLC)) ||
                                                 (Helper.HasFeature(Enums.Features.Source_FFmpeg));
            otherVideoSourceToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Source_Custom));
            _microphoneToolStripMenuItem.Visible =
                _addMicrophoneToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Microphones));
            _uSbCamerasAndMicrophonesOnOtherToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Source_JPEG)) ||
                                                                        (Helper.HasFeature(Enums.Features.Source_MJPEG));
            oNVIFCameraToolStripMenuItem.Visible = (Helper.HasFeature(Enums.Features.Source_ONVIF));

            splitContainer2.Panel2Collapsed = !Helper.HasFeature(Enums.Features.Remote_Commands);

            tssbGridViews.Visible = menuItem31.Visible = Helper.HasFeature(Enums.Features.Grid_Views);
            _toolStripButton4.Visible = Helper.HasFeature(Enums.Features.Settings);
            menuItem2.Visible = _menuItem20.Visible = _menuItem22.Visible = Helper.HasFeature(Enums.Features.Logs);

            _menuItem17.Visible = _menuItem18.Visible = menuItem17.Visible =
                menuItem23.Visible =
                    menuItem25.Visible = menuItem18.Visible = menuItem52.Visible = pTZCommandButtonsToolStripMenuItem.Visible = menuItem7.Visible = Helper.HasFeature(Enums.Features.Access_Media);

            _toolStripDropDownButton2.Visible = _editToolStripMenuItem.Visible = _menuItem36.Visible = _menuItem31.Visible = Helper.HasFeature(Enums.Features.Edit);
            

            _fileItem.Visible = menuItem5.Visible = fileMenuToolStripMenuItem.Visible = Helper.HasFeature(Enums.Features.View_File_Menu);
            _menuItem2.Visible = _menuItem24.Visible = _menuItem10.Visible = _menuItem38.Visible = _menuItem5.Visible = _menuItem27.Visible = _menuItem26.Visible = _menuItem30.Visible = Helper.HasFeature(Enums.Features.View_Ispy_Links);
            if (!Helper.HasFeature(Enums.Features.Access_Media))
                splitContainer1.Panel2Collapsed = true;

            _menuItem26.Visible = tsslPerformance.Visible = tsslPRO.Visible = Helper.HasFeature(Enums.Features.View_Ispy_Links);

            statusStrip1.Visible = Conf.ShowStatus && Helper.HasFeature(Enums.Features.View_Status_Bar);
            menuItem38.Visible = menuItem15.Visible = Helper.HasFeature(Enums.Features.View_Layout_Options);
            menuItem4.Visible = statusBarToolStripMenuItem.Visible = Helper.HasFeature(Enums.Features.View_Status_Bar);

            menuItem28.Visible = Helper.HasFeature(Enums.Features.Edit);

            menuItem31.Text = LocRm.GetString("GridViews");
            menuItem32.Text = LocRm.GetString("Manage");
            menuItem36.Text = LocRm.GetString("ImportObjects");
            tagsToolStripMenuItem.Text = LocRm.GetString("Tags");
            ShowHideMediaPane();
            mediaPanelControl1.RenderResources();
            
        }

        private void HouseKeepingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _houseKeepingTimer.Stop();
            if (LayoutPanel.NeedsRedraw)
            {
                _pnlCameras.PerformLayout();
                _pnlCameras.Invalidate();
                LayoutPanel.NeedsRedraw = false;
            }
            if (NeedsResourceUpdate)
            {
                RenderResources();
                NeedsResourceUpdate = false;
            }
            if (_cputotalCounter != null)
            {
                try
                {
                    while (_cpuAverages.Count > 4)
                        _cpuAverages.RemoveAt(0);
                    _cpuAverages.Add(_cpuCounter.NextValue()/Environment.ProcessorCount);

                    CpuUsage = _cpuAverages.Sum()/_cpuAverages.Count;
                    CpuTotal = _cputotalCounter.NextValue();
                    _counters = $"CPU: {CpuUsage:0.00}%";

                    if (_pcMem != null)
                    {
                        _counters += " RAM Usage: " + Convert.ToInt32(_pcMem.RawValue/1048576) + "Mb";
                    }
                    tsslMonitor.Text = _counters;
                    if (metroLabelMonitor!=null)
                        metroLabelMonitor.Text = _counters;
                }
                catch (Exception ex)
                {
                    // _cputotalCounter = null;
                    Logger.LogException(ex);
                }

                HighCPU = CpuTotal > _conf.CPUMax;
            }
            else
            {
                _counters = "Stats Unavailable - See Log File";
            }

            if (_lastOver > DateTime.MinValue)
            {
                if (_lastOver < Helper.Now.AddSeconds(-4))
                {
                    tsslMediaInfo.Text = "";
                    if (metroLabelMediaInfo!=null)
                        metroLabelMediaInfo.Text = "";
                    _lastOver = DateTime.MinValue;
                }
            }

            _pingCounter++;

            if (NeedsMediaRefresh > DateTime.MinValue && NeedsMediaRefresh < Helper.Now.AddSeconds(-1))
                LoadPreviews();


            if (Resizing)
            {
                if (_lastResize < Helper.Now.AddSeconds(-1))
                    Resizing = false;
            }

            if (_pingCounter >= 301)
            {
                _pingCounter = 0;
                //auto save
                try
                {
                    SaveObjects("");
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                try
                {
                    SaveConfig();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            try
            {
                if (!MWS.Running)
                {
                    _tsslStats.Text = "Server Error - see log file";
                    if (metroLabelStats!=null)
                        metroLabelStats.Text = "Server Error - see log file";
                    if (MWS.NumErr >= 5)
                    {
                        Logger.LogMessage("Server not running - restarting");
                        StopAndStartServer();
                    }
                }
                else
                {
                    if (WsWrapper.WebsiteLive)
                    {
                        if (Conf.ServicesEnabled && !WsWrapper.LoginFailed)
                        {
                            _tsslStats.Text = LocRm.GetString("Online");
                            if (LoopBack && Conf.Subscribed)
                                _tsslStats.Text += $" ({LocRm.GetString("loopback")})";
                            else
                            {
                                if (!Conf.Subscribed)
                                    _tsslStats.Text += $" ({LocRm.GetString("LANonlynotsubscribed")})";
                                else
                                    _tsslStats.Text += $" ({LocRm.GetString("LANonlyNoLoopback")})";
                            }
                            if (metroLabelStats != null)
                                metroLabelStats.Text = _tsslStats.Text;
                        }
                        else
                        {
                            _tsslStats.Text = LocRm.GetString("Offline");
                            if (metroLabelStats != null)
                                metroLabelStats.Text = LocRm.GetString("Offline");
                        }
                    }
                    else
                    {
                        _tsslStats.Text = LocRm.GetString("Offline");
                        if (metroLabelStats != null)
                            metroLabelStats.Text = LocRm.GetString("Offline");
                    }
                }

                if (Conf.ServicesEnabled && !WsWrapper.LoginFailed)
                {
                    if (NeedsSync)
                    {
                        WsWrapper.ForceSync();
                    }
                    WsWrapper.PingServer();
                }


                _storageCounter++;
                if (_storageCounter == 3600) // every hour
                {
                    RunStorageManagement();
                    _storageCounter = 0;
                }


                if (_pingCounter == 80)
                {
                    var t = new Thread(SaveFileData) {IsBackground = true, Name = "Saving File Data"};
                    t.Start();
                }

                if (_needsDelete)
                {
                    _needsDelete = false;
                    try
                    {