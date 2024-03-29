﻿using Microsoft.AspNetCore.Mvc;
using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PantryPlanner.DTOs
{
    /// <summary>
    /// DTO of <see cref="Kitchen"/> that excludes any Collections.
    /// </summary>
    public class KitchenDto
    {
        public long KitchenId { get; set; }
        public Guid UniquePublicGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByUserId { get; set; }

        public string CreatedByUsername { get; set; }

        public KitchenDto(Kitchen kitchen)
        {
            if (kitchen == null)
            {
                return;
            }

            KitchenId = kitchen.KitchenId;
            UniquePublicGuid = kitchen.UniquePublicGuid;
            Name = kitchen.Name;
            Description = kitchen.Description;
            DateCreated = kitchen.DateCreated;
            CreatedByUserId = kitchen.CreatedByUserId;

            CreatedByUsername = kitchen.CreatedByUser?.UserName;
        }

        public static List<KitchenDto> ToList(List<Kitchen> list)
        {
            return list?.Select(k => new KitchenDto(k))?.ToList();
        }

        public override string ToString()
        {
            return $"k: {Name} | created by {CreatedByUsername}";
        }
    }
}
