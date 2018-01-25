namespace VB.Common.Core.Web.MVP
{
    public interface IPresenter<TView> where TView : IView<TView>
    {
        TView View { get; set; }

        void Initialize();

        void Load();

        void PreRender();
    }
}