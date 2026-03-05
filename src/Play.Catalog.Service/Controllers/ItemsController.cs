using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
    [ApiController, Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion","Restore a small amount of HP",5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidot","Cures posion",7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze sword","Deals a small amount of damage",20, DateTimeOffset.UtcNow)
        };

        [HttpGet]
        public IEnumerable<ItemDto> Get() => items;

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault();
            if (item == null)
                return NotFound();

            return item;
        }

        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            if (Guid.TryParse(id.ToString(), out Guid itemId))
            {
                var existingItem = items.SingleOrDefault(item => item.Id == id);

                if (existingItem == null)
                {
                    var item = new ItemDto(Guid.NewGuid(), updateItemDto.Name, updateItemDto.Description, updateItemDto.Price, DateTimeOffset.UtcNow);
                    items.Add(item);

                    return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
                }
                else
                {
                    var updateItem = existingItem with
                    {
                        Name = updateItemDto.Name,
                        Description = updateItemDto.Description,
                        Price = updateItemDto.Price
                    };

                    int idx = items.FindIndex(existingItem => existingItem.Id == id);
                    items[idx] = updateItem;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            int idx = items.FindIndex(item => item.Id == id);
            if (idx < 0)
                return NoContent();
            items.RemoveAt(idx);

            return NoContent();
        }
    }
}