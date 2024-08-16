using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyControleer(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "secret text";
    }


    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var things = context.Users.Find(-1);
        if (things == null) return NotFound();

        return things;
    }


     [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError()
    {
            var thing = context.Users.Find(-1) ?? throw new Exception ("A bad thing has happenned");
            return thing; 
    }


    [HttpGet("bad-request")]
    public ActionResult<AppUser> GetBadRequest()
    {
        return BadRequest ("this was not a good request");
    }
}



