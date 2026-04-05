using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EColor.API.Data;
using EColor.API.Models;
using System.Text.Json;

namespace EColor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionOrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductionOrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductionOrder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionOrder>>> GetProductionOrders()
        {
            return await _context.ProductionOrders
                .Include(p => p.Items)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // GET: api/ProductionOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionOrder>> GetProductionOrder(int id)
        {
            var productionOrder = await _context.ProductionOrders
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productionOrder == null)
            {
                return NotFound();
            }

            return productionOrder;
        }

        // POST: api/ProductionOrder
        [HttpPost]
        public async Task<ActionResult<ProductionOrder>> PostProductionOrder(ProductionOrder productionOrder)
        {
            // 檢查單號是否重複
            if (await _context.ProductionOrders.AnyAsync(p => p.OrderNo == productionOrder.OrderNo))
            {
                return BadRequest(new { message = "單號已存在，請使用其他單號。" });
            }

            _context.ProductionOrders.Add(productionOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductionOrder", new { id = productionOrder.Id }, productionOrder);
        }

        // PUT: api/ProductionOrder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductionOrder(int id, ProductionOrder productionOrder)
        {
            if (id != productionOrder.Id)
            {
                return BadRequest();
            }

            // 確保原本的 Items 被更新（簡單的做法是刪除舊的再新增，或使用追蹤更新）
            var existingOrder = await _context.ProductionOrders
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // 更新主檔：排除 Id 欄位
            _context.Entry(existingOrder).CurrentValues.SetValues(productionOrder);
            existingOrder.Id = id; // 確保 ID 不會被覆寫
            existingOrder.SizesJson = productionOrder.SizesJson;

            // 更新明細：先清除舊的，再加入新的
            if (existingOrder.Items != null)
            {
                _context.ProductionOrderItems.RemoveRange(existingOrder.Items);
            }

            if (productionOrder.Items != null)
            {
                foreach (var item in productionOrder.Items)
                {
                    item.Id = 0; // 確保視為新項目
                    item.ProductionOrderId = id;
                }
                existingOrder.Items = productionOrder.Items;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductionOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingOrder); // 回傳更新後的物件
        }

        // DELETE: api/ProductionOrder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductionOrder(int id)
        {
            var productionOrder = await _context.ProductionOrders.FindAsync(id);
            if (productionOrder == null)
            {
                return NotFound();
            }

            _context.ProductionOrders.Remove(productionOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductionOrderExists(int id)
        {
            return _context.ProductionOrders.Any(e => e.Id == id);
        }
    }
}
