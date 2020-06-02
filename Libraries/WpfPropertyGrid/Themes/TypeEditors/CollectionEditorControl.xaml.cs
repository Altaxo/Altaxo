using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPG.Data;
using WPG.TypeEditors;

namespace WPG.Themes.TypeEditors
{
    public partial class CollectionEditorControl : UserControl
    {


        public Property MyProperty
        {
            get { return (Property)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(Property), typeof(CollectionEditorControl), new UIPropertyMetadata(null));


        public Type NumerableType
        {
            get { return (Type)GetValue(NumerableTypeProperty); }
            set { SetValue(NumerableTypeProperty, value); }
        }

        public static readonly DependencyProperty NumerableTypeProperty =
            DependencyProperty.Register("NumerableType", typeof(Type), typeof(CollectionEditorControl), new UIPropertyMetadata(null));

        public IEnumerable NumerableValue
        {
            get { return (IEnumerable)GetValue(NumerableValueProperty); }
            set { SetValue(NumerableValueProperty, value); }
        }

        public static readonly DependencyProperty NumerableValueProperty =
            DependencyProperty.Register("NumerableValue", typeof(IEnumerable), typeof(CollectionEditorControl), new UIPropertyMetadata(null));

        public CollectionEditorControl()
        {
            InitializeComponent();
            //txtTypeName.Text = NumerableType.GetType().ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CollectionEditorWindow collEdt = new CollectionEditorWindow(this);
            collEdt.ShowDialog();
        }


    }
}
