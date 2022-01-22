using LocalPrint.Template;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LocalPrint.Template
{

    public class MembaseJsonSerializer<T>
    {

        public MembaseJsonSerializer()
        {
        }


        public R FromJson<R>(object json)
        {
            if (typeof(T).IsAssignableFrom(typeof(R)))
            {
                object res = JsonConvert.DeserializeObject(json.ToString(), new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                return (R)res;
            }
            throw new NotImplementedException("Type is not assignable.");
        }

        public string ToJson(object obj)
        {
            if (typeof(T).IsAssignableFrom(obj.GetType()))
            {
                string json = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                return json;
            }
            throw new NotImplementedException("Type is not assignable.");
        }
    }
    internal class TemplateHelper
    {
        public static PropertyBase ControlProperty(Control control)
        {
            PropertyBase property;
            string type = control.GetType().Name;

            switch (type)
            {
                case "TextBox":
                    property = new TextBoxProperty((TextBox)control);
                    break;
                case "PictureBox":
                    property = new PictureBoxProperty((PictureBox)control);
                    break;
                case "Label":
                    property = new LabelProperty((Label)control);
                    break;
                case "Panel":
                    property = new PanelProperty((Panel)control);
                    break;
                default:
                    property = null;
                    break;
            }
            return property;
        }

    }
    public class ControlProperty : PropertyBase
    {
        private Control _Control;
        public ControlProperty()
        {
            this._Control = new Control();
        }
        public ControlProperty(Control control)
        {
            this._Control = control;
        }

        [MyControlAttibute("宽度", "获取或者设置控件宽度", "")]
        public int Width
        {
            get { return this._Control.Width; }
            set
            {
                this._Control.Width = (int)value;
            }
        }
        [MyControlAttibute("高度", "获取或者设置控件高度", "")]
        public int Height
        {
            get { return this._Control.Height; }
            set
            {
                this._Control.Height = (int)value;
            }
        }
        [MyControlAttibute("上边距", "获取或者设置控件上边位置", "")]
        public int Top
        {
            get { return this._Control.Top; }
            set
            {
                this._Control.Top = value;
            }
        }
        [MyControlAttibute("左边距", "获取或者设置控件左边位置", "")]
        public int Left
        {
            get { return this._Control.Left; }
            set
            {
                this._Control.Left = value;
            }
        }
    }
    public class PanelProperty : PropertyBase
    {
        private Panel _Panel;

        public PanelProperty()
        {
            this._Panel = new Panel();
        }
        public PanelProperty(Panel panel)
        {
            this._Panel = panel;
        }
        [MyControlAttibute("宽度(cm)", "获取或者设置控件宽度", "")]
        public float Width
        {
            get { 
                return (float)Math.Round(this._Panel.Width / 39f,2); 
            }
            set
            {
                this._Panel.Width = (int)(value * 39f);
            }
        }
        [MyControlAttibute("高度(cm)", "获取或者设置控件高度", "")]
        public float Height
        {
            get { 
                return (float)Math.Round(this._Panel.Height / 39f, 2); 
            }
            set
            {
                this._Panel.Height = (int)(value * 39f);
            }
        }
        [MyControlAttibute("背景图", "获取或者设置容器背景", "")]
        public string Img
        {
            get { return this._Panel.Tag.ToString(); }
            set
            {
                this._Panel.Tag = value;
                if (File.Exists(value))//读取时先要判读INI文件是否存在
                {
                    this._Panel.BackgroundImage = Image.FromFile(value);
                }
                else
                {
                    this._Panel.BackgroundImage = null;
                }
            }
        }

        public List<PropertyBase> List { get; set; }

        public void InitList()
        {
            List<PropertyBase> tmplist = new List<PropertyBase>();
            foreach (Control ctl in this._Panel.Controls)
            {
                PropertyBase propertyBase = TemplateHelper.ControlProperty(ctl);
                if (propertyBase != null)
                    tmplist.Add(propertyBase);
            }
            this.List = tmplist;
        }
    }
    public class TextBoxProperty : PropertyBase
    {
        private TextBox _Control;

        public TextBoxProperty()
        {
            this._Control = new TextBox();
        }
        public TextBoxProperty(TextBox control)
        {
            this._Control = control;
        }
        [MyControlAttibute("文本", "获取或者设置控件文本", "")]
        public string Text
        {
            get { return this._Control.Text; }
            set
            {
                this._Control.Text = value;
            }
        }
        [MyControlAttibute("宽度", "获取或者设置控件宽度", "")]
        public int Width
        {
            get { return this._Control.Width; }
            set
            {
                this._Control.Width = (int)value;
            }
        }
        [MyControlAttibute("高度", "获取或者设置控件高度", "")]
        public int Height
        {
            get {
                this._Control.Multiline = true;//这里要设置为多行,不然高度会固定为21
                return this._Control.Height; 
            }
            set
            {
                this._Control.Height = (int)value;
            }
        }
        [MyControlAttibute("上边距", "获取或者设置控件上边位置", "")]
        public int Top
        {
            get { return this._Control.Top; }
            set
            {
                this._Control.Top = value;
            }
        }
        [MyControlAttibute("左边距", "获取或者设置控件左边位置", "")]
        public int Left
        {
            get { return this._Control.Left; }
            set
            {
                this._Control.Left = value;
            }
        }
        [MyControlAttibute("字体大小", "获取或者设置字体大小", "")]
        public int Size
        {
            get { return (int)this._Control.Font.Size; }
            set
            {
                this._Control.Font = new Font(this._Control.Font.FontFamily, value, this._Control.Font.Style);
            }
        }
        [MyControlAttibute("字体粗细", "获取或者设置字体粗细", "")]
        public bool Bold
        {
            get { return this._Control.Font.Bold; }
            set
            {
                this._Control.Font = new Font(this._Control.Font, value? FontStyle.Bold: FontStyle.Regular);
            }
        }
    }

    public class PictureBoxProperty : PropertyBase
    {
        private PictureBox _Control;

        public PictureBoxProperty()
        {
            this._Control = new PictureBox();
        }
        public PictureBoxProperty(PictureBox control)
        {
            this._Control = control;
        }

        [MyControlAttibute("条码内容", "获取或者设置条码内容", "")]
        public string Code
        {
            get { return this._Control.Tag.ToString(); }
            set
            {
                this._Control.Tag = value;
                this._Control.Image = CreateBarcodePicture(value,this.Width,this.Height);
            }
        }
        [MyControlAttibute("宽度", "获取或者设置控件宽度", "")]
        public int Width
        {
            get { return this._Control.Width; }
            set
            {
                this._Control.Width = (int)value;
            }
        }
        [MyControlAttibute("高度", "获取或者设置控件高度", "")]
        public int Height
        {
            get { return this._Control.Height; }
            set
            {
                this._Control.Height = (int)value;
            }
        }
        [MyControlAttibute("上边距", "获取或者设置控件上边位置", "")]
        public int Top
        {
            get { return this._Control.Top; }
            set
            {
                this._Control.Top = value;
            }
        }
        [MyControlAttibute("左边距", "获取或者设置控件左边位置", "")]
        public int Left
        {
            get { return this._Control.Left; }
            set
            {
                this._Control.Left = value;
            }
        }

        public static System.Drawing.Image CreateBarcodePicture(string BarcodeString, int ImgWidth, int ImgHeight)
        {
            try
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();//实例化一个条码对象
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;//编码类型
                b.BackColor = System.Drawing.Color.White;//图片背景颜色  
                b.ForeColor = System.Drawing.Color.Black;//条码颜色  
                b.IncludeLabel = true;
                b.Alignment = BarcodeLib.AlignmentPositions.CENTER;
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                b.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;//图片格式  
                System.Drawing.Font font = new System.Drawing.Font("verdana", 10f);//字体设置  
                b.LabelFont = font;
                b.Width = ImgWidth;//图片宽度设置(px单位)
                b.Height = ImgHeight;//图片高度设置(px单位)

                //获取条码图片
                System.Drawing.Image BarcodePicture = b.Encode(type, BarcodeString);

                //BarcodePicture.Save(@"D:\Barcode.jpg");


                return BarcodePicture;
            }
            catch
            {
                return null;
            }
        }
    }

    public class LabelProperty : PropertyBase
    {
        private Label _Control;

        public LabelProperty()
        {
            this._Control = new Label();
        }
        public LabelProperty(Label control)
        {
            this._Control = control;
        }
        [MyControlAttibute("宽度", "获取或者设置控件宽度", "")]
        public int Width
        {
            get { return this._Control.Width; }
            set
            {
                this._Control.Width = (int)value;
            }
        }
        [MyControlAttibute("高度", "获取或者设置控件高度", "")]
        public int Height
        {
            get { return this._Control.Height; }
            set
            {
                this._Control.Height = (int)value;
            }
        }
        [MyControlAttibute("上边距", "获取或者设置控件上边位置", "")]
        public int Top
        {
            get { return this._Control.Top; }
            set
            {
                this._Control.Top = value;
            }
        }
        [MyControlAttibute("左边距", "获取或者设置控件左边位置", "")]
        public int Left
        {
            get { return this._Control.Left; }
            set
            {
                this._Control.Left = value;
            }
        }

    }
    public delegate void PropertyChanged(object Value);
    /// <summary>
    /// 主要是实现中文化属性显示
    /// </summary>
    public class PropertyBase : ICustomTypeDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        #region ICustomTypeDescriptor 显式接口定义
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList props = new ArrayList();
            Type thisType = this.GetType();
            PropertyInfo[] pis = thisType.GetProperties();
            foreach (PropertyInfo p in pis)
            {
                if (p.DeclaringType == thisType || p.PropertyType.ToString() == "System.Drawing.Color")
                {
                    //判断属性是否显示
                    BrowsableAttribute Browsable = (BrowsableAttribute)Attribute.GetCustomAttribute(p, typeof(BrowsableAttribute));
                    if (Browsable != null)
                    {
                        if (Browsable.Browsable == true || p.PropertyType.ToString() == "System.Drawing.Color")
                        {
                            PropertyStub psd = new PropertyStub(p, attributes);
                            props.Add(psd);
                        }
                    }
                    else
                    {
                        PropertyStub psd = new PropertyStub(p, attributes);
                        props.Add(psd);
                    }
                }
            }
            PropertyDescriptor[] propArray = (PropertyDescriptor[])props.ToArray(typeof(PropertyDescriptor));
            return new PropertyDescriptorCollection(propArray);
        }
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyStub : PropertyDescriptor
    {
        PropertyInfo info;
        public PropertyStub(PropertyInfo propertyInfo, Attribute[] attrs)
            : base(propertyInfo.Name, attrs)
        {
            this.info = propertyInfo;
        }
        public override Type ComponentType
        {
            get { return this.info.ReflectedType; }
        }
        public override bool IsReadOnly
        {
            get { return this.info.CanWrite == false; }
        }
        public override Type PropertyType
        {
            get { return this.info.PropertyType; }
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override object GetValue(object component)
        {
            //Console.WriteLine("GetValue: " + component.GetHashCode());
            try
            {
                return this.info.GetValue(component, null);
            }
            catch
            {
                return null;
            }
        }
        public override void ResetValue(object component)
        {
        }
        public override void SetValue(object component, object value)
        {
            //Console.WriteLine("SetValue: " + component.GetHashCode());
            this.info.SetValue(component, value, null);
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        //通过重载下面这个属性，可以将属性在PropertyGrid中的显示设置成中文
        public override string DisplayName
        {
            get
            {
                if (info != null)
                {
                    MyControlAttibute uicontrolattibute = (MyControlAttibute)Attribute.GetCustomAttribute(info, typeof(MyControlAttibute));
                    if (uicontrolattibute != null)
                        return uicontrolattibute.PropertyName;
                    else
                    {
                        return info.Name;
                    }
                }
                else
                    return "";
            }
        }
    }
    public class MyControlAttibute : Attribute
    {
        private string _PropertyName;
        private string _PropertyDescription;
        private object _DefaultValue;
        public MyControlAttibute(string Name, string Description, object DefalutValue)
        {
            this._PropertyName = Name;
            this._PropertyDescription = Description;
            this._DefaultValue = DefalutValue;
        }
        public MyControlAttibute(string Name, string Description)
        {
            this._PropertyName = Name;
            this._PropertyDescription = Description;
            this._DefaultValue = "";
        }
        public MyControlAttibute(string Name)
        {
            this._PropertyName = Name;
            this._PropertyDescription = "";
            this._DefaultValue = "";
        }
        public string PropertyName
        {
            get { return this._PropertyName; }
        }
        public string PropertyDescription
        {
            get { return this._PropertyDescription; }
        }
        public object DefaultValue
        {
            get { return this._DefaultValue; }
        }
    }
}
