using System.ComponentModel;

namespace Hutzper.Library.Common.Forms
{
    public class NoFocusedRoundedButton : RoundedButton
    {
        public NoFocusedRoundedButton()
        {
        }

        public NoFocusedRoundedButton(IContainer container)
        {
            container.Add(this);

            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}