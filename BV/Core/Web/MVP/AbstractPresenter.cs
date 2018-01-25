namespace VB.Common.Core.Web.MVP
{
    public class AbstractPresenter<TView> : IPresenter<TView>
        where TView : IView<TView>
    {
        private TView view;

        public virtual void Initialize()
        {
            // no-op
        }

        public virtual void Load()
        {
            // no-op
        }

        public virtual void PreRender()
        {
            // no-op
        }

        public TView View
        {
            get { return view; }
            set { view = value; }
        }
    }
}