/* 
  This sample illustrates how to access the different camera
  parameter types. It uses the low-level functions provided by GenApiC
  instead of those provided by pylonC.
*/

using System;
using System.Collections.Generic;
using PylonC.NET;

namespace GenApiParam
{
    class GenApiParam
    {
        /* This function demonstrates how to check the presence, readability, and writability
           of a feature. */
        private static void demonstrateAccessibilityCheck(PYLON_DEVICE_HANDLE hDev)
        {
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            string featureName;
            bool val, val_read, val_write;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Check to see if a feature is implemented at all. The 'Width' feature is likely to
            be implemented by just about every existing camera. */
            featureName = "Width";
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (hNode.IsValid)
            {
                /* Node exists. Check whether the feature is implemented. */
                val = GenApi.NodeIsImplemented(hNode);
            }
            else
            {
                /* Node does not exist. Feature is not implemented. */
                val = false;
            }
            Console.WriteLine("The '{0}' feature {1} implemented", featureName, val ? "is" : "is not");

            /* This feature does most likely not exist. */
            featureName = "Weirdness";
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (hNode.IsValid)
            {
                /* Node exists. Check whether the feature is implemented. */
                val = GenApi.NodeIsImplemented(hNode);
            }
            else
            {
                /* Node does not exist. Feature is not implemented. */
                val = false;
            }
            Console.WriteLine("The '{0}' feature {1} implemented.", featureName, val ? "is" : "is not");


            /* Although a feature is implemented by the device, it may not be available
               with the device in its current state. Check to see if the feature is currently 
               available. The GenApi.NodeIsAvailable sets val to false if either the feature
               is not implemented or if the feature is currently not available. */
            featureName = "BinningVertical";
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (hNode.IsValid)
            {
                /* Node exists. Check whether the feature is available. */
                val = GenApi.NodeIsAvailable(hNode);
            }
            else
            {
                /* Node does not exist. Feature is not implemented, and hence not available. */
                val = false;
            }
            Console.WriteLine("The '{0}' feature {1} implemented.", featureName, val ? "is" : "is not");

            /* If a feature is available, it could be read-only, write-only, or both
               readable and writable. Use the Pylon.DeviceFeatureIsReadable() and the
               Pylon.DeviceFeatureIsWritable() functions(). It is safe to call these functions
               for features that are currently not available or not implemented by the device.
               A feature that is not available or not implemented is neither readable nor writable.
               The readability and writability of a feature can change depending on the current 
               state of the device. For example, the Width parameter might not be writable when
               the camera is acquiring images. */

            featureName = "Width";
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (hNode.IsValid)
            {
                /* Node exists. Check whether the feature is readable. */
                val_read = GenApi.NodeIsReadable(hNode);
                val_write = GenApi.NodeIsReadable(hNode);
            }
            else
            {
                /* Node does not exist. Feature is neither readable nor writable. */
                val_read = val_write = false;
            }
            Console.WriteLine("The '{0}' feature {1} readable.", featureName, val_read ? "is" : "is not");
            Console.WriteLine("The '{0}' feature {1} writable.", featureName, val_write ? "is" : "is not");
            Console.WriteLine("");
        }


        /* This function demonstrates how to handle integer camera parameters. */
        private static void demonstrateIntFeature(PYLON_DEVICE_HANDLE hDev)
        {
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            string featureName = "Width";  /* Name of the feature used in this sample: AOI Width. */
            long val, min, max, incr;      /* Properties of the feature. */
            EGenApiNodeType nodeType;
            bool bval;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the feature node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (!hNode.IsValid)
            {
                Console.WriteLine("There is no feature named '" + featureName + "'.");
                return;
            }

            /* We want an integer feature node. */
            nodeType = GenApi.NodeGetType(hNode);

            if (EGenApiNodeType.IntegerNode != nodeType)
            {
                Console.WriteLine("'" + featureName + "' is not an integer feature.");
                return;
            }

            /* 
               Query the current value, the range of allowed values, and the increment of the feature. 
               For some integer features, you are not allowed to set every value within the 
               value range. For example, for some cameras the Width parameter must be a multiple 
               of 2. These constraints are expressed by the increment value. Valid values 
               follow the rule: val >= min && val <= max && val == min + n * inc.
            */

            bval = GenApi.NodeIsReadable(hNode);


            if (bval)
            {
                min = GenApi.IntegerGetMin(hNode);       /* Get the minimum value. */
                max = GenApi.IntegerGetMax(hNode);       /* Get the maximum value. */
                incr = GenApi.IntegerGetInc(hNode);      /* Get the increment value. */
                val = GenApi.IntegerGetValue(hNode);     /* Get the current value. */

                Console.WriteLine("{0}: min= {1}  max= {2}  incr={3}  Value={4}", featureName, min, max, incr, val);

                bval = GenApi.NodeIsWritable(hNode);

                if (bval)
                {
                    /* Set the Width parameter half-way between minimum and maximum. */
                    val = min + (max - min) / incr / 2 * incr;
                    Console.WriteLine("Setting {0} to {1}", featureName, val);
                    GenApi.IntegerSetValue(hNode,val);
                }
                else
                    Console.WriteLine("Cannot set value for feature '{0}' - node not writable.", featureName);
            }
            else
                Console.WriteLine("Cannot read feature '{0}' - node not readable.", featureName);
        }


        /* Some features are floating point features. This function illustrates how to set and get floating
           point parameters. */
        private static void demonstrateFloatFeature(PYLON_DEVICE_HANDLE hDev)
        {
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            string featureName = "Gamma";  /* The name of the feature used. */
            bool bval;                     /* Is the feature available? */
            double min, max, value;        /* Value range and current value. */
            EGenApiNodeType nodeType;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the feature node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (!hNode.IsValid)
            {
                Console.WriteLine("There is no feature named '" + featureName + "'.");
                return;
            }

            /* We want a float feature node. */
            nodeType = GenApi.NodeGetType(hNode);

            if (EGenApiNodeType.FloatNode != nodeType)
            {
                Console.WriteLine("'" + featureName + "' is not an floating-point feature.");
                return;
            }

            bval = GenApi.NodeIsReadable(hNode);

            if (bval)
            {
                /* Query the value range and the current value. */
                min = GenApi.FloatGetMin(hNode);
                max = GenApi.FloatGetMax(hNode);
                value = GenApi.FloatGetValue(hNode);

                Console.WriteLine("{0}: min = {1}, max = {2}, value = {3}", featureName, min, max, value);

                /* Set a new value. */
                bval = GenApi.NodeIsWritable(hNode);

                if (bval)
                {
                    value = 0.5 * (min + max);
                    Console.WriteLine("Setting {0} to {1}", featureName, value);
                    GenApi.FloatSetValue(hNode, value);
                }
                else
                    Console.WriteLine("Cannot set value for feature '{0}' - node not writable.", featureName);
            }
            else
                Console.WriteLine("Cannot read feature '{0}' - node not readable.", featureName);
        }


        /* Some features are boolean features that can be switched on and off. 
           This function illustrates how to access boolean features. */
        private static void demonstrateBooleanFeature(PYLON_DEVICE_HANDLE hDev)
        {
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            string featureName = "GammaEnable";   /* The name of the feature. */
            bool value, bval;                    /* The value of the feature. */
            EGenApiNodeType nodeType;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the feature node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (!hNode.IsValid)
            {
                Console.WriteLine("There is no feature named '" + featureName + "'.");
                return;
            }

            /* We want a boolean feature node. */
            nodeType = GenApi.NodeGetType(hNode);

            if (EGenApiNodeType.BooleanNode != nodeType)
            {
                Console.WriteLine("'" + featureName + "' is not a boolean feature.");
                return;
            }

            /* Check to see if the feature is readable. */
            bval = GenApi.NodeIsReadable(hNode);

            if (bval)
            {
                /* Retrieve the current state of the feature. */
                value = GenApi.BooleanGetValue(hNode);

                Console.WriteLine("The {0} feature is {1}.", featureName, value ? "on" : "off");

                /* Set a new value. */
                bval = GenApi.NodeIsWritable(hNode);

                if (bval)
                {
                    value = (bool)!value;  /* New value. */
                    Console.WriteLine("Switching the {0} feature {1}.", featureName, value ? "on" : "off");
                    GenApi.BooleanSetValue(hNode, value);
                }
                else
                    Console.WriteLine("Cannot set value for feature '{0}' - node not writable.", featureName);
            }
            else
                Console.WriteLine("Cannot read feature '{0}' - node not readable.", featureName);
        }


        /*
          Regardless of the parameter's type, any parameter value can be retrieved as a string. Likewise, each parameter
          can be set by passing in a string. This function illustrates how to set and get the 
          Width parameter as a string. As demonstrated above, the Width parameter is of the integer type.  
          */
        private static void demonstrateFromStringToString(PYLON_DEVICE_HANDLE hDev)
        {
            string featureName = "Width";   /* The name of the feature. */
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            EGenApiNodeType nodeType;
            bool bval;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the feature node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (!hNode.IsValid)
            {
                Console.WriteLine("There is no feature named '" + featureName + "'.");
                return;
            }

            /* We want an integer feature node. */
            nodeType = GenApi.NodeGetType(hNode);

            if (EGenApiNodeType.IntegerNode != nodeType)
            {
                Console.WriteLine("'" + featureName + "' is not an integer feature.");
                return;
            }

            /* Check to see if the feature is readable. */
            bval = GenApi.NodeIsReadable(hNode);

            if (bval)
            {
                string valueString;

                /* Get the value of a feature as a string. */
                valueString = GenApi.NodeToString(hNode);

                Console.WriteLine("{0}: value string = {1}", featureName, valueString);

                /* A feature can be set as a string using the GenApi.NodeFromString() function. 
                   If the content of a string can not be converted to the type of the feature, an 
                   error is returned. */
                bval = GenApi.NodeIsWritable(hNode);

                if (bval)
                {
                    try
                    {
                        GenApi.NodeFromString(hNode, "fourty-two"); /* Can not be converted to an integer. */
                    }
                    catch (Exception e)
                    {
                        /* Retrieve the error message. */
                        string msg = GenApi.GetLastErrorMessage() + "\n" + GenApi.GetLastErrorDetail();
                        Console.WriteLine("Exception caught:");
                        Console.WriteLine(e.Message);
                        if (msg != "\n")
                        {
                            Console.WriteLine("Last error message:");
                            Console.WriteLine(msg);
                        }
                    }
                }
                else
                    Console.WriteLine("Cannot set value for feature '{0}' - node not writable.", featureName);
            }
            else
                Console.WriteLine("Cannot read feature '{0}' - node not readable.", featureName);
        }


        /* There are camera features that behave like enumerations. These features can take a value from a fixed 
           set of possible values. One example is the pixel format feature. This function illustrates how to deal with 
           enumeration features. 
        */
        private static void demonstrateEnumFeature(PYLON_DEVICE_HANDLE hDev)
        {
            string featureName = "PixelFormat";
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            EGenApiNodeType nodeType;
            bool bval;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the feature node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (!hNode.IsValid)
            {
                Console.WriteLine("There is no feature named '" + featureName + "'.");
                return;
            }

            /* We want an enumeration feature node. */
            nodeType = GenApi.NodeGetType(hNode);

            if (EGenApiNodeType.EnumerationNode != nodeType)
            {
                Console.WriteLine("'" + featureName + "' is not an enumeration feature.");
                return;
            }

            /* Check to see if the feature is readable. */
            bval = GenApi.NodeIsReadable(hNode);

            /* The allowed values for an enumeration feature are represented as strings. Use the 
            GenApi.NodeFromString and GenApi.NodeToString methods for setting and getting
            the value of an enumeration feature. */

            if (bval)
            {
                /* Symbolic names of pixel formats. */
                string symMono8 = "Mono8",
                       symMono16 = "Mono16",
                       symYUV422Packed = "YUV422Packed";

                string value;   /* The current value of the feature. */
                bool supportsMono8,
                     supportsYUV422Packed,
                     supportsMono16;
                NODE_HANDLE hEntry;


                /* Get the current value of the enumeration feature. */
                value = GenApi.NodeToString(hNode);

                Console.WriteLine("PixelFormat: {0}", value);

                /*
                For an enumeration feature, the pylon Viewer's "Feature Documentation" window lists the
                names of the possible values. Some of the values may not be supported by the device. 
                To check if a certain "SomeValue" value for a "SomeFeature" feature can be set, call the 
                GenApi.NodeIsAvailable() function on the node of the entry. 
                */
                /* Check to see if the Mono8 pixel format can be set. */
                hEntry = GenApi.EnumerationGetEntryByName(hNode, symMono8);
                supportsMono8 = hEntry.IsValid && GenApi.NodeIsAvailable(hEntry);
                Console.WriteLine("{0} {1} a supported value for the pixel format feature.", symMono8, supportsMono8 ? "is" : "is not");

                /* Check to see if the YUV422Packed pixel format can be set. */
                hEntry = GenApi.EnumerationGetEntryByName(hNode, symYUV422Packed);
                supportsYUV422Packed = hEntry.IsValid && GenApi.NodeIsAvailable(hEntry);
                Console.WriteLine("{0} {1} a supported value for the pixel format feature.", symYUV422Packed, supportsYUV422Packed ? "is" : "is not");

                /* Check to see if the Mono16 pixel format can be set. */
                hEntry = GenApi.EnumerationGetEntryByName(hNode, symMono16);
                supportsMono16 = hEntry.IsValid && GenApi.NodeIsAvailable(hEntry);
                Console.WriteLine("{0} {1} a supported value for the pixel format feature.", symMono16, supportsMono16 ? "is" : "is not");


                /* Before writing a value, we recommend checking to see if the enumeration feature is
                currently writable. */
                bval = GenApi.NodeIsWritable(hNode);

                if (bval)
                {
                    /* The PixelFormat feature is writable. Set it to one of the supported values. */
                    if (supportsMono16)
                    {
                        Console.WriteLine("Setting PixelFormat to Mono16.");
                        GenApi.NodeFromString(hNode, symMono16);

                    }
                    else if (supportsYUV422Packed)
                    {
                        Console.WriteLine("Setting PixelFormat to YUV422Packed.");
                        GenApi.NodeFromString(hNode, symYUV422Packed);

                    }
                    else if (supportsMono8)
                    {
                        Console.WriteLine("Setting PixelFormat to Mono8.");
                        GenApi.NodeFromString(hNode, symMono8);
                    }

                    /* Reset the PixelFormat feature to its previous value. */
                    GenApi.NodeFromString(hNode, value);

                }
                else
                    Console.WriteLine("Cannot set value for feature '{0}' - node not writable.", featureName);
            }
            else
                Console.WriteLine("Cannot read feature '{0}' - node not readable.", featureName);
        }


        /* Enumerate all possible entries for an enumerated feature. For every entry, a selection
           of properties is displayed. A loop similar to the one shown below may be part of a
           GUI program that wants to fill the entries of a menu. */
        private static void demonstrateEnumIteration(PYLON_DEVICE_HANDLE hDev)
        {
            string featureName = "PixelFormat";
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            EGenApiNodeType nodeType;
            bool bval;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the feature node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, featureName);
            if (!hNode.IsValid)
            {
                Console.WriteLine("There is no feature named '" + featureName + "'.");
                return;
            }

            /* We want an enumeration feature node. */
            nodeType = GenApi.NodeGetType(hNode);

            if (EGenApiNodeType.EnumerationNode != nodeType)
            {
                Console.WriteLine("'" + featureName + "' is not an enumeration feature.");
                return;
            }

            /* Check to see if the feature is readable. */
            bval = GenApi.NodeIsReadable(hNode);

            if (bval)
            {
                uint max, i;

                /* Check entries. */
                max = GenApi.EnumerationGetNumEntries(hNode);

                /* Write out header. */
                Console.WriteLine("Allowed values for feature '{0}':\n" +
                                  "--------------",
                                  featureName);

                /* A loop to visit every enumeration entry node once. */
                for (i = 0; i < max; i++)
                {
                    NODE_HANDLE hEntry;
                    string name, displayName, description;
                    bool avail;

                    /* Get handle for enumeration entry node. */
                    hEntry = GenApi.EnumerationGetEntryByIndex(hNode, i);

                    /* Get node name. */
                    name = GenApi.NodeGetName(hEntry);

                    /* Get display name. */
                    displayName = GenApi.NodeGetDisplayName(hEntry);

                    /* Get description. */
                    description = GenApi.NodeGetDescription(hEntry);

                    /* Get availability. */
                    avail = GenApi.NodeIsAvailable(hEntry);

                    /* Write out results. */
                    Console.WriteLine("Node name:    {0}\n" +
                           "Display name: {1}\n" +
                           "Description:  {2}\n" +
                           "Available:    {3}\n" +
                           "--------------",
                           name, displayName, description, avail ? "yes" : "no");
                }
            }
            else
                Console.WriteLine("Cannot read feature '{0}' - node not readable.", featureName);
        }



        /* Traverse the feature tree, displaying all categories and all features. */
        private static void handleCategory(NODE_HANDLE hRoot, string indentation)
        {
            uint numfeat, i;

            string rootname = GenApi.NodeGetName(hRoot);

            /* Get the number of feature nodes in this category. */
            numfeat = GenApi.CategoryGetNumFeatures(hRoot);

            Console.WriteLine("{0} category has {1} children:", indentation + rootname, numfeat);

            indentation += "  ";

            /* Now loop over all feature nodes. */
            for (i = 0; i < numfeat; ++i)
            {
                NODE_HANDLE hNode;
                EGenApiNodeType nodeType;

                /* Get next feature node and check its type. */
                hNode = GenApi.CategoryGetFeatureByIndex(hRoot, i);
                nodeType = GenApi.NodeGetType(hNode);

                if (EGenApiNodeType.Category != nodeType)
                {
                    /* A regular feature. */
                    EGenApiAccessMode am;
                    string amode;

                    string name = GenApi.NodeGetName(hNode);

                    am = GenApi.NodeGetAccessMode(hNode);

                    switch (am)
                    {
                        case EGenApiAccessMode.NI:
                            amode = "not implemented";
                            break;
                        case EGenApiAccessMode.NA:
                            amode = "not available";
                            break;
                        case EGenApiAccessMode.WO:
                            amode = "write only";
                            break;
                        case EGenApiAccessMode.RO:
                            amode = "read only";
                            break;
                        case EGenApiAccessMode.RW:
                            amode = "read and write";
                            break;
                        default:
                            amode = "undefined";
                            break;
                    }

                    Console.WriteLine("{0} feature - access: {1}", indentation + name, amode);
                }
                else
                    /* Another category node. */
                    handleCategory(hNode, indentation + "  ");
            }
        }

        private static void demonstrateCategory(PYLON_DEVICE_HANDLE hDev)
        {
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the root node. */
            hNode = GenApi.NodeMapGetNode(hNodeMap, "Root");

            handleCategory(hNode, "");
        }



        /* There are camera features, such as AcquisitionStart, that represent a command. 
           This function that loads the default set, illustrates how to execute a command feature.  */
        private static void demonstrateCommandFeature(PYLON_DEVICE_HANDLE hDev)
        {
            string selectorName = "UserSetSelector",
                                commandName = "UserSetLoad";
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hCommand, hSelector;
            EGenApiNodeType nodeType;
            bool bval;

            /* Get a handle for the device's node map. */
            hNodeMap = Pylon.DeviceGetNodeMap(hDev);

            /* Look up the command node. */
            hCommand = GenApi.NodeMapGetNode(hNodeMap, commandName);
            if (!hCommand.IsValid)
            {
                Console.WriteLine("There is no node named '" + commandName + "'.");
                return;
            }

            /* Look up the selector node. */
            hSelector = GenApi.NodeMapGetNode(hNodeMap, selectorName);
            if (!hSelector.IsValid)
            {
                Console.WriteLine("There is no node named '" + selectorName + "'.");
                return;
            }

            /* We want a command feature node. */
            nodeType = GenApi.NodeGetType(hCommand);

            if (EGenApiNodeType.CommandNode != nodeType)
            {
                Console.WriteLine("'" + selectorName + "' is not a command feature.");
                return;
            }

            /* Before executing the user set load command, the user set selector must be
               set to the default set. */

            /* Check to see if the selector is writable. */
            bval = GenApi.NodeIsWritable(hSelector);

            if (bval)
            {
                /* Choose the default set (which includes one of the factory setups). */
                GenApi.NodeFromString(hSelector, "Default");

            }
            else
                Console.WriteLine("Cannot set selector '{0}' - node not writable.", selectorName);


            /* Check to see if the command is writable. */
            bval = GenApi.NodeIsWritable(hCommand);

            if (bval)
            {
                /* Execute the user set load command. */
                Console.WriteLine("Loading the default set.");
                GenApi.CommandExecute(hCommand);

            }
            else
                Console.WriteLine("Cannot execute command '{0}' - node not writable.", commandName);
        }

        static void mmMain(string[] args)
        {
            PYLON_DEVICE_HANDLE hDev = new PYLON_DEVICE_HANDLE();          /* Handle for the pylon device. */
            try
            {
                uint numDevices;    /* Number of devices available. */

#if DEBUG
                /* This is a special debug setting needed only for GigE cameras.
                See 'Building Applications with pylon' in the programmer's guide. */
                Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "300000" /*ms*/);
#endif

                /* Before using any pylon methods, the pylon runtime must be initialized. */
                Pylon.Initialize();

                /* Enumerate all camera devices. You must call 
                Pylon.EnumerateDevices() before creating a device. */
                numDevices = Pylon.EnumerateDevices();

                if (0 == numDevices)
                {
                    throw new Exception("No devices found.");
                }

                /* Get a handle for the first device found.  */
                hDev = Pylon.CreateDeviceByIndex(0);

                /* Before using the device, it must be opened. Open it for configuring
                parameters and for grabbing images. */
                Pylon.DeviceOpen(hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);

                /* Print out the name of the camera we are using. */
                {
                    bool isReadable;

                    isReadable = Pylon.DeviceFeatureIsReadable(hDev, "DeviceModelName");
                    if (isReadable)
                    {
                        string name = Pylon.DeviceFeatureToString(hDev, "DeviceModelName");
                        Console.WriteLine("Using camera {0}", name);
                    }
                }

                /* Demonstrate how to check the accessibility of a feature. */
                demonstrateAccessibilityCheck(hDev);
                Console.WriteLine("");

                /* Demonstrate how to handle integer camera parameters. */
                demonstrateIntFeature(hDev);
                Console.WriteLine("");

                /* Demonstrate how to handle floating point camera parameters. */
                demonstrateFloatFeature(hDev);
                Console.WriteLine("");

                /* Demonstrate how to handle boolean camera parameters. */
                demonstrateBooleanFeature(hDev);
                Console.WriteLine("");

                /* Each feature can be read as a string and also set as a string. */
                demonstrateFromStringToString(hDev);
                Console.WriteLine("");

                /* Demonstrate how to handle enumeration camera parameters. */
                demonstrateEnumFeature(hDev);
                Console.WriteLine("");

                /* Demonstrate how to iterate enumeration entries. */
                demonstrateEnumIteration(hDev);
                Console.WriteLine("");

                /* Demonstrate how to execute actions. */
                demonstrateCommandFeature(hDev);
                Console.WriteLine("");

                /* Display category nodes. */
                demonstrateCategory(hDev);

                /* Clean up. Close and release the pylon device. */
                Pylon.DeviceClose(hDev);
                Pylon.DestroyDevice(hDev);


                /* Shut down the pylon runtime system. Don't call any pylon method after 
                   calling Pylon.Terminate(). */
                Pylon.Terminate();

                Console.Error.WriteLine("\nPress enter to exit.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                /* Retrieve the error message. */
                string msg = GenApi.GetLastErrorMessage() + "" + GenApi.GetLastErrorDetail();
                Console.Error.WriteLine("Exception caught:");
                Console.Error.WriteLine(e.Message);
                if (msg.Length > 0)
                {
                    Console.Error.WriteLine("Last error message:");
                    Console.Error.WriteLine(msg);
                }

                try
                {
                    if (hDev.IsValid)
                    {
                        /* ... Close and release the pylon device. */
                        if (Pylon.DeviceIsOpen(hDev))
                        {
                            Pylon.DeviceClose(hDev);
                        }
                        Pylon.DestroyDevice(hDev);
                    }
                }
                catch (Exception)
                {
                    /*No further handling here.*/
                }

                Pylon.Terminate();  /* Releases all pylon resources. */

                Console.Error.WriteLine("\nPress enter to exit.");
                Console.ReadLine();

                Environment.Exit(1);
            }
        }
    }
}
