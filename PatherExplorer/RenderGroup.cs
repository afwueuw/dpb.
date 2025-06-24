
using System.Windows.Media;

namespace PatherExplorer
{
    public abstract class RenderGroup
    {
        private bool _enabled;

        public RenderGroup(System.Windows.Controls.Image viewport)
        {
            View = viewport;
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                }
            }
        }

        public System.Windows.Controls.Image View { get; private set; }

        public abstract void Render(PatherExplorerData data);

    }
}
