﻿#region License
// Copyright (c) 2016-2017 Cisco Systems, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using SparkSDK;

namespace KitchenSink
{
    /// <summary>
    /// Interaction logic for Call.xaml
    /// </summary>
    public partial class CallView : UserControl
    {
        public IntPtr LocalViewHandle
        {
            get
            {
                return pblocalVideo.Handle;
            }
        }
        public IntPtr RemoteViewHandle
        {
            get
            {
                return pbRemoteVideo.Handle;
            }
        }

        public IntPtr RemoteShareViewHandle
        {
            get
            {
                return pbShareScreenVideo.Handle;
            }
        }

        public CallView()
        {
            InitializeComponent();
            this.Loaded += CallView_Loaded;
            this.DataContext = new CallViewModel(this);

            this.pbRemoteVideo.SizeChanged += PbRemoteVideo_SizeChanged;
            this.pblocalVideo.SizeChanged += PblocalVideo_SizeChanged;
            this.pbShareScreenVideo.SizeChanged += PbShareScreenVideo_SizeChanged;
        }

        private void PblocalVideo_SizeChanged(object sender, EventArgs e)
        {
            var viewModel = this.DataContext as CallViewModel;
            if (viewModel == null)
            {
                return;
            }
            viewModel.UpdateLocalVideoView();
        }

        private void PbRemoteVideo_SizeChanged(object sender, EventArgs e)
        {
            var viewModel = this.DataContext as CallViewModel;
            if (viewModel == null)
            {
                return;
            }
            viewModel.UpdateRemoteVideoView();
        }

        private void PbShareScreenVideo_SizeChanged(object sender, EventArgs e)
        {
            var viewModel = this.DataContext as CallViewModel;
            if (viewModel == null)
            {
                return;
            }
            viewModel.UpdateRemoteShareVideoView();
        }

        private void CallView_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CallViewModel;
            if (viewModel == null)
            {
                return;
            }

            switch (ApplicationController.Instance.ChangeViewCmd)
            {
                case ChangeViewCmd.CallViewDial:
                    viewModel.Dial(ApplicationController.Instance.CurSparkManager?.CurCalleeAddress);
                    break;
                case ChangeViewCmd.CallViewAnswer:
                    viewModel.Answer();
                    break;
                case ChangeViewCmd.CallViewDecline:
                    viewModel.Reject();
                    break;
                case ChangeViewCmd.None:
                    break;
                default:
                    break;
            }
        } 

        public void RefreshViews()
        {
            Dispatcher.Invoke(() =>
            { 
                this.pbRemoteVideo.Refresh();
                this.pblocalVideo.Refresh();
                this.pbShareScreenVideo.Refresh();
            });
        }
        public void RefreshRemoteViews()
        {
            Dispatcher.Invoke(() =>
            {
                this.pbRemoteVideo.Refresh();
            });
        }
        public void RefreshLocalViews()
        {
            Dispatcher.Invoke(() =>
            {
                this.pblocalVideo.Refresh();
            });
        }
        public void RefreshShareViews()
        {
            Dispatcher.Invoke(() =>
            {
                this.pbShareScreenVideo.Refresh();
            });
        }

        public void SwitchShareViewWithRemoteView(bool openShare)
        {
            Dispatcher.Invoke(() =>
            {
                if (openShare)
                {
                    var tmp = remoteVideoOrg.Child;
                    remoteVideoOrg.Child = null;
                    this.remoteVideoMini.Child = tmp;
                    this.wfhShareScreenVideo.Visibility = Visibility.Visible;
                    
                }
                else
                {
                    this.wfhShareScreenVideo.Visibility = Visibility.Collapsed;
                    var tmp = remoteVideoMini.Child;
                    remoteVideoMini.Child = null;
                    this.remoteVideoOrg.Child = tmp;
                }

                var viewModel = this.DataContext as CallViewModel;
                if (viewModel == null)
                {
                    return;
                }
                viewModel.UpdateRemoteVideoView();
            });

        }
    }
}
