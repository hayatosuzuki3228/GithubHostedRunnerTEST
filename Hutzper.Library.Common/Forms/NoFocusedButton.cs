using System.ComponentModel;

namespace Hutzper.Library.Common.Forms
{
    public class NoFocusedButton : Button
    {
        public NoFocusedButton()
        {
        }

        public NoFocusedButton(IContainer container)
        {
            container.Add(this);

            SetStyle(ControlStyles.Selectable, false);
        }
    }
}