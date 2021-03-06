﻿using System;
using Business.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Business.Commands.Invites
{
	public class InviteFilterCommand : IRequest<ApiResponse<Pagination<Invite>>>
	{
		[FromQuery(Name = "guildId")] public Guid GuildId { get; set; } = Guid.Empty;
		[FromQuery(Name = "memberId")] public Guid MemberId { get; set; } = Guid.Empty;
		[FromQuery(Name = "count")] public int Count { get; set; } = 20;
	}
}