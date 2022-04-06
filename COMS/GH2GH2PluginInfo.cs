extern alias GH1;
using System;
using Grasshopper.Framework;
using Grasshopper.UI;
using Grasshopper.UI.Icon;
using GH1.Grasshopper;
using GH2GH2.COMS;
using GH1::Grasshopper.Kernel;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Doc;
using System.Linq;
using Eto.Forms;
using System.Diagnostics;

namespace GH2GH2
{
    public sealed class GH2GH2PluginInfo : Plugin
    {
        internal static Dictionary<string, Guid> nn = new Dictionary<string, Guid>();
        public sealed override IIcon Icon
        {
            get
            {
                return null;
            }
        }
        private Dictionary<string, IIcon> _icons = new Dictionary<string, IIcon>();
        public sealed override string Author => "YANGDAOQIAN";

        public sealed override string Copyright => $"Copyright © 2010-{DateTime.UtcNow.Year} YANGDAOQIAN";

        public sealed override string Website => "";

        public sealed override string Contact => "";

        public sealed override string LicenseAgreement => "";

        [DebuggerHidden]
        public GH2GH2PluginInfo()
                   : base(new Guid("A4969AD8-1F9E-85BE-84A1-5FA98D1DAE04"), new Nomen("GH2GH2", "."), new Version(1, 0, 0, 0))
        {
            Dictionary<Guid, ObjectProxy> FF = (Dictionary<Guid, ObjectProxy>)Info.GetValue(null);

            foreach (ObjectProxy i in ObjectProxies.Proxies)
            {
                string iii = i.Nomen.Name + i.Nomen.Chapter + i.Nomen.Section;
                if (!nn.ContainsKey(iii))
                    nn.Add(iii, Guid.Empty);
            }
            try
            {
                foreach (IGH_ObjectProxy o in Instances.ComponentServer.ObjectProxies)
                {
                    if ((o.Type != null && typeof(IGH_Component).IsAssignableFrom(o.Type)) || o.Kind == GH_ObjectType.UserObject)
                    {
                        ObjectProxy proxy = null;
                        try
                        {
                            IGH_DocumentObject doc = null;
                            try
                            {
                                doc = o.CreateInstance();
                            }
                            catch
                            {
                                continue;
                            }
                            if (doc != null && doc.Category != string.Empty && doc.Category.Length > 0 && doc.Category != "Category")
                            {
                                string iiii = doc.Name + doc.Category + doc.SubCategory;
                                Nomen ni = new Nomen(doc.Name, doc.Description, doc.Category, doc.SubCategory, 2, Rank.Normal);
                                if (!nn.ContainsKey(iiii))
                                {
                                    nn.Add(iiii, o.Guid);
                                    GH2GH2_COM ccc = new GH2GH2_COM();
                                    ccc.COM = doc as GH_Component;
                                    ccc.set_dd(ni);
                                    proxy = new ObjectProxy(ccc, this);
                                    try
                                    {
                                        typeof(ObjectProxy).BaseType.GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(proxy, o.Guid);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        if (proxy != null)
                        {
                            if (!FF.ContainsKey(o.Guid))
                                FF.Add(o.Guid, proxy);
                        }
                    }
                }
            }
            catch
            {

            }
            Info.SetValue(null, FF);
        }
        private FieldInfo info;

        public FieldInfo Info
        {
            get
            {
                if (info == null)
                {
                    info = typeof(ObjectProxies).GetField("_proxies", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return info;
            }
        }
    }
}
