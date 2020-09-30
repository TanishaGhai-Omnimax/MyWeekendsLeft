﻿using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Validators;
using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace MWL.Services.Implementation
{
    public class WeekendsLeftService : IWeekendsLeftService
    {
        public WeekendsLeftResponse GetWeekendsLeft(WeekendsLeftRequest weekendsLeftRequest)
        {
            var weekendsLeftResponse = new WeekendsLeftResponse();

            // Model Validation                         
            var validator = new WeekendsLeftRequestValidator();
            var results = validator.Validate(weekendsLeftRequest);
            if (!results.IsValid)
            {
                weekendsLeftResponse.Errors = results.Errors.Select(errors => errors.ErrorMessage).ToList();
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            // County Code Validation - jjb
            var cache = new MemoryCache(new MemoryCacheOptions()); // NEED TO DELETE
            var countriesService = new CountriesService(cache);
            if (!countriesService.GetCountryData().ContainsKey(weekendsLeftRequest.Country))
            {
                weekendsLeftResponse.Errors = new[] {"Country Code is not valid"};
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            var rng = new Random();
            var estimatedDayOfDeath = DateTime.Now.AddDays(rng.Next(100, 20000));
            var estimatedDaysLeft = (estimatedDayOfDeath - DateTime.Now).Days;
            var estimatedAgeOfDeath = weekendsLeftRequest.Age + (estimatedDaysLeft / 365);
            var estimatedWeekendsLeft = estimatedDaysLeft / 7;

            weekendsLeftResponse.EstimatedDayOfDeath = estimatedDayOfDeath;
            weekendsLeftResponse.EstimatedAgeOfDeath = estimatedAgeOfDeath;
            weekendsLeftResponse.EstimatedWeekendsLeft = estimatedWeekendsLeft;
            weekendsLeftResponse.Message = $"You have an estimated {estimatedWeekendsLeft} weekends left in your life, get out there and enjoy it!";
            
            return weekendsLeftResponse;
        }
    }
}