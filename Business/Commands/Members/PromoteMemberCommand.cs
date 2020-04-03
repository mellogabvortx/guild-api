﻿using Business.ResponseOutputs;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Business.Commands.Members
{
  public class PromoteMemberCommand : IRequest<ApiResponse<Member>>
  {
    public Guid Id { get; set; }
    public Member Member { get; private set; }

    public PromoteMemberCommand([FromRoute(Name = "id")] Guid id, [FromServices] IMemberRepository repository)
    {
      Id = id;
      Member = repository.GetForGuildOperationsAsync(id).Result;
    }
  }
}