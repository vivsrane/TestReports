namespace VB.Common.Core.Web.MVP
{
    public interface IView<T> where T : IView<T>
    {
        void Bind();
    }
}