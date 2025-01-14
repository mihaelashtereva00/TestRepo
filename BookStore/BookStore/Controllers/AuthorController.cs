﻿using AutoMapper;
using BookStore.BL.Interfaces;
using BookStore.Models.MediatR.Commands;
using BookStore.Models.Models;
using BookStore.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private static readonly List<Author> _testModels = new List<Author>()
        {
            new Author()
            {
            Id = 1,
            Name = "Name1"
            },
            new Author()
            {
            Id = 2,
            Name = "Name2"
            }
        };

        public AuthorController(ILogger<AuthorController> logger, IAuthorService authorService, IMapper mapper, IMediator mediator)
        {
            _logger = logger;
            _authorService = authorService;
            _mapper = mapper;
            _mediator = mediator;
        }

       [ProducesResponseType(StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [HttpPost("Add author")]
       public async Task<IActionResult> AddAuthorAsync([FromBody] AuthorRequest addAuthorRequest)
        {
            var result = await _mediator.Send(new AddAuthorCommand(addAuthorRequest));

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get all")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetAllAuthorsCommand()));
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost(nameof(AddAuthorRangeAsync))]
        public async Task<IActionResult> AddAuthorRangeAsync([FromBody] AddMultipleAuthorsRequest addMultipleAuthors)
        {
            if (addMultipleAuthors != null && !addMultipleAuthors.AuthorRequests.Any())
                return BadRequest(addMultipleAuthors);

            var authorCollection = _mapper.Map<IEnumerable<Author>>(addMultipleAuthors.AuthorRequests);

            var result = await _authorService.AddMultipleAuthors(authorCollection);
            
            if(!result) return BadRequest(result);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest($"Parameter id:{id} must be greater than 0");
            var result = await _mediator.Send(new GetByIdAuthorCommand(id));
            if (result == null) return NotFound(id);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(nameof(GetByName))]
        public async Task<IActionResult> GetByName(string name)
        {
            if (name.Length <= 0) return BadRequest($"Parameter name:{name} must be greater than 0");
            var result = await _authorService.GetByName(name);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("Update author")]
        public async Task<IActionResult> Update(AuthorRequest updateAuthorRequest)
        {
            var result = await _mediator.Send(new UpdateAuthorCommand(updateAuthorRequest));

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete(nameof(Delete))]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0 && _authorService.GetById != null)
            {
                var result = await _mediator.Send(new DeleteAuthorCommand(id));
                return Ok(result);
            }
            return BadRequest();
        }
    }
}

//book update  