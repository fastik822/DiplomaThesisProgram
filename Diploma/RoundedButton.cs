using System.Windows;
using System.Windows.Controls.Primitives;

namespace Diploma
{
   
    public class RoundedButton : ButtonBase
    {


        public CornerRadius Corner
        {
            get { return (CornerRadius)GetValue(CornerProperty); }
            set { SetValue(CornerProperty, value); }
        }

        public static readonly DependencyProperty CornerProperty =
            DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(RoundedButton), new PropertyMetadata(new CornerRadius(0)));


        static RoundedButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundedButton), new FrameworkPropertyMetadata(typeof(RoundedButton)));
        }
    }
}
