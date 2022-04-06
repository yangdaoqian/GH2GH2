extern alias GH1;
using Grasshopper.Components;
using Grasshopper.UI;
using GrasshopperIO;
using GH1.Grasshopper.Kernel;
using Grasshopper.UI.Icon;
using System.Drawing;
using System.IO;
using GH1::Grasshopper.Kernel.Data;
using System.Collections;
using System;
using GH1::Grasshopper.Kernel.Types;
using Grasshopper.Doc;
using Eto.Forms;
using System.Reflection;
using Grasshopper.UI.TabbedPanel;
using Grasshopper.Framework;
using Grasshopper;
using Grasshopper.UI.Skinning;
using Grasshopper.Extensions;
using Grasshopper.UI.Canvas;
using Grasshopper.Data;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Parameters;
using GH1::Grasshopper.Kernel.Parameters;

namespace GH2GH2.COMS
{
    [IoId("0cb23d43-ba51-0920-3a09-bc5524c674bb")]
    public sealed class GH2GH2_COM : Component
    {
        internal class GH_Iterator : IGH_DataAccess
        {
            private IDataAccess m_dataaccess;
            private GH_Component m_component;

            internal IDataAccess Component => m_dataaccess;

            public int Iteration => 1;

            internal GH_Iterator(IDataAccess parent, GH_Component component)
            {
                if (parent == null)
                {
                    throw new ArgumentNullException("parent");
                }
                m_dataaccess = parent;
                m_component = component;
            }

            public void IncrementIteration()
            {
            }

            public void DisableGapLogic()
            {
            }

            public void DisableGapLogic(int paramIndex)
            {
            }

            public GH_Path ParameterTargetPath(int paramIndex)
            {
                return null;
            }

            public int ParameterTargetIndex(int paramIndex)
            {
                return 0;

            }

            public void AbortComponentSolution()
            {
                m_dataaccess.Solution.Cancel();
            }

            public System.Collections.Generic.List<T> Util_RemoveNullRefs<T>(System.Collections.Generic.List<T> L)
            {
                return null;
            }

            public int Util_CountNullRefs<T>(System.Collections.Generic.List<T> L)
            {
                return 0;
            }

            public int Util_CountNonNullRefs<T>(System.Collections.Generic.List<T> L)
            {
                return 0;
            }

            public bool Util_EnsureNonNullCount<T>(System.Collections.Generic.List<T> L, int N)
            {
                return false;
            }

            public int Util_FirstNonNullItem<T>(System.Collections.Generic.List<T> L)
            {
                return 0;
            }
            public bool SetData(int paramIndex, object data)
            {
                Type t = data.GetType();
                if (typeof(IGH_Goo).IsAssignableFrom(t))
                {
                    data = t.GetProperty("Value").GetValue(data);
                }
                m_dataaccess.SetItem(paramIndex, data);
                return true;
            }

            public bool SetData(int paramIndex, object data, int itemIndexOverride)
            {
                return SetData(paramIndex, data);
            }

            public bool SetData(string paramName, object data)
            {
                int i = m_component.Params.IndexOfInputParam(paramName);
                SetData(i, data);
                return true;
            }




            public bool SetDataList(int paramIndex, IEnumerable data)
            {
                try
                {
                    IEnumerator enumerator = data.GetEnumerator();
                    //Type t = null;
                    //Type tt = null;
                    //object lll = null;
                    List<object> list = new List<object>();
                    try
                    {

                        while (enumerator.MoveNext())
                        {
                            object item = enumerator.Current;
                            if (item is IGH_Goo)
                            {
                                item = item.GetType().GetProperty("Value").GetValue(item);
                            }
                            //if (t == null)
                            //{            
                            //    //list.Add(item);
                            //    //t = item.GetType();
                            //    //tt = typeof(List<>).MakeGenericType(typeof(object));
                            //    //lll = Activator.CreateInstance(tt);
                            //}
                            list.Add(item);
                            //tt.GetMethod("Add").Invoke(lll, new object[] { item });
                        }

                    }
                    finally
                    {
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                    try
                    {
                        m_dataaccess.SetTwig(paramIndex, list.ToArray());
                    }
                    catch
                    {
                        try
                        {
                            m_dataaccess.SetTwig(paramIndex, (ITwig)(typeof(Garden).GetMethod("ITwigFromList", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { list, null, null })));
                        }
                        catch
                        {
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    m_dataaccess.AddError("ERROR: "+ex.Message,"");
                    return false;
                }
            }

            public bool SetDataList(int paramIndex, IEnumerable data, int listIndexOverride)
            {
                return SetDataList(paramIndex, data);
            }

            public bool SetDataList(string paramName, IEnumerable data)
            {
                int i = m_component.Params.IndexOfInputParam(paramName);
                SetDataList(i, data);
                return true;
            }
            internal ITree Trees(IGH_Structure outputs)
            {
                if (outputs == null)
                    return null;
                ITwig[] twigs = new ITwig[outputs.PathCount];
                for (int i = 0; i < outputs.PathCount; i++)
                {
                    twigs[i] = Twigs(outputs.get_Branch(outputs.Paths[i]));
                }
                Grasshopper.Data.Paths p = new Grasshopper.Data.Paths(outputs.Paths.ToList().ConvertAll(i => new Grasshopper.Data.Path(i.Indices)));
                return (ITree)typeof(Tree).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Paths), typeof(ITwig[]) }, null).Invoke(new object[] { p, twigs });
            }
            internal Twig<object> Twigs(IList list)
            {
                if (list != null && list.Count > 0)
                {
                    Type t = typeof(Twig<object>);
                    object o = Activator.CreateInstance(t, true);
                    t.GetMethod("AddRange", new Type[] { typeof(IEnumerable<object>) }).Invoke(o, new object[] { ConvertIListToList<object>(list) });
                    return (Twig<object>)o;
                }
                return null;
            }
            private IEnumerable<T> ConvertIListToList<T>(IList gbList) where T : class
            {
                if (gbList != null && gbList.Count > 0)
                {
                    List<T> list = new List<T>();
                    for (int i = 0; i < gbList.Count; i++)
                    {
                        T temp = gbList[i] as T;

                        if (temp != null)
                            list.Add(temp);
                    }
                    return list;
                }
                return null;
            }

            public bool SetDataTree(int paramIndex, IGH_DataTree tree)
            {
                Param_GenericObject param_Generic = new Param_GenericObject();
                tree.MergeWithParameter(param_Generic);
                m_dataaccess.SetTree(paramIndex, Trees(param_Generic.VolatileData));
                return true;
            }

            public bool SetDataTree(int paramIndex, IGH_Structure tree)
            {
                m_dataaccess.SetTree(paramIndex, Trees(tree));
                return true;
            }

            public bool BlitData<Q>(int paramIndex, GH_Structure<Q> tree, bool overwrite) where Q : IGH_Goo
            {
                return true;
            }

            public bool GetData<T>(int index, ref T destination)
            {
                object f = null;
                m_dataaccess.GetItem<object>(index, out f);
                if (typeof(IGH_Goo).IsAssignableFrom(typeof(T)))
                {
                    object uuu = Activator.CreateInstance(typeof(T));
                    typeof(T).BaseType.GetProperty("Value").SetValue(uuu, f);
                    destination = (T)(uuu);
                    return true;
                }
                if (f != null)
                {
                    destination = (T)f;
                    return true;
                }
                return false;
            }

            public bool GetData<T>(string name, ref T destination)
            {
                int i = m_component.Params.IndexOfOutputParam(name);
                return GetData(i, ref destination);

            }

            public bool GetDataList<T>(int index, List<T> list)
            {
                Twig<object> vs = null;
                try
                {
                    m_dataaccess.GetTwig(index, out vs);
                }
                catch (Exception ex)
                {
                    m_dataaccess.AddError("GETLISTY.", ex.Message);
                }

                PropertyInfo property = typeof(T).GetProperty("Value");
                if (typeof(IGH_Goo).IsAssignableFrom(typeof(T)))
                {
                    list = vs.NonNullItems.ToList().ConvertAll(i =>
                      {
                          object ai = Activator.CreateInstance(typeof(T));
                          property.SetValue(ai, i);
                          return (T)ai;
                      });
                    return true;
                }
                if (vs != null)
                {
                    list = vs.NonNullItems.ToList().ConvertAll(i => (T)i);
                    return true;
                }
                return false;
            }

            public bool GetDataList<T>(string name, System.Collections.Generic.List<T> list)
            {
                int i = m_component.Params.IndexOfOutputParam(name);
                return GetDataList(i, list);
            }

            public bool GetDataTree<T>(int index, out GH_Structure<T> tree) where T : IGH_Goo
            {
                m_dataaccess.GetITree(index, out ITree trees);
                tree = new GH_Structure<T>();
                Grasshopper.Data.Paths p = trees.Paths;
                int i = 0;
                foreach (Grasshopper.Data.Path pi in p)
                {
                    tree.AppendRange(trees.AllTwigs.ElementAt(0).NonNullItems.ToList().ConvertAll(i => (T)i), new GH_Path(pi.ToArray()));
                }
                return true;
            }

            public bool GetDataTree<T>(string name, out GH_Structure<T> tree) where T : IGH_Goo
            {
                int i = m_component.Params.IndexOfInputParam(name);
                return GetDataTree(i, out tree);
            }
        }

        public GH_Component COM
        {
            get
            {
                return m_component;
            }
            set
            {
                m_component = value;
                if (m_component != null)
                {
                    m_IconInternal = AbstractIcon.FromBitmap(new Eto.Drawing.Bitmap(get_bitmap()));
                    set_dd(new Nomen(m_component.Name, m_component.Description, m_component.Category, m_component.SubCategory, 2, Rank.Normal));
                }
            }
        }
        internal void set_dd(Nomen b)
        {
            typeof(DocumentObject).GetProperty("Nomen").GetSetMethod(true).Invoke(this, new object[] { b });
        }

        private GH_Component m_component;
        public GH2GH2_COM()
            : base(new Nomen("GH2GH2", "", "GH2GH2", "COM", 2, Rank.Hidden))
        {
            if (Editor.Instance != null)
            {
                if (Editor.Instance.Tabs != null)
                {
                    TabItem t = typeof(Grasshopper.UI.TabbedPanel.TabControl).GetField("_indicatedItem", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Editor.Instance.Tabs) as TabItem;
                    if (t != null)
                    {
                        CompositionItem i = t.Content;
                        if (i != null)
                        {
                            IGH_ObjectProxy o = GH1.Grasshopper.Instances.ComponentServer.EmitObjectProxy(i.Id);
                            if (o != null)
                            {
                                GH_Component GHH = o.CreateInstance() as GH_Component;
                                if (GHH != null)
                                {
                                    COM = GHH;
                                    AddInputs(new InputAdder(this));
                                    AddOutputs(new OutputAdder(this));
                                    VariableParameterMaintenance();
                                }
                            }
                        }
                    }
                }
            }
        }
        public GH2GH2_COM(IReader reader)
           : base(reader)
        {
            Guid ID = reader.Guid128("M_COM");
            IGH_ObjectProxy o = GH1.Grasshopper.Instances.ComponentServer.EmitObjectProxy(ID);
            if (o != null)
            {
                GH_Component GHH = o.CreateInstance() as GH_Component;
                if (GHH != null)
                {
                    COM = GHH;
                }
            }
        }
        private byte[] get_bitmap()
        {
            Bitmap b = m_component.Icon_24x24;
            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }

        private IIcon m_IconInternal;
        protected override IIcon IconInternal => m_IconInternal;

        protected override void AddInputs(InputAdder inputs)
        {
            GH_Component c = m_component as GH_Component;
            if (c != null)
            {
                foreach (IGH_Param p in c.Params.Input)
                {
                    if (p.Type == typeof(GH_Point))
                    {
                        inputs.AddPoint(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Boolean))
                    {
                        inputs.AddBoolean(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Line))
                    {
                        inputs.AddLine(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Vector))
                    {
                        inputs.AddVector(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Plane))
                    {
                        inputs.AddPlane(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Transform))
                    {
                        inputs.AddTransform(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Arc))
                    {
                        inputs.AddArc(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Curve))
                    {
                        inputs.AddCurve(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Brep))
                    {
                        inputs.AddSurface(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Surface))
                    {
                        inputs.AddSurface(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Mesh))
                    {
                        inputs.AddMesh(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Integer))
                    {
                        inputs.AddInteger(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Interval))
                    {
                        inputs.AddInterval(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Number))
                    {
                        inputs.AddNumber(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    inputs.AddGeneric(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                }
            }
        }

        protected override void AddOutputs(OutputAdder outputs)
        {
            GH_Component c = m_component as GH_Component;
            if (c != null)
            {
                foreach (IGH_Param p in c.Params.Output)
                {
                    if (p.Type == typeof(GH_Point))
                    {
                        outputs.AddPoint(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Line))
                    {
                        outputs.AddLine(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Boolean))
                    {
                        outputs.AddBoolean(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Vector))
                    {
                        outputs.AddVector(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Plane))
                    {
                        outputs.AddPlane(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Transform))
                    {
                        outputs.AddTransform(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Arc))
                    {
                        outputs.AddArc(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Curve))
                    {
                        outputs.AddCurve(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Brep))
                    {
                        outputs.AddSurface(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Surface))
                    {
                        outputs.AddSurface(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Mesh))
                    {
                        outputs.AddMesh(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Integer))
                    {
                        outputs.AddInteger(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Interval))
                    {
                        outputs.AddInterval(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    if (p.Type == typeof(GH_Number))
                    {
                        outputs.AddNumber(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                        continue;
                    }
                    outputs.AddGeneric(p.Name, p.NickName, p.Description, (Access)((int)(p.Access)));
                }
            }
        }

        protected override void Process(IDataAccess access)
        {
            try
            {
                if (m_component != null)
                {
                    if (m_component is IGH_TaskCapableComponent)
                        ((IGH_TaskCapableComponent)m_component).UseTasks = false;
                    typeof(GH_Component).GetMethod("SolveInstance", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(m_component, new object[] { new GH_Iterator(access, m_component) });
                }
            }
            catch (Exception EX)
            {
                access.AddError("ERROR", EX.InnerException.Message);
            }
        }
        public override void Store(IWriter writer)
        {
            base.Store(writer);
            if (m_component != null)
                writer.Guid128("M_COM", m_component.ComponentGuid);
        }

    }
}
