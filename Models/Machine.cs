using Framework;
using System.Linq;
using System.Text;
using System;
using System.Reflection;
using BetaComponents;
using FrameworkUI;
using System.IO;
using System.Collections.Generic;
using Utils;
using System.Configuration;
using Dr7Hmi.Properties;
using TwinCAT.Ads;
using System.Security.Permissions;

namespace Dr7Hmi
{
    public class Machine : Component
    {
        public Robot Robot { get; protected set; }

        public SimulatorSRS SimulatorSRS { get; protected set; }


        #region Revision

        [LiveValue]
        [DisplayName("Version")]
        [Category("HMI Infos")]
        public string TagHMI
        {
            get
            {
                if (_tag == null)
                {
                    var attribute = Assembly.GetAssembly(typeof(Machine)).GetCustomAttribute<AssemblyProductAttribute>();

                    if (attribute != null)
                    {
                        var tags = attribute.Product.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

                        if(tags.Count() == 2)
                            _tag = tags[1];
                    }

                    if(_tag == null)
                        _tag = "Not specified";
                }

                return _tag;
            }
        }
        string _tag = null;

        [LiveValue]
        [DisplayName("Revision HMI")]
        [Category("HMI Infos")]
        public string RevisionHMI
        {
            get
            {
                if (_revision == null)
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                    _revision = fvi.FileVersion;
                }

                return _revision;
            }
        }
        string _revision = null;
        #endregion
    }
}