using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ManagementOfForeigners.Models;

namespace ManagementOfForeigners.Controllers;

[Route("home")]
public class TrangChuController : Controller
{
    [Route("")]
    [Route("index")]
    [Route("~/")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("privacy")]
    public IActionResult ChinhSachBaoMat()
    {
        return View();
    }

    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Loi()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
