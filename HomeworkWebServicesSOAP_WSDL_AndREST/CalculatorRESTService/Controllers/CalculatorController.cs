using System;
using System.Collections.Generic;
using System.Web.Http;
using CalculatorRESTService.Models;

namespace CalculatorRESTService.Controllers
{
    public class CalculatorController : ApiController
    {
        [HttpGet]
        [Route("api/distance")]
        public double CalculateDistance([FromUri] TwoPoints points)
        {
            Point start = new Point(points.startX, points.startY);
            Point end = new Point(points.endX, points.endY);
            
            return Math.Sqrt(Math.Pow((double)(end.X - start.X), 2) + Math.Pow((double)(end.Y - start.Y), 2));
        }
    }
}
