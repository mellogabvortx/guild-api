﻿using Domain.Services;
using Application.ActionFilters;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Application.Controllers
{
    [Route("api/[controller]/v1"), ApiController]
    public class InvitesController : ControllerBase
    {
        [HttpGet("{id}", Name = "get-invite"), UseCache(20)]
        public ActionResult Get(Guid id, [FromServices] IInviteService service)
        {
            return Ok(service.Get(id));
        }

        [HttpGet(Name = "get-invites"), UseCache(30)]
        public ActionResult GetAll([FromServices] IInviteService service, [FromQuery] InviteDto payload)
        {
            return Ok(service.List(payload));
        }

        [HttpPost(Name = "invite-member"), UseUnitOfWork]
        public ActionResult InviteMember([FromBody] InviteDto payload, [FromServices] IInviteService service)
        {
            var invite = service.InviteMember(payload);

            return CreatedAtRoute("get-invite", new { id = invite.Id }, invite);
        }

        [HttpPatch("{id}/accept", Name = "accept-invite"), UseUnitOfWork]
        public ActionResult Accept(Guid id, [FromServices] IInviteService service)
        {
            return Ok(service.Accept(id));
        }

        [HttpPatch("{id}/decline", Name = "decline-invite"), UseUnitOfWork]
        public ActionResult Decline(Guid id, [FromServices] IInviteService service)
        {
            return Ok(service.Decline(id));
        }

        [HttpPatch("{id}/cancel", Name = "cancel-invite"), UseUnitOfWork]
        public ActionResult Cancel(Guid id, [FromServices] IInviteService service)
        {
            return Ok(service.Cancel(id));
        }

        [HttpDelete("{id}", Name = "delete-invite"), UseUnitOfWork]
        public ActionResult Delete(Guid id, [FromServices] IInviteService service)
        {
            service.Delete(id);
            return NoContent();
        }
    }
}