﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PylonC.NET;
using PylonC.NETSupportLibrary;
using OpenCvSharp;

namespace MT3
{
    partial class Form1 //BaslerSDK
    {
        /* Handles the click on the single frame button. */
        public void BaslerStart(uint index)
        {
            //Basler
            m_imageProvider.Open(index);
            m_imageProvider.UserSetLoad();
            m_imageProvider.Setup(appSettings.Width, appSettings.Height);
            m_imageProvider.SetupExposureTimeAbs(appSettings.Exposure * 1000.0); // [usec]
            m_imageProvider.SetupGain((long)appSettings.Gain); // 100-1023
            m_imageProvider.SetupFrameRate(appSettings.Framerate);
        }
        /* Handles the click on the single frame button. */
        public void BaslerEnd()
        {
            //Basler
            m_imageProvider.Close(); 
        }
        /* Handles the click on the single frame button. */
        private void toolStripButtonOneShot_Click(object sender, EventArgs e)
        {
            OneShot(); /* Starts the grabbing of one image. */
        }
        /* Handles the click on the continuous frame button. */
        private void toolStripButtonContinuousShot_Click(object sender, EventArgs e)
        {
            ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
        }

        /* Handles the click on the stop frame acquisition button. */
        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            Stop(); /* Stops the grabbing of images. */
        }

        /* Handles the event related to the occurrence of an error while grabbing proceeds. */
        private void OnGrabErrorEventCallback(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback), grabException, additionalErrorMessage);
                return;
            }
            ShowException(grabException, additionalErrorMessage);
        }

        /* Handles the event related to the removal of a currently open device. */
        private void OnDeviceRemovedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback));
                return;
            }
            /* Disable the buttons. */
            EnableButtons(false, false);
            /* Stops the grabbing of images. */
            Stop();
            /* Close the image provider. */
            CloseTheImageProvider();
            /* Since one device is gone, the list needs to be updated. */
            //UpdateDeviceList();
        }

        /* Handles the event related to a device being open. */
        private void OnDeviceOpenedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback));
                return;
            }
            /* The image provider is ready to grab. Enable the grab buttons. */
            EnableButtons(true, false);
        }

        /* Handles the event related to a device being closed. */
        private void OnDeviceClosedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback));
                return;
            }
            /* The image provider is closed. Disable all buttons. */
            EnableButtons(false, false);
        }

        /* Handles the event related to the image provider executing grabbing. */
        private void OnGrabbingStartedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback));
                return;
            }

            /* Do not update device list while grabbing to avoid jitter because the GUI-Thread is blocked for a short time when enumerating. */
            //updateDeviceListTimer.Stop();

            /* The image provider is grabbing. Disable the grab buttons. Enable the stop button. */
            EnableButtons(false, true);
        }

        /* Handles the event related to an image having been taken and waiting for processing. */
        private void OnImageReadyEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback));
                return;
            }

            try
            {
                /* Acquire the image from the image provider. Only show the latest image. The camera may acquire images faster than images can be displayed*/
                ImageProvider.Image image = m_imageProvider.GetCurrentImage(); // m_imageProvider.GetLatestImage();

                /* Check if the image has been removed in the meantime. */
                if (image != null)
                {
                    try
                    {
                        /* Display image */
                        //Pylon.ImageWindowDisplayImage<Byte>(0, image);//.Buffer, PylonC.NET.EPylonPixelType.PixelType_Mono8 ,1920,1200,0,0);
          
                        System.Object lockThis = new System.Object();
                        lock (lockThis)
                        {
                            // img_dmk は使わず、直接imgdata.imgにコピー
                            //
                            // マネージド配列の中身をアンマネージド配列にコピーする
                            // Marshal.Copyの引数
                            // 第一引数には、コピー元、第二引数にはstartIndex、
                            // 第三引数には、コピー先、第四引数にはコピーする長さ
                   //         Marshal.Copy(image.Buffer, 0, imgdata.img.ImageDataOrigin, image.Buffer.Length);
                            Marshal.Copy(image.Buffer, 0, imgdata.img.ImageDataOrigin, image.Buffer.Length -1 );
                   //         Console.WriteLine(" image[0]:{0} {1}",image.Buffer[0], image.Buffer[1]);
                        }

                        /* The processing of the image is done. Release the image buffer. */
                        m_imageProvider.ReleaseImage();
                        /* The buffer can be used for the next image grabs. */
                    }
                    catch (KeyNotFoundException)
                    {
                        MessageBox.Show("KeyNotFoundException:20");
                    }

                    try
                    {
                        // 表示画像反転 実装場所　要検討
                        if (appSettings.Flipmode == OpenCvSharp.FlipMode.X || appSettings.Flipmode == OpenCvSharp.FlipMode.Y)
                        {
                            Cv.Flip(imgdata.img, imgdata.img, appSettings.Flipmode);
                        }

                        //++frame_id;
                        //detect();
                    }
                    catch (KeyNotFoundException)
                    {
                        MessageBox.Show("KeyNotFoundException:21");
                    }

                    try{
                        ++frame_id;
                        detect();
                        imgdata_push_FIFO();
                    }
                    catch (KeyNotFoundException)
                    {
                        MessageBox.Show("KeyNotFoundException:22");
                    }
                }                 
            }
            catch (Exception e)
            {
                ShowException(e, m_imageProvider.GetLastErrorMessage());
            }

        }

        /* Handles the event related to the image provider having stopped grabbing. */
        private void OnGrabbingStoppedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback));
                return;
            }
            try
            {
                /* Enable device list update again */
                //updateDeviceListTimer.Start();

                /* The image provider stopped grabbing. Enable the grab buttons. Disable the stop button. */
                EnableButtons(m_imageProvider.IsOpen, false);
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("KeyNotFoundException:3");
            }
        }

        /* Helps to set the states of all buttons. */
        private void EnableButtons(bool canGrab, bool canStop)
        {
   //         toolStripButtonContinuousShot.Enabled = canGrab;
   //         toolStripButtonOneShot.Enabled = canGrab;
   //         toolStripButtonStop.Enabled = canStop;
        }

        /* Stops the image provider and handles exceptions. */
        private void Stop()
        {
            /* Stop the grabbing. */
            try
            {
                m_imageProvider.Stop();
            }
            catch (Exception e)
            {
                ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
        }

        /* Closes the image provider and handles exceptions. */
        private void CloseTheImageProvider()
        {
            /* Close the image provider. */
            try
            {
                m_imageProvider.Close();
            }
            catch (Exception e)
            {
                ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
        }

        /* Starts the grabbing of one image and handles exceptions. */
        private void OneShot()
        {
            try
            {
                m_imageProvider.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
        }

        /* Starts the grabbing of images until the grabbing is stopped and handles exceptions. */
        private void ContinuousShot()
        {
            try
            {
                m_imageProvider.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
        }

        /* Updates the list of available devices in the upper left area. */
        private void UpdateDeviceList()
        {
            try
            {
                /* Ask the device enumerator for a list of devices. */
                List<DeviceEnumerator.Device> list = DeviceEnumerator.EnumerateDevices();

                ListView.ListViewItemCollection items = null; ///= deviceListView.Items;

                /* Add each new device to the list. */
                foreach (DeviceEnumerator.Device device in list)
                {
                    bool newitem = true;
                    /* For each enumerated device check whether it is in the list view. */
                    foreach (ListViewItem item in items)
                    {
                        /* Retrieve the device data from the list view item. */
                        DeviceEnumerator.Device tag = item.Tag as DeviceEnumerator.Device;

                        if (tag.FullName == device.FullName)
                        {
                            /* Update the device index. The index is used for opening the camera. It may change when enumerating devices. */
                            tag.Index = device.Index;
                            /* No new item needs to be added to the list view */
                            newitem = false;
                            break;
                        }
                    }

                    /* If the device is not in the list view yet the add it to the list view. */
                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                        {
                            item.ToolTipText = device.Tooltip;
                        }
                        item.Tag = device;

                        /* Attach the device data. */
                        ///deviceListView.Items.Add(item);
                    }
                }

                /* Delete old devices which are removed. */
                foreach (ListViewItem item in items)
                {
                    bool exists = false;

                    /* For each device in the list view check whether it has not been found by device enumeration. */
                    foreach (DeviceEnumerator.Device device in list)
                    {
                        if (((DeviceEnumerator.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    /* If the device has not been found by enumeration then remove from the list view. */
                    if (!exists)
                    {
                        ///deviceListView.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
        }

        /* Shows exceptions in a message box. */
        private void ShowException(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* Closes the image provider when the window is closed. */
        private void MainForm_FormClosing(object sender, FormClosingEventArgs ev)
        {
            /* Stops the grabbing of images. */
            Stop();
            /* Close the image provider. */
            CloseTheImageProvider();
        }

        /* Handles the selection of cameras from the list box. The currently open device is closed and the first 
         selected device is opened. */
        private void deviceListView_SelectedIndexChanged(object sender, EventArgs ev)
        {
            /* Close the currently open image provider. */
            /* Stops the grabbing of images. */
            Stop();
            /* Close the image provider. */
            CloseTheImageProvider();

            /* Open the selected image provider. 
            if (deviceListView.SelectedItems.Count > 0)
            {
                // Get the first selected item.
                ListViewItem item = deviceListView.SelectedItems[0];
                // Get the attached device data. 
                DeviceEnumerator.Device device = item.Tag as DeviceEnumerator.Device;
                try
                {
                    // Open the image provider using the index from the device data. 
                    m_imageProvider.Open(device.Index);
                }
                catch (Exception e)
                {
                    ShowException(e, m_imageProvider.GetLastErrorMessage());
                }
            }*/
        }

        /* If the F5 key has been pressed update the list of devices. */
        private void deviceListView_KeyDown(object sender, KeyEventArgs ev)
        {
            if (ev.KeyCode == Keys.F5)
            {
                ev.Handled = true;
                /* Update the list of available devices in the upper left area. */
                UpdateDeviceList();
            }
        }

        /* Timer callback used for periodically checking whether displayed devices are still attached to the PC. */
        private void updateDeviceListTimer_Tick(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }
        /// <summary>
        ///  There are camera features, such as AcquisitionStart, that represent a command.
        ///  This function that loads the default set, illustrates how to execute a command feature.
        /// </summary>
        private static void demonstrateCommandFeature(PYLON_DEVICE_HANDLE hDev)
        {
            /* Before executing the user set load command, the user set selector must be
               set to the default set. Since we are focusing on the command feature,
               we skip the recommended steps for checking the availability of the user set
               related features and values. */

            /* Choose the default set (which includes one of the factory setups). */
            //Pylon.DeviceFeatureFromString(hDev, "UserSetSelector", "Default");

            /* Execute the user set load command. */
            Console.WriteLine("Loading the default settings.");
            Pylon.DeviceExecuteCommandFeature(hDev, "UserSetLoad");
        }
        /// <summary>
        /// UserSet Load
        /// </summary>
        public void ExeCommand(string featureName = "UserSetLoad" )
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                //string featureName = "UserSetLoad";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return ;
                }
                /* We want a command node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.CommandNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an command feature.");
                    return ;
                }

                GenApi.CommandExecute(hNode);
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// フレームレート値読み出し
        /// </summary>
        public double GetFrameRate()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;           /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;    /* Is the feature available? */
                double val = 0;      /* Properties of the feature. */

                featureName = "ResultingFrameRateAbs";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    /* Query the value range and the current value. */
                    val = GenApi.FloatGetValue(hNode);

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Expo値読み出し[us]
        /// </summary>
        public double GetExposureTime()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                double val = 0;      /* Properties of the feature. */

                featureName = "ExposureTimeAbs";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    /* Query the value range and the current value. */
                    val = GenApi.FloatGetValue(hNode);

                    return val; // [us]
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// ゲインの設定
        /// </summary>
        public long GetGain()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                featureName = "GainRaw";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.IntegerNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val ; 
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Timestamp
        /// </summary>
        public long GetBaslerInt(string featureName)
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                //string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                //featureName = "Timestamp";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.IntegerNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Statistic_Total_Buffer_Count 読み出し
        /// </summary>
        public long Get_Statistic_Total_Buffer_Count()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                featureName = "Statistic_Total_Buffer_Count";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Statistic_Failed_Buffer_Count 読み出し
        /// </summary>
        public long Get_Statistic_Failed_Buffer_Count()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                featureName = "Statistic_Failed_Buffer_Count";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Statistic_Buffer_Underrun_Countt 読み出し
        /// </summary>
        public long Get_Statistic_Buffer_Underrun_Count()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                featureName = "Statistic_Buffer_Underrun_Count";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Statistic_Total_Packet_Count 読み出し
        /// </summary>
        public long Get_Statistic_Total_Packet_Count()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                featureName = "Statistic_Total_Packet_Count";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        /// <summary>
        /// Statistic_Failed_Packet_Count 読み出し
        /// </summary>
        public long Get_Statistic_Failed_Packet_Count()
        {
            try
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;
                bool bval;                     /* Is the feature available? */
                string featureName;  /* Name of the feature used in this sample: AOI Width. */
                //bool isAvailable;              /* Is the feature available? */
                //double val = 0;      /* Properties of the feature. */
                long val, min, max, incr;      /* Properties of the feature. */
                featureName = "Statistic_Failed_Packet_Count";

                hNode = m_imageProvider.GetNodeFromDevice(featureName);
                if (!hNode.IsValid)
                {
                    Console.WriteLine("There is no feature named '" + featureName + "'.");
                    return 0;
                }
                /* We want a float feature node. */
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.FloatNode != nodeType)
                {
                    Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                    return 0;
                }

                bval = GenApi.NodeIsReadable(hNode);

                if (bval)
                {
                    min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                    max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                    incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                    val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                    return val;
                }
                return 0;
            }
            catch
            {
                // UpdateLastError();   /* Get the last error message here, because it could be overwritten by cleaning up. */
                try
                {
                    Close(); /* Try to close any open handles. */
                }
                catch
                {
                    /* Another exception cannot be handled. */
                }
                throw;
            }
        }
        
    }
}
