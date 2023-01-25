﻿/*
Copyright (c) 2018-2021 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

Copyright (c) 2019-2021 PHOENIX CONTACT GmbH & Co. KG <opensource@phoenixcontact.com>,
author: Andreas Orzelski

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AasxIntegrationBase;
using AasxPackageExplorer;
using AasxPackageLogic;
using AasxPackageLogic.PackageCentral;
using AdminShellNS;
using AnyUi;
using Extensions;
using Microsoft.JSInterop;

namespace BlazorUI.Data
{
    /// <summary>
    /// This class is used by Blazor to auto-create session information.
    /// A session holds almost all data a user concerns with, as multiple users/ roles might use the same
    /// server application.
    /// </summary>
    public partial class BlazorSession : IDisposable
    {
        /// <summary>
        /// Monotonous index to be counted upwards to generate SessionId
        /// </summary>
        public static int SessionIndex = 0;

        /// <summary>
        /// Numerical session id of that specific session.
        /// </summary>
        public int SessionId = 0;

        /// <summary>
        /// Number of active session; incremented an decremented (on disposal)
        /// </summary>
        public static int SessionNumActive = 0;

        /// <summary>
        /// All loaded packages, repos are user-specific
        /// </summary>
        // TODO (MIHO, 2023-01-15): Think of a concept of sharing? locking? repo items
        public PackageCentral PackageCentral = null;

        /// <summary>
        /// This object wrap the visual elements, that is, the main tree
        /// </summary>
        public BlazorVisualElements DisplayElements = new BlazorVisualElements();


        /// <summary>
        /// The main menu holds the options, which are provided by a top/left positioned 
        /// application menu
        /// </summary>
        public AasxMenuBlazor MainMenu = new AasxMenuBlazor();

        /// <summary>
        /// Holds the stack panel of widgets for active AAS element.
        /// </summary>
        public AnyUiStackPanel ElementPanel = new AnyUiStackPanel();

        /// <summary>
        /// Position of the dividing column (of 12) between left & right panel
        /// </summary>
        public int DividerTreeAndElement = 4;

        /// <summary>
        /// Session is in edit mode. This setting shall be controlled by the menu / hotkey/ script
        /// functionality.
        /// </summary>
        public bool EditMode = false;

        /// <summary>
        /// Session is in edit mode. This setting shall be controlled by the menu / hotkey/ script
        /// functionality.
        /// </summary>
        public bool HintMode = true;

        // old stuff, to be refactored

        public AdminShellPackageEnv env = null;
        public IndexOfSignificantAasElements significantElements = null;

        public string[] aasxFiles = new string[1];
        public string aasxFileSelected = "";
        public PackageContainerListHttpRestRepository repository = null;
        public DispEditHelperEntities helper = null;
        public ModifyRepo repo = null;
        public PackageContainerBase container = null;

        public AnyUiStackPanel stack = new AnyUiStackPanel();
        public AnyUiStackPanel stack2 = new AnyUiStackPanel();


        public string thumbNail = null;

        public IJSRuntime renderJsRuntime = null;

        public ListOfItems items = null;
        public Thread htmlDotnetThread = null;

        public static int totalIndexTimer = 0;

        public Plugins.PluginInstance LoadedPluginInstance = null;
        public object LoadedPluginSessionId = null;

        /// <summary>
        /// Called to create a new session
        /// </summary>
        public BlazorSession()
        {
            // Statistics
            SessionId = ++SessionIndex;
            SessionNumActive++;

            // create a new session for plugin / event handling
            AnyUiDisplayContextHtml.addSession(SessionId);

            // create a new package central
            PackageCentral = new PackageCentral();

            // display elements have a cache
            DisplayElements.ActivateElementStateCache();

            // logical main menu
            var logicalMainMenu = ExplorerMenuFactory.CreateMainMenu();
            logicalMainMenu.DefaultActionAsync = CommandBinding_GeneralDispatch;

            // top level children have other color
            logicalMainMenu.DefaultForeground = AnyUiColors.Black;
            foreach (var mi in logicalMainMenu)
                if (mi is AasxMenuItem mii)
                    mii.Foreground = AnyUiColors.White;

            // Main menu
            MainMenu = new AasxMenuBlazor();
            MainMenu.LoadAndRender(logicalMainMenu, null, null);

            // show Logo?
            if (Options.Curr.LogoFile != null)
                try
                {
                    //var fullfn = System.IO.Path.GetFullPath(Options.Curr.LogoFile);
                    //var bi = new BitmapImage(new Uri(fullfn, UriKind.RelativeOrAbsolute));
                    //this.LogoImage.Source = bi;
                    //this.LogoImage.UpdateLayout();
                }
                catch (Exception ex)
                {
                    AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
                }

            // Package Central starting ..
            PackageCentral.CentralRuntimeOptions = UiBuildRuntimeOptionsForMainAppLoad();

            // LRU repository?
            var lruFn = PackageContainerListLastRecentlyUsed.BuildDefaultFilename();
            try
            {
                if (System.IO.File.Exists(lruFn))
                {
                    var lru = PackageContainerListLastRecentlyUsed.Load<PackageContainerListLastRecentlyUsed>(lruFn);
                    PackageCentral?.Repositories.Add(lru);
                }
            }
            catch (Exception ex)
            {
                Log.Singleton.Error(ex, $"while loading last recently used file {lruFn}");
            }

            // Repository pointed by the Options
            if (Options.Curr.AasxRepositoryFn.HasContent())
            {
                //var fr2 = UiLoadFileRepository(Options.Curr.AasxRepositoryFn);
                //if (fr2 != null)
                //{
                //    this.UiAssertFileRepository(visible: true);
                //    _packageCentral.Repositories.AddAtTop(fr2);
                //}
            }

            // initialize menu
            //_mainMenu?.SetChecked("FileRepoLoadWoPrompt", Options.Curr.LoadWithoutPrompt);
            //_mainMenu?.SetChecked("ShowIriMenu", Options.Curr.ShowIdAsIri);
            //_mainMenu?.SetChecked("VerboseConnect", Options.Curr.VerboseConnect);
            //_mainMenu?.SetChecked("AnimateElements", Options.Curr.AnimateElements);
            //_mainMenu?.SetChecked("ObserveEvents", Options.Curr.ObserveEvents);
            //_mainMenu?.SetChecked("CompressEvents", Options.Curr.CompressEvents);

            // the UI application might receive events from items in the package central
            PackageCentral.ChangeEventHandler = (data) =>
            {
                // if (data.Reason == PackCntChangeEventReason.Exception)
                    Log.Singleton.Info("PackageCentral events: " + data.Info);
                //DisplayElements.PushEvent(new AnyUiLambdaActionPackCntChange() { Change = data });
                return false;
            };

            // nearly last task here ..
            Log.Singleton.Info("Application started ..");

            // start with a new file
            PackageCentral.MainItem.New();
            RedrawAllAasxElements();

            // pump all pending log messages(from plugins) into the
            // log / status line, before setting the last information
            // MainTimer_HandleLogMessages();

            // Try to load?            
            if (Options.Curr.AasxToLoad != null)
            {
                var location = Options.Curr.AasxToLoad;
                try
                {
                    Log.Singleton.Info($"Auto-load file at application start " +
                        $"from {location} into container");

                    var container = PackageContainerFactory.GuessAndCreateFor(
                        PackageCentral,
                        location,
                        location,
                        overrideLoadResident: true,
                        containerOptions: PackageContainerOptionsBase.CreateDefault(Options.Curr),
                        runtimeOptions: PackageCentral.CentralRuntimeOptions);

                    if (container == null)
                        Log.Singleton.Error($"Failed to auto-load AASX from {location}");
                    else
                        UiLoadPackageWithNew(PackageCentral.MainItem,
                            takeOverContainer: container, onlyAuxiliary: false, indexItems: true);

                    Log.Singleton.Info($"Successfully auto-loaded AASX {location}");
                }
                catch (Exception ex)
                {
                    Log.Singleton.Error(ex, $"When auto-loading {location}");
                }
            }

            //
            // OLD
            //

            env = null;

            helper = new DispEditHelperEntities();
            helper.levelColors = DispLevelColors.GetLevelColorsFromOptions(Options.Curr);
            
            // some functionality still uses repo != null to detect editMode!!
            repo = new ModifyRepo();
            helper.editMode = EditMode;
            helper.hintMode = HintMode;
            helper.repo = repo;
            helper.context = null;
            helper.packages = PackageCentral;

            ElementPanel = new AnyUiStackPanel() { Orientation = AnyUiOrientation.Vertical };

            if (env?.AasEnv?.AssetAdministrationShells != null)
                helper.DisplayOrEditAasEntityAas(PackageCentral, env.AasEnv,
                    env.AasEnv.AssetAdministrationShells[0], EditMode, ElementPanel, hintMode: HintMode);

            htmlDotnetThread = new Thread(AnyUiDisplayContextHtml.htmlDotnetLoop);
            htmlDotnetThread.Start();
        }

        /// <summary>
        /// Called by Blazor to dispose a session
        /// </summary>
        public void Dispose()
        {
            AnyUiDisplayContextHtml.deleteSession(SessionId);
            SessionNumActive--;
            if (env != null)
                env.Close();
        }

        /// <summary>
        /// Sends a dispose signal to the loaded plugin in order to properly
        /// release its resources before session might be disposed or plugin might
        /// be changed.
        /// </summary>
        public void DisposeLoadedPlugin()
        {
            // access
            if (LoadedPluginInstance == null || LoadedPluginSessionId == null)
            {
                LoadedPluginInstance = null;
                LoadedPluginSessionId = null;
                return;
            }

            // try release
            try
            {
                LoadedPluginInstance.InvokeAction("dispose-anyui-visual-extension",
                    LoadedPluginSessionId);

                LoadedPluginInstance = null;
                LoadedPluginSessionId = null;
            }
            catch (Exception ex)
            {
                LogInternally.That.CompletelyIgnoredError(ex);
            }
        }

        private PackCntRuntimeOptions UiBuildRuntimeOptionsForMainAppLoad()
        {
            var ro = new PackCntRuntimeOptions()
            {
                Log = Log.Singleton,
                ProgressChanged = (state, tfs, tbd) =>
                {
                    ;
                },
                ShowMesssageBox = (content, text, title, buttons) =>
                {
                    return AnyUiMessageBoxResult.Cancel;
                }
            };
            return ro;
        }

        public void UiLoadPackageWithNew(
            PackageCentralItem packItem,
            AdminShellPackageEnv takeOverEnv = null,
            string loadLocalFilename = null,
            string info = null,
            bool onlyAuxiliary = false,
            bool doNotNavigateAfterLoaded = false,
            PackageContainerBase takeOverContainer = null,
            string storeFnToLRU = null,
            bool indexItems = false)
        {
            // access
            if (packItem == null)
                return;

            if (loadLocalFilename != null)
            {
                if (info == null)
                    info = loadLocalFilename;
                Log.Singleton.Info("Loading new AASX from: {0} as auxiliary {1} ..", info, onlyAuxiliary);
                if (!packItem.Load(PackageCentral, loadLocalFilename, loadLocalFilename,
                    overrideLoadResident: true,
                    PackageContainerOptionsBase.CreateDefault(Options.Curr)))
                {
                    Log.Singleton.Error($"Loading local-file {info} as auxiliary {onlyAuxiliary} did not " +
                        $"return any result!");
                    return;
                }
            }
            else
            if (takeOverEnv != null)
            {
                Log.Singleton.Info("Loading new AASX from: {0} as auxiliary {1} ..", info, onlyAuxiliary);
                packItem.TakeOver(takeOverEnv);
            }
            else
            if (takeOverContainer != null)
            {
                Log.Singleton.Info("Loading new AASX from container: {0} as auxiliary {1} ..",
                    "" + takeOverContainer.ToString(), onlyAuxiliary);
                packItem.TakeOver(takeOverContainer);
            }
            else
            {
                Log.Singleton.Error("UiLoadPackageWithNew(): no information what to load!");
                return;
            }

            // displaying
            try
            {
                RestartUIafterNewPackage(onlyAuxiliary);
            }
            catch (Exception ex)
            {
                Log.Singleton.Error(
                    ex, $"When displaying element tree of {info}, an error occurred");
                return;
            }

            // further actions
            try
            {
                // TODO (MIHO, 2020-12-31): check for ANYUI MIHO
                //if (!doNotNavigateAfterLoaded)
                //    UiCheckIfActivateLoadedNavTo();

                if (indexItems && packItem?.Container?.Env?.AasEnv != null)
                    packItem.Container.SignificantElements
                        = new IndexOfSignificantAasElements(packItem.Container.Env.AasEnv);
            }
            catch (Exception ex)
            {
                Log.Singleton.Error(
                    ex, $"When performing actions after load of {info}, an error occurred");
                return;
            }

            // record in LRU?
            try
            {
                var lru = PackageCentral?.Repositories?.FindLRU();
                if (lru != null && storeFnToLRU.HasContent())
                    lru.Push(packItem?.Container as PackageContainerRepoItem, storeFnToLRU);
            }
            catch (Exception ex)
            {
                Log.Singleton.Error(
                    ex, $"When managing LRU files");
                return;
            }

            // done
            Log.Singleton.Info("AASX {0} loaded.", info);
        }

        private void RestartUIafterNewPackage(bool onlyAuxiliary = false)
        {
            if (onlyAuxiliary)
            {
                // reduced, in the background
                RedrawAllAasxElements();
            }
            else
            {
                // visually a new content
                // switch off edit mode -> will will cause the browser to show the AAS as selected element
                // and -> this will update the left side of the screen correctly!
                // _mainMenu?.SetChecked("EditMenu", false);
                // ClearAllViews();
                RedrawAllAasxElements();
                // RedrawElementView();
                // ShowContentBrowser(Options.Curr.ContentHome, silent: true);
                // _eventHandling.Reset();
            }
        }

        public void RedrawAllAasxElements(bool keepFocus = false)
        {
            // focus info
            var focusMdo = DisplayElements.SelectedItem?.GetDereferencedMainDataObject();

            // TODO: Can we set title of the browser tab?
            //var t = "AASX Package Explorer V3RC02";  //TODO:jtikekar remove V3RC02
            //if (PackageCentral.MainAvailable)
            //    t += " - " + PackageCentral.MainItem.ToString();
            //if (PackageCentral.AuxAvailable)
            //    t += " (auxiliary AASX: " + PackageCentral.AuxItem.ToString() + ")";            
            // this.Title = t;

#if _log_times
            Log.Singleton.Info("Time 10 is: " + DateTime.Now.ToString("hh:mm:ss.fff"));
#endif

            // clear the right section, first (might be rebuild by callback from below)
            // DispEditEntityPanel.ClearDisplayDefautlStack();
            // ContentTakeOver.IsEnabled = false;

            // rebuild middle section
            DisplayElements.RebuildAasxElements(
                PackageCentral, PackageCentral.Selector.Main, this.EditMode,
                lazyLoadingFirst: true);

            // ok .. try re-focus!!
            if (keepFocus)
            {
                // make sure that Submodel is expanded
                this.DisplayElements.ExpandAllItems();

                // still proceed?
                var veFound = this.DisplayElements.SearchVisualElementOnMainDataObject(focusMdo,
                        alsoDereferenceObjects: true);

                if (veFound != null)
                    DisplayElements.TrySelectVisualElement(veFound, wishExpanded: true);
            }

            // display again
            DisplayElements.Refresh();            

#if _log_times
            Log.Singleton.Info("Time 90 is: " + DateTime.Now.ToString("hh:mm:ss.fff"));
#endif
        }

        public void RedrawElementView(DispEditHighlight.HighlightFieldInfo hightlightField = null)
        {
            if (DisplayElements == null)
                return;

            //// the AAS will cause some more visual effects
            //var tvlaas = DisplayElements.SelectedItem as VisualElementAdminShell;
            //if (_packageCentral.MainAvailable && tvlaas != null && tvlaas.theAas != null && tvlaas.theEnv != null)
            //{
            //    // AAS
            //    // update graphic left

            //    // what is AAS specific?
            //    this.AasId.Text = WpfStringAddWrapChars(
            //        AdminShellUtil.EvalToNonNullString("{0}", tvlaas.theAas.Id, "<id missing!>"));

            //    // what is asset specific?
            //    this.AssetPic.Source = null;
            //    this.AssetId.Text = "<id missing!>";
            //    var asset = tvlaas.theAas.AssetInformation;
            //    if (asset != null)
            //    {

            //        // text id
            //        if (asset.GlobalAssetId != null)
            //            this.AssetId.Text = WpfStringAddWrapChars(
            //                AdminShellUtil.EvalToNonNullString("{0}", asset.GlobalAssetId.GetAsIdentifier()));

            //        // asset thumbnail
            //        try
            //        {
            //            // identify which stream to use..
            //            if (_packageCentral.MainAvailable)
            //                try
            //                {
            //                    using (var thumbStream = _packageCentral.Main.GetLocalThumbnailStream())
            //                    {
            //                        // load image
            //                        if (thumbStream != null)
            //                        {
            //                            var bi = new BitmapImage();
            //                            bi.BeginInit();

            //                            // See https://stackoverflow.com/a/5346766/1600678
            //                            bi.CacheOption = BitmapCacheOption.OnLoad;

            //                            bi.StreamSource = thumbStream;
            //                            bi.EndInit();
            //                            this.AssetPic.Source = bi;
            //                        }
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
            //                }

            //            if (this.theOnlineConnection != null && this.theOnlineConnection.IsValid() &&
            //                this.theOnlineConnection.IsConnected())
            //                try
            //                {
            //                    using (var thumbStream = this.theOnlineConnection.GetThumbnailStream())
            //                    {
            //                        if (thumbStream != null)
            //                        {
            //                            using (var ms = new MemoryStream())
            //                            {
            //                                thumbStream.CopyTo(ms);
            //                                ms.Flush();
            //                                var bitmapdata = ms.ToArray();

            //                                var bi = (BitmapSource)new ImageSourceConverter().ConvertFrom(bitmapdata);
            //                                this.AssetPic.Source = bi;
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
            //                }

            //        }
            //        catch (Exception ex)
            //        {
            //            // no error, intended behaviour, as thumbnail might not exist / be faulty in some way
            //            // (not violating the spec)
            //            AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
            //        }
            //    }
            //}

            //// for all, prepare the display
            //PrepareDispEditEntity(
            //    _packageCentral.Main,
            //    DisplayElements.SelectedItems,
            //     _mainMenu?.IsChecked("EditMenu") == true,
            //     _mainMenu?.IsChecked("HintsMenu") == true,
            //     _mainMenu?.IsChecked("ShowIriMenu") == true,
            //    hightlightField: hightlightField);

        }


        public void StartSession()
        {
            


        }

        public void RebuildTree()
        {
            Log.Singleton.Error("Implement REBUILD TREE");
        }
        
    }
}