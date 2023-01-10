// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommanderTerminal.Adding;
using CommanderTerminal.CommandPad;
using CommanderTerminalCore.Configuration.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CommanderTerminal.Navigation.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationRootPage : Page
    {
        private const string DataIdentifier = "MyTabItem";
        public NavigationRootPage()
        {
            this.InitializeComponent();
            App.Current.Navigation = this;

            Tabs.TabItemsChanged += Tabs_TabItemsChanged;

            Loaded += TabViewWindowingSamplePage_Loaded;
        }

        private void TabViewWindowingSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            var currentWindow = App.Current.ShellWindow;
            if (currentWindow is { })
            {
                currentWindow.ExtendsContentIntoTitleBar = true;
                currentWindow.SetTitleBar(CustomDragRegion);
            }

            CustomDragRegion.MinWidth = 188;
        }

        private void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                App.Current.ShellWindow?.Close();
            }
            // If there is only one tab left, disable dragging and reordering of Tabs.
            else if (sender.TabItems.Count == 1)
            {
                sender.CanReorderTabs = false;
                sender.CanDragTabs = false;
            }
            else
            {
                sender.CanReorderTabs = true;
                sender.CanDragTabs = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetupWindow();
        }

        void SetupWindow()
        {
            Tabs.TabItems.Add(new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.World
                },
                Header = $"Welcome",
                Content = new WelcomePage()
            });

            Tabs.SelectedIndex = 0;

            Tabs.TabCloseRequested += (tab, e)=>{
                CommandPadPage ? commandPad = (e.Item as TabViewItem)?.Content as CommandPadPage;
                if(commandPad is { })
                {
                    commandPad.Close();
                }
            };

#if UNIVERSAL
            // Extend into the titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
#endif
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // To ensure that the tabs in the titlebar are not occluded by shell
            // content, we must ensure that we account for left and right overlays.
            // In LTR layouts, the right inset includes the caption buttons and the
            // drag region, which is flipped in RTL.

            // The SystemOverlayLeftInset and SystemOverlayRightInset values are
            // in terms of physical left and right. Therefore, we need to flip
            // then when our flow direction is RTL.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                ShellTitleBarInset.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                ShellTitleBarInset.MinWidth = sender.SystemOverlayRightInset;
            }

            // Ensure that the height of the custom regions are the same as the titlebar.
            CustomDragRegion.Height = ShellTitleBarInset.Height = sender.Height;
        }

        public void AddTabToTabs(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
        }

        // Create a new Window once the Tab is dragged outside.
        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            //var newPage = new TabViewWindowingSamplePage();

            //Tabs.TabItems.Remove(args.Tab);
            //newPage.AddTabToTabs(args.Tab);

            //var newWindow = WindowHelper.CreateWindow();
            //newWindow.ExtendsContentIntoTitleBar = true;
            //newWindow.Content = newPage;

            //newWindow.Activate();
        }

        private void Tabs_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            // We can only drag one tab at a time, so grab the first one...
            var firstItem = args.Tab;

            // ... set the drag data to the tab...
            args.Data.Properties.Add(DataIdentifier, firstItem);

            // ... and indicate that we can move it
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void Tabs_TabStripDrop(object sender, DragEventArgs e)
        {
            // This event is called when we're dragging between different TabViews
            // It is responsible for handling the drop of the item into the second TabView

            if (e.DataView.Properties.TryGetValue(DataIdentifier, out object obj))
            {
                // Ensure that the obj property is set before continuing.
                if (obj == null)
                {
                    return;
                }

                var destinationTabView = sender as TabView;
                if (destinationTabView is not { })
                {
                    return;
                }

                var destinationItems = destinationTabView.TabItems;
                if (destinationItems != null)
                {
                    // First we need to get the position in the List to drop to
                    var index = -1;

                    // Determine which items in the list our pointer is between.
                    for (int i = 0; i < destinationTabView.TabItems.Count; i++)
                    {
                        var item = destinationTabView.ContainerFromIndex(i) as TabViewItem;

                        if (item is { })
                        {
                            if (e.GetPosition(item).X - item.ActualWidth < 0)
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    // The TabView can only be in one tree at a time. Before moving it to the new TabView, remove it from the old.
                    var destinationTabViewListView = ((obj as TabViewItem)?.Parent as TabViewListView);
                    destinationTabViewListView?.Items.Remove(obj);

                    if (index < 0)
                    {
                        // We didn't find a transition point, so we're at the end of the list
                        destinationItems.Add(obj);
                    }
                    else if (index < destinationTabView.TabItems.Count)
                    {
                        // Otherwise, insert at the provided index.
                        destinationItems.Insert(index, obj);
                    }

                    // Select the newly dragged tab
                    destinationTabView.SelectedItem = obj;
                }
            }
        }

        // This method prevents the TabView from handling things that aren't text (ie. files, images, etc.)
        private void Tabs_TabStripDragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey(DataIdentifier))
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        public async void OpenAddDialog()
        {
            var addDialog = new ContentDialog();
            addDialog.XamlRoot = this.XamlRoot;
            addDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            addDialog.Title = "Open Commander";
            addDialog.IsPrimaryButtonEnabled = true;
            addDialog.IsSecondaryButtonEnabled = false;
            addDialog.PrimaryButtonText = "Open";
            //addDialog.SecondaryButtonText = "Don't Save";
            addDialog.CloseButtonText = "Close";
            addDialog.DefaultButton = ContentDialogButton.Primary;
            var addPage = new AddPage();

            addDialog.Content = addPage;
            addDialog.CloseButtonClick += (sender, e) =>
            {
                addPage.IsReadyToOpenHost = true;
            };
            addDialog.Closing += (sender, e) =>
            {
                if (!addPage.IsReadyToOpenHost)
                {
                    addPage.Log("Please choosing a host.", severity: InfoBarSeverity.Error);
                    e.Cancel = true;
                }
            };

            var dialogResult = await addDialog.ShowAsync();
            if (dialogResult == ContentDialogResult.None)
            {
                return;
            }

            var itemConfig = addPage.HostItemConfigEtt;
            if (itemConfig is not { })
            {
                return;
            }

            Tabs.TabItems.Add(new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.Link
                },
                Header = itemConfig.Name,
                Content = new CommandPadPage(itemConfig)
            });
            Tabs.SelectedIndex = Tabs.TabItems.Count - 1;
        }

        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            OpenAddDialog();
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }
    }
}
