using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    /// <summary>
    /// Interface of the controller can be returned to the original state
    /// </summary>
    public interface IRestoreStateController
    {
        /// <summary>
        /// To return to the original state
        /// </summary>
        /// <returns>ActionResult</returns>
        ActionResult Restore();
    }
}
