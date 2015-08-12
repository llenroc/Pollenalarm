﻿using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Pollenalarm.Backend.Controllers.Base;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services;

namespace Pollenalarm.Backend.Controllers
{
    [RoutePrefix("api/pollution")]
    public class PollutionController : PollenalarmApiControllerBase
    {
        private readonly PollutionService pollutionService;

        public PollutionController()
        {
            pollutionService = new PollutionService();
        }

        // GET: api/Pollution?zip=52080
        /// <summary>
        /// Gets the current pollution of a single city
        /// </summary>
        /// <param name="zip">Zip </param>
        /// <returns>Pollution values</returns>
        [HttpGet]
        [ResponseType(typeof(List<Pollution>))]
        public IHttpActionResult GetPollutionForCity(string zip)
        {
            var result = pollutionService.GetPollutionsForCity(zip);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Content);
        }

		public IHttpActionResult GetMapPollution()
		{
		    return Ok();
		}
    }
}
