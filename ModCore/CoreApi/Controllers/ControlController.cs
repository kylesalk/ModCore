using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.CoreApi.Controllers
{
    [Route("")]
    public class ControlController : Controller
    {
        private MainCore core;

        public ControlController(CoreContainer cont)
        {
            this.core = cont.mcore;
        }

        public IActionResult Index()
        {
            ViewBag.BotAccountName = $"{core.Shards[0].Client.CurrentUser.Username}#{core.Shards[0].Client.CurrentUser.Discriminator}";
            ViewBag.BotProfilePic = core.Shards[0].Client.CurrentUser.AvatarUrl;
            return View("Index");
        }
    }
}
